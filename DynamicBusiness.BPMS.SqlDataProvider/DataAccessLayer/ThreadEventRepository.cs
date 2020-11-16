﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;
using System.Data.Entity.SqlServer;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ThreadEventRepository : IThreadEventRepository
    {
        private Db_BPMSEntities Context;
        public ThreadEventRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsThreadEvent threadEvent)
        {
            threadEvent.ID = Guid.NewGuid();
            this.Context.sysBpmsThreadEvents.Add(threadEvent);
        }

        public void Update(sysBpmsThreadEvent threadEvent)
        {
            sysBpmsThreadEvent retVal = (from p in this.Context.sysBpmsThreadEvents
                                         where p.ID == threadEvent.ID
                                         select p).FirstOrDefault();
            retVal.Load(threadEvent);
        }

        public void Delete(Guid threadEventId)
        {
            sysBpmsThreadEvent threadEvent = this.Context.sysBpmsThreadEvents.FirstOrDefault(d => d.ID == threadEventId);
            if (threadEvent != null)
            {
                this.Context.sysBpmsThreadEvents.Remove(threadEvent);
            }
        }

        public sysBpmsThreadEvent GetInfo(Guid ID, string[] Includes)
        {
            return (from P in this.Context.sysBpmsThreadEvents
                    where P.ID == ID
                    select P).AsNoTracking().Include(Includes).FirstOrDefault();
        }

        public List<sysBpmsThreadEvent> GetTimerActive(string[] Includes)
        {
            DateTime dateTimeFrom = DateTime.Now;
            return (from P in this.Context.sysBpmsThreadEvents
                    where
                    ((P.sysBpmsEvent.TypeLU == ((int)sysBpmsEvent.e_TypeLU.StartEvent) && P.sysBpmsEvent.SubType == (int)WorkflowStartEvent.BPMNStartEventType.Timer) ||
                    (P.sysBpmsEvent.TypeLU == ((int)sysBpmsEvent.e_TypeLU.boundary) && P.sysBpmsEvent.SubType == (int)WorkflowBoundaryEvent.BPMNBoundaryType.timer) ||
                    (P.sysBpmsEvent.TypeLU == ((int)sysBpmsEvent.e_TypeLU.IntermediateCatch) && P.sysBpmsEvent.SubType == (int)WorkflowIntermediateCatchEvent.BPMNIntermediateCatchType.Timer)) &&
                    (P.ExecuteDate <= dateTimeFrom) &&
                    (P.StatusLU == (int)sysBpmsThreadEvent.e_StatusLU.InProgress)
                    select P).Include(Includes).OrderBy(c => c.StartDate).AsNoTracking().ToList();
        }

        public List<sysBpmsThreadEvent> GetMessageActive(Guid notProcessID, Guid messageTypeID, string[] Includes)
        {
            return (from P in this.Context.sysBpmsThreadEvents
                    where
                    (P.sysBpmsEvent.ProcessID != notProcessID) &&
                    ((P.sysBpmsEvent.TypeLU == ((int)sysBpmsEvent.e_TypeLU.boundary) && P.sysBpmsEvent.SubType == (int)WorkflowBoundaryEvent.BPMNBoundaryType.Message) ||
                    (P.sysBpmsEvent.TypeLU == ((int)sysBpmsEvent.e_TypeLU.IntermediateCatch) && P.sysBpmsEvent.SubType == (int)WorkflowIntermediateCatchEvent.BPMNIntermediateCatchType.Message)) &&
                    (P.StatusLU == (int)sysBpmsThreadEvent.e_StatusLU.InProgress)
                    select P).Include(Includes).OrderBy(c => c.StartDate).AsNoTracking().ToList();
        }
        public List<sysBpmsThreadEvent> GetActive(Guid threadId)
        {
            return (from P in this.Context.sysBpmsThreadEvents
                    where
                    (P.ThreadID == threadId) &&
                    (P.StatusLU == (int)sysBpmsThreadEvent.e_StatusLU.InProgress)
                    select P).OrderBy(c => c.StartDate).AsNoTracking().ToList();
        }

        public List<sysBpmsThreadEvent> GetList(Guid threadId)
        {
            return (from P in this.Context.sysBpmsThreadEvents
                    where
                    (P.ThreadID == threadId)
                    select P).AsNoTracking().ToList();
        }

        public int GetCount(Guid? threadId, Guid eventID, Guid? ProcessID, int? statusLU, string[] Includes)
        {
            return (from P in this.Context.sysBpmsThreadEvents
                    where
                    (!threadId.HasValue || P.ThreadID == threadId) &&
                    (!statusLU.HasValue || P.StatusLU == statusLU) &&
                    (P.EventID == eventID)
                    select P).Count();
        }

        public sysBpmsThreadEvent GetLastExecuted(Guid? threadId, Guid? processID, Guid? eventID, string[] Includes)
        {
            return (from P in this.Context.sysBpmsThreadEvents
                    join T in this.Context.sysBpmsThreads on P.ThreadID equals T.ID
                    where
                    (!threadId.HasValue || P.ThreadID == threadId) &&
                    (!processID.HasValue || T.ProcessID == processID) &&
                    (P.StatusLU == (int)sysBpmsThreadEvent.e_StatusLU.Done) &&
                    (!eventID.HasValue || P.EventID == eventID)
                    select P).Include(Includes).OrderByDescending(c => c.ExecuteDate).AsNoTracking().FirstOrDefault();
        }

    }
}
