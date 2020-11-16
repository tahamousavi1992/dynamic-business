using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class TaskEngine : BaseEngine
    {
        public TaskEngine(EngineSharedModel engineSharedModel, IUnitOfWork unitOfWork = null) : base(engineSharedModel, unitOfWork) { }

        public bool CheckUserAccess(Domain.sysBpmsTask task, Guid? userID, List<sysBpmsDepartmentMember> list)
        {
            switch ((Domain.sysBpmsTask.e_OwnerTypeLU)task.OwnerTypeLU.Value)
            {
                case Domain.sysBpmsTask.e_OwnerTypeLU.Role:
                    switch ((UserTaskRuleModel.e_RoleAccessType)task.UserTaskRuleModel.AccessType)
                    {
                        case UserTaskRuleModel.e_RoleAccessType.Static:
                        case UserTaskRuleModel.e_RoleAccessType.Variable:
                            if (task.UserTaskRuleModel.AccessType == (int)UserTaskRuleModel.e_RoleAccessType.Variable)
                                task.SetRoleDepartment(new DataManageEngine(base.EngineSharedModel, base.UnitOfWork).GetValueByBinding(task.UserTaskRuleModel.Variable).ToStringObj(), task.UserTaskRuleModel.SpecificDepartmentId);
                            return
                                   (!task.IsInRole(null, (int)sysBpmsDepartmentMember.e_RoleLU.Requester) || base.EngineSharedModel.CurrentThread == null || (base.EngineSharedModel.CurrentThread?.UserID ?? Guid.Empty) == userID)
                                   &&
                                   list.Any(c => task.RoleName == string.Empty || task.IsInRole(c.DepartmentID, c.RoleLU));
                        case UserTaskRuleModel.e_RoleAccessType.CorrespondentRole:
                            Guid? departmentId = new AccessCodeHelper(base.UnitOfWork).GetDepartmentHierarchyByUserId(userID.ToGuidObj(), task.UserTaskRuleModel.RoleCode.ToIntObj(), task.UserTaskRuleModel.GoUpDepartment);
                            return departmentId.HasValue && list.Any(f => f.DepartmentID == departmentId && f.RoleLU == task.UserTaskRuleModel.RoleCode.ToIntObj());
                    }
                    break;
                case Domain.sysBpmsTask.e_OwnerTypeLU.User:
                    switch ((UserTaskRuleModel.e_UserAccessType)task.UserTaskRuleModel.AccessType)
                    {
                        case UserTaskRuleModel.e_UserAccessType.Static:
                            return task.UserID.ToStringObj().Contains(userID.ToStringObj());
                        case UserTaskRuleModel.e_UserAccessType.Variable:
                            return new DataManageEngine(base.EngineSharedModel, base.UnitOfWork).GetValueByBinding(task.UserTaskRuleModel.Variable).ToStringObj().Contains(userID.ToStringObj());
                    }
                    break;
            }
            return true;
        }

        public bool CheckAccessByThreadTask(Domain.sysBpmsTask task, sysBpmsThreadTask threadTask, Guid? userID, List<sysBpmsDepartmentMember> list = null)
        {
            list = list ?? (userID.HasValue ? new DepartmentMemberService().GetList(null, null, userID) : new List<sysBpmsDepartmentMember>());
            if (!string.IsNullOrWhiteSpace(threadTask.OwnerRole))
            {
                return (
                    !threadTask.IsInRole(null, (int)sysBpmsDepartmentMember.e_RoleLU.Requester) ||
                     base.EngineSharedModel.CurrentThread == null || 
                     (base.EngineSharedModel.CurrentThread?.UserID ?? Guid.Empty) == userID)
                           &&
                           list.Any(c => (string.IsNullOrWhiteSpace(threadTask.OwnerRole) || threadTask.IsInRole(c.DepartmentID, c.RoleLU)));
            }
            else
                return threadTask.OwnerUserID == userID;
        }

        public ResultOperation DoneThreadTask(sysBpmsThreadTask currentThreadTask, sysBpmsUser currentUser)
        {
            ThreadTaskService threadTaskService = new ThreadTaskService(base.UnitOfWork);
            currentThreadTask.Update(DateTime.Now, (int)sysBpmsThreadTask.e_StatusLU.Done,
              currentThreadTask.OwnerUserID ?? currentUser?.ID);
            return threadTaskService.Update(currentThreadTask);
        }

        /// <summary>
        /// It is called by Process engine for adding theadtask related to task element.  
        /// </summary>
        /// <param name="item"></param>
        public (ResultOperation result, List<sysBpmsThreadTask> listTask) AddThreadTask(sysBpmsTask item, bool isFirstTask)
        {
            ThreadTaskService threadTaskService = new ThreadTaskService(base.UnitOfWork);
            ResultOperation result = new ResultOperation();
            List<sysBpmsThreadTask> listTask = new List<sysBpmsThreadTask>();
            switch ((sysBpmsTask.e_TypeLU)item.TypeLU)
            {
                case sysBpmsTask.e_TypeLU.UserTask:
                    if (item.MarkerTypeLU.HasValue)
                    {
                        foreach (sysBpmsThreadTask threadTaskItem in threadTaskService.GetMultiInstanceTask(item, base.EngineSharedModel))
                        {
                            //if user has access to this method, set thread task access Info to userID
                            if (isFirstTask && this.CheckAccessByThreadTask(item, threadTaskItem, base.EngineSharedModel.CurrentThread.UserID, null))
                                threadTaskItem.UpdateAccessInfo(base.EngineSharedModel.CurrentThread.UserID, null);
                            threadTaskService.Add(threadTaskItem);
                            listTask.Add(threadTaskItem);
                        }
                    }
                    else
                    {
                        sysBpmsThreadTask threadTask = new sysBpmsThreadTask();
                        threadTask.Update(base.EngineSharedModel.CurrentThreadID.Value, item.ID, DateTime.Now, null, string.Empty, 0, (int)sysBpmsThreadTask.e_StatusLU.New);
                        threadTaskService.SetAccessInfoTo(threadTask, item, base.EngineSharedModel);
                        //if user has access to this method, set thread task access Info to thread userID
                        if (isFirstTask && this.CheckAccessByThreadTask(item, threadTask, base.EngineSharedModel.CurrentThread.UserID, null))
                            threadTask.UpdateAccessInfo(base.EngineSharedModel.CurrentThread.UserID, null);

                        threadTaskService.Add(threadTask);
                        listTask.Add(threadTask);
                    }
                    break;
            }
            return (result, listTask);
        }
    }
}
