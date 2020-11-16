using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DynamicBusiness.BPMS.SharedPresentation;
using System;

namespace DynamicBusiness.BPMS.Controllers
{
    public class BpmsTaskController : BpmsAdminApiControlBase
    {
        public object GetIndex(string ElementId, Guid ProcessId)
        {
            using (TaskService taskService = new TaskService())
            {
                TaskDTO task = new TaskDTO(taskService.GetInfo(ElementId, ProcessId));

                using (DepartmentService departmentService = new DepartmentService())
                using (UserService userService = new UserService())
                //for access
                using (LURowService luRowService = new LURowService())
                using (DynamicFormService dynamicFormService = new DynamicFormService())
                using (ProcessService processService = new ProcessService())
                using (StepService stepService = new StepService())
                    return new
                    {
                        ListSteps = stepService.GetList(task.ID, null).Select(c => new StepDTO(c)).ToList(),
                        AllowEdit = processService.GetInfo(ProcessId).AllowEdit(),
                        RoleAccessTypes = EnumObjHelper.GetEnumList<UserTaskRuleModel.e_RoleAccessType>().Select(c => new QueryModel(c.Key.ToString(), c.Value)).ToList(),
                        UserAccessTypes = EnumObjHelper.GetEnumList<UserTaskRuleModel.e_UserAccessType>().Select(c => new QueryModel(c.Key.ToString(), c.Value)).ToList(),
                        UserTypes = EnumObjHelper.GetEnumList<UserTaskRuleModel.e_UserType>().Select(c => new QueryModel(c.Key.ToString(), c.Value)).ToList(),
                        Departments = departmentService.GetList(true, "", null).Select(c => new QueryModel(c.ID.ToString(), c.Name)).ToList(),
                        Users = userService.GetList("", null).Select(c => new QueryModel(c.ID.ToString(), c.FullName)).ToList(),
                        UsersJson = (task.MarkerTypeLU.HasValue ? userService.GetList("", null) : new List<sysBpmsUser>()).Select(c => new ComboTreeModel()
                        {
                            id = c.ID.ToString(),
                            title = c.FullName
                        }).ToList(),
                        OwnerTypes = luRowService.GetList(sysBpmsLUTable.e_LUTable.LaneOwnerTypeLU.ToString()).Select(c => new QueryModel(c.CodeOf, c.NameOf)).ToList(),
                        RoleNames = luRowService.GetList("DepartmentRoleLU").Select(c => new QueryModel(c.CodeOf, c.NameOf)).ToList(),
                        DynamicForms = dynamicFormService.GetList(ProcessId, null, false, string.Empty, null, null).Select(c => new QueryModel(c.ID.ToString(), c.Name)).ToList(),
                        RoleNamesJson = luRowService.GetList("DepartmentRoleLU").Select(c => new ComboTreeModel()
                        {
                            id = c.CodeOf,
                            title = c.NameOf,
                        }).ToList(),
                        Model = task,
                    };
            }

        }

