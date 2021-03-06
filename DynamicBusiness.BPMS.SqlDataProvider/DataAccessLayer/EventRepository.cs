﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class EventRepository : IEventRepository
    {
        private Db_BPMSEntities Context;
        public EventRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsEvent _event)
        {
            this.Context.sysBpmsEvents.Add(_event);
        }

        public void Update(sysBpmsEvent _event)
        {
            //To fix 'Attaching an entity failed' error.
            var local = this.Context.Set<sysBpmsEvent>().Local.FirstOrDefault(f => f.ID == _event.ID);
            if (local != null)
            {
                this.Context.Entry(local).State = EntityState.Detached;
                local = null;
            }
            this.Context.Entry(_event.Clone()).State = EntityState.Modified;
        }

        public void Delete(Guid id)
        {
            sysBpmsEvent _event = this.Context.sysBpmsEvents.FirstOrDefault(d => d.ID == id);
            if (_event != null)
            {
                this.Context.sysBpmsEvents.Remove(_event);
            }
        }

        public sysBpmsEvent GetInfo(Guid id)
        {
            return (from P in this.Context.sysBpmsEvents
                    where P.ID == id
                    select P).AsNoTracking().FirstOrDefault();
        }

        public sysBpmsEvent GetInfo(string elementId, Guid processId)
        {
            return (from P in this.Context.sysBpmsEvents
                    where P.ElementID == elementId && P.ProcessID == processId
                    select P).AsNoTracking().FirstOrDefault();
        }

        public List<sysBpmsEvent> GetList(int? TypeLU, Guid? ProcessID, string RefElementID, int? SubType, string[] Includes)
        {
            return this.Context.sysBpmsEvents.Include(c => c.Element).Where(d =>
             (RefElementID == string.Empty || d.RefElementID == RefElementID) &&
             (!TypeLU.HasValue || d.TypeLU == TypeLU) &&
             (!SubType.HasValue || d.SubType == SubType) &&
             (!ProcessID.HasValue || d.Element.ProcessID == ProcessID)).OrderBy(c => c.TypeLU).Include(Includes).AsNoTracking().ToList();
        }

        public List<sysBpmsEvent> GetList(int? TypeLU, Guid? ProcessID, string RefElementID, int? SubType, int? ProcessStatusLU, string[] Includes)
        {
            return this.Context.sysBpmsEvents.Include(c => c.Element).Where(d =>
             (!TypeLU.HasValue || d.TypeLU == TypeLU) &&
             (!ProcessID.HasValue || d.Element.ProcessID == ProcessID) &&
             (RefElementID == string.Empty || d.RefElementID == RefElementID) &&
             (!SubType.HasValue || d.SubType == SubType) &&
             (!ProcessStatusLU.HasValue || d.Element.Process.StatusLU == ProcessStatusLU)).OrderBy(c => c.TypeLU).Include(Includes).AsNoTracking().ToList();
        }
        public List<sysBpmsEvent> GetListStartMessage(Guid? notProcessID, string key, Guid messageTypeID, string[] Includes)
        {
            var list = this.Context.sysBpmsEvents.Include(c => c.Element).Where(d =>
              (!notProcessID.HasValue || d.ProcessID != notProcessID) &&
              (d.MessageTypeID == messageTypeID) &&
              (d.TypeLU == (int)sysBpmsEvent.e_TypeLU.StartEvent) &&
              (d.SubType == (int)WorkflowStartEvent.BPMNStartEventType.Message) &&
              (d.Process.StatusLU == (int)sysBpmsProcess.Enum_StatusLU.Published ||
               d.Process.StatusLU == (int)sysBpmsProcess.Enum_StatusLU.OldVersion)).OrderBy(c => c.TypeLU).Include(Includes).AsNoTracking().ToList();
            //Start events can only have static key value.
            return list.Where(c => c.SubTypeMessageEventModel.Key == key).ToList();
        }
    }
}
