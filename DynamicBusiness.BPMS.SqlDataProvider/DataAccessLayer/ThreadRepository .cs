using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ThreadRepository : IThreadRepository
    {
        private Db_BPMSEntities Context;
        public ThreadRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsThread thread)
        {
            thread.ID = Guid.NewGuid();
            this.Context.sysBpmsThreads.Add(thread);
        }

        public void Update(sysBpmsThread thread)
        {
            //To fix 'Attaching an entity failed' error.
            var local = this.Context.Set<sysBpmsThread>().Local.FirstOrDefault(f => f.ID == thread.ID);
            if (local != null)
            {
                this.Context.Entry(local).State = EntityState.Detached;
                local = null;
            }
            this.Context.Entry(thread.Clone()).State = EntityState.Modified;
        }

        public sysBpmsThread GetInfo(Guid ID, string[] includes)
        {
            sysBpmsThread retVal = null;
            retVal = (from P in this.Context.sysBpmsThreads
                      where P.ID == ID
                      select P).Include(includes).AsNoTracking().FirstOrDefault();
            return retVal;
        }

        public int GetCountActive(Guid UserId, Guid? ProcessID)
        {

            return (from P in this.Context.sysBpmsThreads
                    where (P.UserID == UserId) &&
                          (!ProcessID.HasValue || P.ProcessID == ProcessID) &&
                          (P.StatusLU == (int)sysBpmsThread.Enum_StatusLU.Draft || P.StatusLU == (int)sysBpmsThread.Enum_StatusLU.InProgress)
                    select P).Count();
        }

        public int GetCount(Guid processID)
        {

            return (from P in this.Context.sysBpmsThreads
                    where (P.ProcessID == processID)
                    select P).Count();
        }

        public void Delete(Guid ID)
        {
            sysBpmsThread sysBpmsThread1 = this.Context.sysBpmsThreads.FirstOrDefault(d => d.ID == ID);
            if (sysBpmsThread1 != null)
            {
                this.Context.sysBpmsThreads.Remove(sysBpmsThread1);
            }
        }


        public List<sysBpmsThread> GetArchiveList(Guid? TaskOwnerUserID, Guid? ProcessID, int[] statusLU, Guid? UserID, DateTime? StartFrom, DateTime? StartTo, DateTime? EndFrom, DateTime? EndTo, PagingProperties currentPaging, string[] includes)
        {
            List<sysBpmsThread> list = null;
            bool allStatus = statusLU == null;
            statusLU = statusLU ?? new int[] { };

            var query = (from P in this.Context.sysBpmsThreads
                         where (!UserID.HasValue || P.UserID == UserID) &&
                               (!ProcessID.HasValue || P.ProcessID == ProcessID) &&
                               (allStatus || statusLU.Contains(P.StatusLU)) &&
                               (!TaskOwnerUserID.HasValue || P.ThreadTasks.Count(c => c.OwnerUserID == TaskOwnerUserID) > 0) &&
                               (!StartFrom.HasValue || DbFunctions.TruncateTime(P.StartDate) >= StartFrom) &&
                               (!StartTo.HasValue || DbFunctions.TruncateTime(P.StartDate) <= StartTo) &&
                               (!EndFrom.HasValue || DbFunctions.TruncateTime(P.EndDate) >= EndFrom) &&
                               (!EndTo.HasValue || DbFunctions.TruncateTime(P.EndDate) <= EndTo)
                         select P).Include(includes).AsNoTracking();

            if (currentPaging != null)
            {
                currentPaging.RowsCount = query.Count();
                if (Math.Ceiling(Convert.ToDecimal(currentPaging.RowsCount) / Convert.ToDecimal(currentPaging.PageSize)) < currentPaging.PageIndex)
                    currentPaging.PageIndex = 1;

                list = query.OrderByDescending(p => p.Number).Skip((currentPaging.PageIndex - 1) * currentPaging.PageSize).Take(currentPaging.PageSize).ToList();
            }
            else list = query.ToList();

            return list;
        }

        public int MaxNumber()
        {
            return ((from p in this.Context.sysBpmsThreads
                     select (int?)p.Number).Max(c => c == null ? 0 : c)).ToIntObj();
        }

    }
}
