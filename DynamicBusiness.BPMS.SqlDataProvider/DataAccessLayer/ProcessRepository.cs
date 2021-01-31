using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ProcessRepository : IProcessRepository
    {
        private Db_BPMSEntities Context;

        public ProcessRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        #region .:: public ::.

        public void Add(sysBpmsProcess Process)
        {
            Process.ID = Guid.NewGuid();
            this.Context.sysBpmsProcesses.Add(Process);
        }

        public void Update(sysBpmsProcess Process)
        {
            sysBpmsProcess retVal = (from p in this.Context.sysBpmsProcesses
                                 where p.ID == Process.ID
                                 select p).FirstOrDefault();
            retVal.Load(Process);
        }

        public void Delete(Guid ID)
        {
            sysBpmsProcess process = this.Context.sysBpmsProcesses.FirstOrDefault(d => d.ID == ID);
            if (process != null)
            {
                this.Context.sysBpmsProcesses.Remove(process);
            }
        }
         
        public sysBpmsProcess GetInfo(Guid ID)
        {
            return (from P in this.Context.sysBpmsProcesses
                    where P.ID == ID
                    select P).AsNoTracking().FirstOrDefault();
        }

        public List<sysBpmsProcess> GetList(DateTime? StartDate, DateTime? EndDate, Guid? processGroupId, PagingProperties currentPaging)
        {
            List<sysBpmsProcess> retVal = null;
            var tempRetVal = (from P in this.Context.sysBpmsProcesses
                              where
                              (!processGroupId.HasValue || processGroupId == P.ProcessGroupID) &&
                              (!StartDate.HasValue || StartDate < P.CreateDate) &&
                              (!EndDate.HasValue || P.CreateDate < EndDate)
                              select P).AsNoTracking();

            if (currentPaging != null)
            {
                currentPaging.RowsCount = tempRetVal.Count();
                if (Math.Ceiling(Convert.ToDecimal(currentPaging.RowsCount) / Convert.ToDecimal(currentPaging.PageSize)) < currentPaging.PageIndex)
                    currentPaging.PageIndex = 1;

                retVal = tempRetVal.OrderByDescending(p => p.CreateDate).Skip((currentPaging.PageIndex - 1) * currentPaging.PageSize).Take(currentPaging.PageSize).AsNoTracking().ToList();
            }
            else
            {
                retVal = tempRetVal.OrderByDescending(p => p.CreateDate).AsNoTracking().ToList();
            }
            return retVal;
        }
        public List<sysBpmsProcess> GetList(int? statusLU, Guid? parentProcessId)
        {
            var tempRetVal = (from P in this.Context.sysBpmsProcesses
                              where
                              (!statusLU.HasValue || P.StatusLU == statusLU) &&
                              (!parentProcessId.HasValue || P.ParentProcessID == parentProcessId)
                              select P).AsNoTracking();

            return tempRetVal.OrderByDescending(p => p.CreateDate).AsNoTracking().ToList();
        }
        public sysBpmsProcess GetLastActive(Guid parentProcessId)
        {
            var tempRetVal = (from P in this.Context.sysBpmsProcesses
                              where
                              (P.StatusLU == (int)sysBpmsProcess.Enum_StatusLU.Published || P.StatusLU == (int)sysBpmsProcess.Enum_StatusLU.Inactive) &&
                              (P.ID == parentProcessId || P.ParentProcessID == parentProcessId)
                              select P).AsNoTracking();

            return tempRetVal.OrderByDescending(p => p.CreateDate).AsNoTracking().FirstOrDefault();
        }
        public List<string> GetListBeginTaskElementID(Guid ID)
        {
            List<sysBpmsTask> list = new List<sysBpmsTask>();
            List<sysBpmsEvent> startEvents = (from P in this.Context.sysBpmsEvents
                                          where P.Element.ProcessID == ID && P.TypeLU == (int)sysBpmsEvent.e_TypeLU.StartEvent
                                          select P).AsNoTracking().ToList();
            foreach (sysBpmsEvent startEvent in startEvents)
            {
                this.GetRecursiveBeginElements(startEvent.ElementID, startEvent.ProcessID, list);
            }
            return list.Select(c => c.ElementID).ToList();
        }

        public List<sysBpmsTask> GetAvailableProccess(Guid UserID)
        {
            string staticAccessType = $"<AccessType>{(int)UserTaskRuleModel.e_UserAccessType.Static}</AccessType>";
            string userIdString = UserID.ToString();
            List<sysBpmsTask> items = (from T in this.Context.sysBpmsTasks
                                   join P in this.Context.sysBpmsProcesses on T.ProcessID equals P.ID
                                   join D in this.Context.sysBpmsDepartmentMembers on UserID equals D.UserID into Dlist
                                   join E in this.Context.sysBpmsEvents on P.ID equals E.ProcessID into Elist
                                   where
                                   (P.TypeLU == (int)sysBpmsProcess.e_TypeLU.General) &&
                                   (P.StatusLU == (int)sysBpmsProcess.Enum_StatusLU.Published) &&
                                   (Elist.Count(c => c.TypeLU == (int)sysBpmsEvent.e_TypeLU.StartEvent && c.SubType == (int)WorkflowStartEvent.BPMNStartEventType.None) > 0) &&
                                   (this.Context.sysBpmsSplit(P.BeginTasks, ",").Any(c => c.Data == T.ElementID)) &&
                                   (
                                   (T.OwnerTypeLU == (int)Domain.sysBpmsTask.e_OwnerTypeLU.User && T.UserID.Contains(userIdString)) ||
                                   (T.OwnerTypeLU == (int)Domain.sysBpmsTask.e_OwnerTypeLU.Role &&
                                   (T.RoleName == string.Empty
                                   || T.RoleName == (",0:" + (int)sysBpmsDepartmentMember.e_RoleLU.Requester + ",")//it means that everyone can start this proccess.
                                   || Dlist.Count(c => this.Context.sysBpmsSplit(T.RoleName, ",").Any(f => f.Data == ("0:" + c.RoleLU.ToString().Trim()) || f.Data == (c.DepartmentID.ToString() + ":" + c.RoleLU.ToString().Trim()))) > 0)
                                   ) ||
                                   (!T.Rule.Contains(staticAccessType)))//it will be calculated in engine.
                                   select T).OrderBy(d => d.Element.Process.Name).Include(c => c.Element.Process).AsNoTracking().ToList();
            return items;
        }

        public int MaxNumber()
        {
            return ((from p in this.Context.sysBpmsProcesses
                     select (int?)p.Number).Max(c => c == null ? 0 : c)).ToIntObj();
        }

        #endregion

        #region .:: Private Methods ::.

        private void GetRecursiveBeginElements(string elementID, Guid processId, List<sysBpmsTask> list)
        {
            foreach (sysBpmsSequenceFlow item in this.Context.sysBpmsSequenceFlows.Where(c => c.SourceElementID == elementID).AsNoTracking().ToList())
            {
                sysBpmsElement element = this.Context.sysBpmsElements.AsNoTracking().FirstOrDefault(c => c.ProcessID == processId && c.ID == item.TargetElementID);
                this.EvaluateNextRecursiveItem(element, list);
            }
        }

        private void EvaluateNextRecursiveItem(sysBpmsElement element, List<sysBpmsTask> list)
        {
            switch ((sysBpmsElement.e_TypeLU)element.TypeLU)
            {
                case sysBpmsElement.e_TypeLU.Event:
                    this.GetRecursiveBeginElements(element.ID, element.ProcessID, list);
                    break;
                case sysBpmsElement.e_TypeLU.Gateway:
                    this.GetRecursiveBeginElements(element.ID, element.ProcessID, list);
                    break;
                case sysBpmsElement.e_TypeLU.Task:
                    sysBpmsTask task = this.Context.sysBpmsTasks.AsNoTracking().FirstOrDefault(c => c.ProcessID == element.ProcessID && c.ElementID == element.ID);
                    switch ((sysBpmsTask.e_TypeLU)task.TypeLU)
                    {
                        case sysBpmsTask.e_TypeLU.ServiceTask:
                            this.GetRecursiveBeginElements(task.ElementID, element.ProcessID, list);
                            break;
                        case sysBpmsTask.e_TypeLU.UserTask:
                            list.Add(task);
                            break;
                    }
                    break;
            }
        }

        #endregion
    }
}
