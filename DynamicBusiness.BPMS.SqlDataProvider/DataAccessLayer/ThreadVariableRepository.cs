using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ThreadVariableRepository : IThreadVariableRepository
    {
        private Db_BPMSEntities Context;
        public ThreadVariableRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }
        public void Add(sysBpmsThreadVariable ThreadVariable)
        {
            ThreadVariable.ID = Guid.NewGuid();
            this.Context.sysBpmsThreadVariables.Add(ThreadVariable);
        }

        public void Update(sysBpmsThreadVariable ThreadVariable)
        {
            sysBpmsThreadVariable retVal = (from p in this.Context.sysBpmsThreadVariables
                                        where p.ID == ThreadVariable.ID
                                     select p).FirstOrDefault();
            retVal.Load(ThreadVariable);
        }

        public void Delete(Guid ID)
        {
            sysBpmsThreadVariable ThreadVariable = this.Context.sysBpmsThreadVariables.FirstOrDefault(d => d.ID == ID);
            if (ThreadVariable != null)
            {
                this.Context.sysBpmsThreadVariables.Remove(ThreadVariable);
                this.Context.SaveChanges();
            }
        }

        public sysBpmsThreadVariable GetInfo(Guid ID)
        {
            return (from P in this.Context.sysBpmsThreadVariables
                    where P.ID == ID
                    select P).AsNoTracking().FirstOrDefault();
        }

        public sysBpmsThreadVariable GetInfo(Guid ThreadID, Guid VariableID)
        {
            return (from P in this.Context.sysBpmsThreadVariables
                    where P.ThreadID == ThreadID && P.VariableID == VariableID
                    select P).AsNoTracking().FirstOrDefault();
        }

        public List<sysBpmsThreadVariable> GetList(Guid? ThreadID, Guid? VariableID, string Value)
        {
            Value = Value ?? string.Empty;

            return this.Context.sysBpmsThreadVariables.Include(c => c.sysBpmsVariable).Where(d =>
              (!ThreadID.HasValue || d.ThreadID == ThreadID) &&
              (!VariableID.HasValue || d.VariableID == VariableID) &&
              (Value == string.Empty || d.Value == Value)).AsNoTracking().ToList();
        }
    }
}
