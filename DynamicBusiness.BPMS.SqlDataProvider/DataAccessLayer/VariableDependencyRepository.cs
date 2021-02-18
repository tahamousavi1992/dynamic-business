using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class VariableDependencyRepository : IVariableDependencyRepository
    {
        private Db_BPMSEntities Context;
        public VariableDependencyRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsVariableDependency variableDependency)
        {
            variableDependency.ID = Guid.NewGuid();
            this.Context.sysBpmsVariableDependencies.Add(variableDependency);
        }

        public void Update(sysBpmsVariableDependency variableDependency)
        {
            this.Context.Entry(variableDependency).State = EntityState.Modified;
        }

        public void Delete(Guid VariableDependencyId)
        {
            sysBpmsVariableDependency variableDependency = this.Context.sysBpmsVariableDependencies.FirstOrDefault(d => d.ID == VariableDependencyId);
            if (variableDependency != null)
            {
                this.Context.sysBpmsVariableDependencies.Remove(variableDependency);
            }
        }

        public sysBpmsVariableDependency GetInfo(Guid ID)
        {
            return (from P in this.Context.sysBpmsVariableDependencies
                    where P.ID == ID
                    select P).AsNoTracking().FirstOrDefault(); ;
        }

        public List<sysBpmsVariableDependency> GetList(Guid? DependentVariableID, Guid? ToVariableID)
        {
            return this.Context.sysBpmsVariableDependencies.Where(d =>
             (!DependentVariableID.HasValue || d.DependentVariableID == DependentVariableID) &&
             (!ToVariableID.HasValue || d.ToVariableID == ToVariableID)).AsNoTracking().ToList();
        }

        public List<sysBpmsVariableDependency> GetList(Guid processId)
        {
            return this.Context.sysBpmsVariableDependencies.Where(d => d.DependentVariable.ProcessID == processId).AsNoTracking().ToList();
        }

    }
}
