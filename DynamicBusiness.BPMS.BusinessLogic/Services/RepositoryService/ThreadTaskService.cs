using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ThreadTaskService : ServiceBase
    {
        public ThreadTaskService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsThreadTask threadTask)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IThreadTaskRepository>().Add(threadTask);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsThreadTask ThreadTask)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IThreadTaskRepository>().Update(ThreadTask);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public sysBpmsThreadTask GetInfo(Guid ID, string[] Includes = null)
        {
            return this.UnitOfWork.Repository<IThreadTaskRepository>().GetInfo(ID, Includes);
        }

        public bool HasAny(Guid processId, Guid taskId)
        {
            return this.UnitOfWork.Repository<IThreadTaskRepository>().HasAny(processId, taskId);
        }
        /// <summary>
        /// OrderByDescending(c => c.StartDate)
        /// </summary>
        public List<sysBpmsThreadTask> GetList(Guid ThreadID, int? TaskType, Guid? taskId, int? statusLU, string[] Includes = null)
        {
            return this.UnitOfWork.Repository<IThreadTaskRepository>().GetList(ThreadID, TaskType, taskId, statusLU, Includes);
        }
        /// <summary>
        /// Include(c => c.sysBpmsTask) andtask with new and ongoing status.
        /// </summary>
        public List<sysBpmsThreadTask> GetListRunning(Guid ThreadID)
        {
            return this.UnitOfWork.Repository<IThreadTaskRepository>().GetListRunning(ThreadID);
        }

        public List<sysBpmsThreadTask> GetListKartable(Guid UserID, int[] statusLU, PagingProperties currentPaging)
        {
            return this.UnitOfWork.Repository<IThreadTaskRepository>().GetListKartable(UserID, statusLU, currentPaging);
        }

        public void SetAccessInfoTo(sysBpmsThreadTask threadTask, Domain.sysBpmsTask task, EngineSharedModel engineSharedModel)
        {
            if (task.OwnerTypeLU.HasValue)
            {
                switch ((sysBpmsTask.e_OwnerTypeLU)task.OwnerTypeLU)
                {
                    case sysBpmsTask.e_OwnerTypeLU.User:
                        switch ((UserTaskRuleModel.e_UserAccessType)task.UserTaskRuleModel.AccessType)
                        {
                            case UserTaskRuleModel.e_UserAccessType.Static:
                                threadTask.UpdateAccessInfo(task.UserID.ToGuidObj(), null);
                                break;
                            case UserTaskRuleModel.e_UserAccessType.Variable:
                                Guid? variableUserID = new DataManageEngine(engineSharedModel, base.UnitOfWork).GetValueByBinding(task.UserTaskRuleModel.Variable).ToGuidObjNull();
                                if (!variableUserID.HasValue)
                                    throw new Exception(LangUtility.Get("UserNotFound.Text", nameof(sysBpmsThreadTask)));

                                threadTask.UpdateAccessInfo(variableUserID, null);
                                break;
                        }
                        break;
                    case sysBpmsTask.e_OwnerTypeLU.Role:
                        switch ((UserTaskRuleModel.e_RoleAccessType)task.UserTaskRuleModel.AccessType)
                        {
                            case UserTaskRuleModel.e_RoleAccessType.Static:
                                //if this userTask must be seen by requester user, this add thread userId to new threadTask
                                if (task.IsInRole(null, (int)sysBpmsDepartmentMember.e_RoleLU.Requester))
                                    threadTask.UpdateAccessInfo(engineSharedModel.CurrentThread?.UserID, null);
                                else
                                    threadTask.UpdateAccessInfo(null, task.GetDepartmentRoles);
                                break;
                            case UserTaskRuleModel.e_RoleAccessType.Variable:
                                string roleName = new DataManageEngine(engineSharedModel, base.UnitOfWork).GetValueByBinding(task.UserTaskRuleModel.Variable).ToStringObj();
                                if (string.IsNullOrWhiteSpace(roleName))
                                    throw new Exception(LangUtility.Get("RoleNotFound.Text", nameof(sysBpmsThreadTask)));

                                task.SetRoleDepartment(roleName, task.UserTaskRuleModel.SpecificDepartmentId);
                                //if this userTask must be seen by requester user, this add thread userId to new threadTask
                                if (task.IsInRole(null, (int)sysBpmsDepartmentMember.e_RoleLU.Requester))
                                    threadTask.UpdateAccessInfo(engineSharedModel.CurrentThread?.UserID, null);
                                else
                                    threadTask.UpdateAccessInfo(null, task.GetDepartmentRoles);
                                break;
                            case UserTaskRuleModel.e_RoleAccessType.CorrespondentRole:
                                Guid? userId = task.UserTaskRuleModel.UserType == (int)UserTaskRuleModel.e_UserType.CurrentUserID ? new UserService(base.UnitOfWork).GetInfo(engineSharedModel.CurrentUserName ?? "")?.ID : engineSharedModel.CurrentThread?.UserID;
                                Guid? departmentId = new AccessCodeHelper(base.UnitOfWork).GetDepartmentHierarchyByUserId(userId.ToGuidObj(), task.UserTaskRuleModel.RoleCode.ToIntObj(), task.UserTaskRuleModel.GoUpDepartment);
                                if (!departmentId.HasValue)
                                    throw new Exception(LangUtility.Get("OrganizationNotFound.Text", nameof(sysBpmsThreadTask)));

                                threadTask.UpdateAccessInfo(null, new List<Tuple<Guid?, string>>() { new Tuple<Guid?, string>(departmentId, task.UserTaskRuleModel.RoleCode) });
                                break;
                        }
                        break;
                }
            }
            else
                throw new Exception(LangUtility.Get("NextNoAccessSetting.Text", nameof(sysBpmsThreadTask)));

        }

        public List<sysBpmsThreadTask> GetMultiInstanceTask(Domain.sysBpmsTask task, EngineSharedModel engineSharedModel)
        {
            sysBpmsThreadTask threadTask = null;
            List<sysBpmsThreadTask> list = new List<sysBpmsThreadTask>();
            switch ((sysBpmsTask.e_OwnerTypeLU)task.OwnerTypeLU)
            {
                case sysBpmsTask.e_OwnerTypeLU.User:
                    switch ((UserTaskRuleModel.e_UserAccessType)task.UserTaskRuleModel.AccessType)
                    {
                        case UserTaskRuleModel.e_UserAccessType.Static:
                        case UserTaskRuleModel.e_UserAccessType.Variable:
                            string userId = task.UserID;
                            if (task.UserTaskRuleModel.AccessType == (int)UserTaskRuleModel.e_UserAccessType.Variable)
                                userId = new DataManageEngine(engineSharedModel, base.UnitOfWork).GetValueByBinding(task.UserTaskRuleModel.Variable).ToStringObj();
                            int i = 0;
                            foreach (string item in userId.Split(','))
                            {
                                threadTask = new sysBpmsThreadTask();
                                threadTask.Update(engineSharedModel.CurrentThreadID.Value, task.ID, DateTime.Now.AddSeconds(i++), null, string.Empty, 0, (int)sysBpmsThreadTask.e_StatusLU.New);
                                threadTask.UpdateAccessInfo(item.ToGuidObj(), null);
                                list.Add(threadTask);
                            }
                            break;
                    }
                    break;
                case sysBpmsTask.e_OwnerTypeLU.Role:
                    switch ((UserTaskRuleModel.e_RoleAccessType)task.UserTaskRuleModel.AccessType)
                    {
                        case UserTaskRuleModel.e_RoleAccessType.Static:
                        case UserTaskRuleModel.e_RoleAccessType.Variable:
                            if (task.UserTaskRuleModel.AccessType == (int)UserTaskRuleModel.e_RoleAccessType.Variable)
                                task.SetRoleDepartment(new DataManageEngine(engineSharedModel, base.UnitOfWork).GetValueByBinding(task.UserTaskRuleModel.Variable).ToStringObj(), task.UserTaskRuleModel.SpecificDepartmentId);
                            foreach (var item in task.GetDepartmentRoles)
                            {
                                List<Guid> users = new DepartmentMemberService(base.UnitOfWork).GetList(item.Item1, item.Item2.ToIntObj(), null).Select(c => c.UserID).ToList();
                                //It is used  for sequential task.
                                int i = 0;
                                foreach (Guid uId in users)
                                {
                                    threadTask = new sysBpmsThreadTask();
                                    threadTask.Update(engineSharedModel.CurrentThreadID.Value, task.ID, DateTime.Now.AddSeconds(i++), null, string.Empty, 0, (int)sysBpmsThreadTask.e_StatusLU.New);
                                    threadTask.UpdateAccessInfo(uId, null);
                                    list.Add(threadTask);
                                }
                            }
                            break;
                        case UserTaskRuleModel.e_RoleAccessType.CorrespondentRole:

                            Guid? userId = task.UserTaskRuleModel.UserType == (int)UserTaskRuleModel.e_UserType.CurrentUserID ? new UserService(base.UnitOfWork).GetInfo(engineSharedModel.CurrentUserName ?? "")?.ID : engineSharedModel.CurrentThread?.UserID;
                            Guid? departmentId = new AccessCodeHelper(base.UnitOfWork).GetDepartmentHierarchyByUserId(userId.ToGuidObj(), task.UserTaskRuleModel.RoleCode.ToIntObj(), task.UserTaskRuleModel.GoUpDepartment);
                            if (!departmentId.HasValue)
                                throw new Exception(LangUtility.Get("OrganizationNotFound.Text", nameof(sysBpmsThreadTask)));
                            {
                                List<Guid> users = new DepartmentMemberService(base.UnitOfWork).GetList(departmentId, task.UserTaskRuleModel.RoleCode.ToIntObj(), null).Select(c => c.UserID).ToList();
                                //It is used  for sequential task.
                                int i = 0;
                                foreach (Guid uId in users)
                                {
                                    threadTask = new sysBpmsThreadTask();
                                    threadTask.Update(engineSharedModel.CurrentThreadID.Value, task.ID, DateTime.Now.AddSeconds(i++), null, string.Empty, 0, (int)sysBpmsThreadTask.e_StatusLU.New);
                                    threadTask.UpdateAccessInfo(uId, null);
                                    list.Add(threadTask);
                                }
                            }
                            break;
                    }

                    break;

            }
            return list;
        }

        /// <summary>
        /// this method check whether current user has access to this task or not.
        /// </summary>
        /// <param name="userId">set to currnet User ID if user has logined otherwise set to null</param>
        /// <returns></returns>
        public (bool, string) CheckAccess(Guid threadTaskID, Guid? userId, Guid processId, bool isPost, bool accessByTracking = false)
        {
            sysBpmsThreadTask threadTask = this.GetInfo(threadTaskID);
            if (isPost && !new ProcessService(base.UnitOfWork).GetInfo(processId).AllowNextFlow())
                return (false, LangUtility.Get("NotAllowNextFlow.Text", nameof(sysBpmsThreadTask)));

            if (threadTask.StatusLU == (int)sysBpmsThreadTask.e_StatusLU.New ||
                threadTask.StatusLU == (int)sysBpmsThreadTask.e_StatusLU.Ongoing)
            {
                if (threadTask.OwnerUserID.HasValue && threadTask.OwnerUserID == userId)
                    return (true, "");

                if (userId.HasValue)
                {
                    List<sysBpmsDepartmentMember> listDepartmentMember = new DepartmentMemberService(base.UnitOfWork).GetList(null, null, userId.Value);
                    if (listDepartmentMember.Any(c => threadTask.GetDepartmentRoles.Any(d => (!d.Item1.HasValue || d.Item1 == c.DepartmentID) && d.Item2.ToIntObj() == c.RoleLU)))
                        return (true, "");
                }
                else
                {
                    if (!threadTask.sysBpmsThread.UserID.HasValue && !threadTask.OwnerUserID.HasValue && string.IsNullOrWhiteSpace(threadTask.OwnerRole))
                        return (true, "");
                }
                if (threadTask.sysBpmsTask.UserTaskRuleModel?.AccessType != (int)UserTaskRuleModel.e_UserAccessType.Static &&
                    threadTask.sysBpmsTask.UserTaskRuleModel?.AccessType != (int)UserTaskRuleModel.e_UserAccessType.Variable)
                    return (true, "");

                if (accessByTracking && threadTask.sysBpmsTask.IsInRole(null, (int)sysBpmsDepartmentMember.e_RoleLU.Requester))
                    return (true, "");
            }
            return (false, LangUtility.Get("AccessError.Text", nameof(sysBpmsThreadTask)));
        }

        public ResultOperation Delete(Guid ID)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IThreadTaskRepository>().Delete(ID);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }
    }
}
