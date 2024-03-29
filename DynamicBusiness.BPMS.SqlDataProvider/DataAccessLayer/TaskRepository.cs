﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class TaskRepository : ITaskRepository
    {
        private Db_BPMSEntities Context;
        public TaskRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(Domain.sysBpmsTask Task)
        {
            this.Context.sysBpmsTasks.Add(Task);
        }

        public void Update(Domain.sysBpmsTask task)
        { 
            //To fix 'Attaching an entity failed' error.
            var local = this.Context.Set<sysBpmsTask>().Local.FirstOrDefault(f => f.ID == task.ID);
            if (local != null)
            {
                this.Context.Entry(local).State = EntityState.Detached;
                local = null;
            }
            this.Context.Entry(task).State = EntityState.Modified;
        }

        public void Delete(Guid id)
        {
            Domain.sysBpmsTask Task = this.Context.sysBpmsTasks.FirstOrDefault(d => d.ID == id);
            if (Task != null)
            {
                this.Context.sysBpmsTasks.Remove(Task);
            }
        }

        public Domain.sysBpmsTask GetInfo(string elementId, Guid processId)
        {
            return (from P in this.Context.sysBpmsTasks
                    where P.ElementID == elementId && P.ProcessID == processId
                    select P).AsNoTracking().FirstOrDefault();
        }

        public Domain.sysBpmsTask GetInfo(Guid id)
        {
            return (from P in this.Context.sysBpmsTasks
                    where P.ID == id
                    select P).AsNoTracking().FirstOrDefault();
        }

        public List<Domain.sysBpmsTask> GetList(int? typeLU, Guid? processID)
        {

            return this.Context.sysBpmsTasks.Include(c => c.Element).Where(d =>
                 (!typeLU.HasValue || d.TypeLU == typeLU) &&
                 (!processID.HasValue || d.ProcessID == processID)).AsNoTracking().ToList();
        }

        public List<Domain.sysBpmsTask> GetListBeginTasks(Guid processID)
        { 
            List<sysBpmsTask> items = (from T in this.Context.sysBpmsTasks
                                       join P in this.Context.sysBpmsProcesses on T.ProcessID equals P.ID 
                                       where
                                       T.ProcessID == processID &&
                                       (this.Context.sysBpmsSplit(P.BeginTasks, ",").Any(c => c.Data == T.ElementID))
                                       select T).OrderBy(d => d.ID).Include(c => c.Element).AsNoTracking().ToList();
            return items;
        }
    }
}
