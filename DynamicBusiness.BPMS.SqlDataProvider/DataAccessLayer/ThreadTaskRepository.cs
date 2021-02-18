using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;
using System.Data.Entity.SqlServer;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ThreadTaskRepository : IThreadTaskRepository
    {
        private Db_BPMSEntities Context;
        public ThreadTaskRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsThreadTask ThreadTask)
        {
            ThreadTask.ID = Guid.NewGuid();
            this.Context.sysBpmsThreadTasks.Add(ThreadTask);
        }

        public void Update(sysBpmsThreadTask ThreadTask)
        {
            this.Context.Entry(ThreadTask).State = EntityState.Modified;
        }

        public sysBpmsThreadTask GetInfo(Guid ID, string[] Includes)
        {
            return (from P in this.Context.sysBpmsThreadTasks
                    where P.ID == ID
                    select P).AsNoTracking().Include(Includes).FirstOrDefault();
        }

        public bool HasAny(Guid processId, Guid taskId)
        {
            return (from P in this.Context.sysBpmsThreadTasks
                    where P.Thread.ProcessID == processId && P.TaskID == taskId
                    select P).Count() > 0;
        }

        public void Delete(Guid ID)
        {
            sysBpmsThreadTask sysBpmsThreadTask = this.Context.sysBpmsThreadTasks.FirstOrDefault(d => d.ID == ID);
            if (sysBpmsThreadTask != null)
            {
                this.Context.sysBpmsThreadTasks.Remove(sysBpmsThreadTask);
            }
        }

        public List<sysBpmsThreadTask> GetList(Guid ThreadID, int? TaskType, Guid? taskId, int? statusLU, string[] Includes)
        {
            return (from P in this.Context.sysBpmsThreadTasks
                    where P.ThreadID == ThreadID &&
                    (!TaskType.HasValue || P.Task.TypeLU == TaskType) &&
                    (!statusLU.HasValue || P.StatusLU == statusLU) &&
                    (!taskId.HasValue || P.TaskID == taskId)
                    select P).Include(Includes).OrderByDescending(c => c.StartDate).AsNoTracking().ToList();
        }

        public List<sysBpmsThreadTask> GetListRunning(Guid ThreadID)
        {
            return (from P in this.Context.sysBpmsThreadTasks
                    where P.ThreadID == ThreadID &&
                    (P.StatusLU == (int)sysBpmsThreadTask.e_StatusLU.New ||
                    P.StatusLU == (int)sysBpmsThreadTask.e_StatusLU.Ongoing)
                    select P).Include(c => c.Task).AsNoTracking().ToList();
        }

        public List<sysBpmsThreadTask> GetListKartable(Guid UserID, int[] statusLU, PagingProperties currentPaging)
        {
            List<sysBpmsThreadTask> list = new List<sysBpmsThreadTask>();
            statusLU = statusLU ?? new int[] { };
            if (UserID != Guid.Empty)
            {
                var query = (from P in this.Context.sysBpmsThreadTasks
                             join T in this.Context.sysBpmsTasks on P.TaskID equals T.ID
                             join D in this.Context.sysBpmsDepartmentMembers on UserID equals D.UserID into Dlist
                             where
                             (!statusLU.Any() || statusLU.Contains(P.StatusLU)) &&
                             (P.OwnerUserID.HasValue || (P.OwnerRole != null && P.OwnerRole != string.Empty)) &&
                             (!P.OwnerUserID.HasValue || UserID == P.OwnerUserID) &&
                             (P.OwnerRole == string.Empty || Dlist.Count(c => this.Context.sysBpmsSplit(P.OwnerRole, ",").Any(f => f.Data == ("0:" + c.RoleLU.ToString().Trim()) || f.Data == (c.DepartmentID.ToString() + ":" + c.RoleLU.ToString().Trim()))) > 0) &&
                             (!T.MarkerTypeLU.HasValue || T.MarkerTypeLU == (int)Domain.sysBpmsTask.e_MarkerTypeLU.NonSequential ||
                             T.MarkerTypeLU == (int)Domain.sysBpmsTask.e_MarkerTypeLU.Loop
                             || (T.MarkerTypeLU == (int)Domain.sysBpmsTask.e_MarkerTypeLU.Sequential && this.Context.sysBpmsThreadTasks.Count(c => c.ThreadID == P.ThreadID && c.TaskID == P.TaskID && c.StartDate < P.StartDate && c.StatusLU != (int)sysBpmsThreadTask.e_StatusLU.Done) == 0))
                             select P).Include(c => c.Task.Element).Include(c => c.Thread.Process).AsNoTracking();


                if (currentPaging != null)
                {
                    currentPaging.RowsCount = query.Count();
                    if (Math.Ceiling(Convert.ToDecimal(currentPaging.RowsCount) / Convert.ToDecimal(currentPaging.PageSize)) < currentPaging.PageIndex)
                        currentPaging.PageIndex = 1;

                    list = query.OrderByDescending(p => p.StartDate).Skip((currentPaging.PageIndex - 1) * currentPaging.PageSize).Take(currentPaging.PageSize).ToList();
                }
                else list = query.ToList();
            }
            return list;
        }

    }
}