        [HttpPost]
        public object PostAddEditUserTask(PostAddEditUserTaskDTO userTaskDTO)
        {
            using (ProcessService processService = new ProcessService())
            {
                if (!processService.GetInfo(userTaskDTO.ProcessID).AllowEdit())
                    return new PostMethodMessage(LangUtility.Get("NotAllowEdit.Text", nameof(sysBpmsProcess)), DisplayMessageType.error);
            }
            ResultOperation resultOperation = new ResultOperation();
            using (TaskService taskService = new TaskService())
            {
                //save access
                sysBpmsTask task = taskService.GetInfo(userTaskDTO.ID);
                UserTaskRuleModel userTaskRuleModel = new UserTaskRuleModel();
                if (userTaskDTO.OwnerTypeLU.HasValue)
                {
                    switch ((sysBpmsTask.e_OwnerTypeLU)userTaskDTO.OwnerTypeLU)
                    {
                        case sysBpmsTask.e_OwnerTypeLU.User:
                            userTaskRuleModel.AccessType = userTaskDTO.UserAccessType.ToIntObj();
                            switch ((UserTaskRuleModel.e_UserAccessType)userTaskDTO.UserAccessType)
                            {
                                case UserTaskRuleModel.e_UserAccessType.Static:
                                    //userTaskDTO.UserID is filled automativally
                                    break;
                                case UserTaskRuleModel.e_UserAccessType.Variable:
                                    userTaskRuleModel.Variable = userTaskDTO.ddlUserVariable;
                                    break;
                            }

                            break;
                        case sysBpmsTask.e_OwnerTypeLU.Role:
                            userTaskRuleModel.AccessType = userTaskDTO.RoleAccessType.ToIntObj();
                            switch ((UserTaskRuleModel.e_RoleAccessType)userTaskDTO.RoleAccessType)
                            {
                                case UserTaskRuleModel.e_RoleAccessType.Static:
                                    //userTaskRuleModel.RoleCode = Request.Form["RoleName"];
                                    userTaskRuleModel.SpecificDepartmentId = userTaskDTO.SpecificDepartmentID;
                                    break;
                                case UserTaskRuleModel.e_RoleAccessType.Variable:
                                    userTaskRuleModel.Variable = userTaskDTO.ddlRoleVariable;
                                    userTaskRuleModel.SpecificDepartmentId = userTaskDTO.SpecificDepartmentID;
                                    break;
                                case UserTaskRuleModel.e_RoleAccessType.CorrespondentRole:
                                    userTaskRuleModel.RoleCode = userTaskDTO.ddlRoleRuleRoleName;
                                    userTaskRuleModel.UserType = userTaskDTO.ddlRoleRuleUserType;
                                    userTaskRuleModel.GoUpDepartment = userTaskDTO.GoUpDepartment;
                                    break;
                            }
                            break;
                    }

                    userTaskDTO.Rule = userTaskRuleModel.BuildXml();
                    resultOperation = task.Update(userTaskDTO.RoleName, userTaskDTO.SpecificDepartmentID, userTaskDTO.OwnerTypeLU, userTaskDTO.UserID, userTaskDTO.Rule);
                    if (!resultOperation.IsSuccess)
                        return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                }
                resultOperation = taskService.Update(task);

                //save step
                int index = 0;
                userTaskDTO.ListSteps = userTaskDTO.ListSteps ?? new List<StepDTO>();
                userTaskDTO.ListSteps.ForEach(c => { c.Position = ++index; });

                using (StepService stepService = new StepService())
                {
                    List<sysBpmsStep> CurrentSteps = stepService.GetList(userTaskDTO.ID, null);
                    foreach (sysBpmsStep item in CurrentSteps.Where(c => !userTaskDTO.ListSteps.Any(d => d.ID == c.ID)))
                    {
                        resultOperation = stepService.Delete(item.ID);
                        if (!resultOperation.IsSuccess)
                            return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                    }
                    foreach (StepDTO item in userTaskDTO.ListSteps)
                    {
                        resultOperation = null;
                        if (item.ID != Guid.Empty)
                            resultOperation = stepService.Update(new sysBpmsStep(item.ID, item.TaskID, item.Position, item.DynamicFormID, item.Name));
                        else
                            resultOperation = stepService.Add(new sysBpmsStep(item.ID, item.TaskID, item.Position, item.DynamicFormID, item.Name));

                        if (!resultOperation.IsSuccess)
                            return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                    }

                    if (resultOperation.IsSuccess)
                        return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                    else
                        return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                }
            }
        }

        [HttpPost]
        public object PostAddEditServiceTask(PostAddEditServiceTaskDTO model)
        {
            using (ProcessService processService = new ProcessService())
            {
                if (!processService.GetInfo(base.ProcessId.Value).AllowEdit())
                    return new PostMethodMessage(LangUtility.Get("NotAllowEdit.Text", nameof(sysBpmsProcess)), DisplayMessageType.error);
            }
            using (TaskService taskService = new TaskService())
            {
                sysBpmsTask task = taskService.GetInfo(model.ElementId, base.ProcessId.Value);
                task.UpdateCode(model.DesignCode);
                ResultOperation resultOperation = taskService.Update(task);

                if (!resultOperation.IsSuccess)
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
            return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);

        }

    }
}