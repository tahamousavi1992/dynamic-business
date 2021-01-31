using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using System.Data.Entity;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class VariableRepository : IVariableRepository
    {
        private Db_BPMSEntities Context;
        public VariableRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsVariable variable)
        {
            variable.ID = Guid.NewGuid();
            this.Context.sysBpmsVariables.Add(variable);
        }

        public void Update(sysBpmsVariable Variable)
        {
            sysBpmsVariable retVal = (from p in this.Context.sysBpmsVariables
                                  where p.ID == Variable.ID
                                  select p).FirstOrDefault();
            retVal.Load(Variable);
        }

        public void Delete(Guid VariableId)
        {
            sysBpmsVariable Variable = this.Context.sysBpmsVariables.FirstOrDefault(d => d.ID == VariableId);
            if (Variable != null)
            {
                this.Context.sysBpmsVariables.Remove(Variable);
            }
        }
        /// <summary>
        /// Include(c => c.VariableDependencies)
        /// </summary>
        public sysBpmsVariable GetInfo(Guid ID, string[] includes)
        {
            return (from P in this.Context.sysBpmsVariables
                    where P.ID == ID
                    select P).Include(includes).AsNoTracking().FirstOrDefault(); ;
        }
        /// <summary>
        /// Include(c => c.VariableDependencies)
        /// </summary>
        public sysBpmsVariable GetInfo(Guid? ProcessID, Guid? ApplicationPageID, string Name, string[] includes)
        {
            return (from P in this.Context.sysBpmsVariables
                    where
                    (!ProcessID.HasValue || P.ProcessID == ProcessID) &&
                    (!ApplicationPageID.HasValue || P.ApplicationPageID == ApplicationPageID) &&
                     P.Name == Name
                    select P).OrderBy(c => c.Name).Include(includes).AsNoTracking().FirstOrDefault();
        }
        public List<sysBpmsVariable> GetList(Guid dbConnectionID)
        {
            return this.Context.sysBpmsVariables.Where(d =>
            (d.DBConnectionID == dbConnectionID)).OrderBy(c => c.Name).AsNoTracking().ToList();
        }
        public List<sysBpmsVariable> GetList(Guid? ProcessID, Guid? ApplicationPageID, int? VarTypeLU, string Name, Guid? EntityDefID, bool? EntityIsActive, string[] includes)
        {
            Name = Name.ToStringObj().ToLower();

            return this.Context.sysBpmsVariables.Where(d =>
            (!ProcessID.HasValue || d.ProcessID == ProcessID) &&
            (!ApplicationPageID.HasValue || d.ApplicationPageID == ApplicationPageID) &&
            (!VarTypeLU.HasValue || d.VarTypeLU == VarTypeLU) &&
            (Name == string.Empty || d.Name.ToLower().Contains(Name)) &&
            (!EntityIsActive.HasValue || !d.EntityDefID.HasValue || d.EntityDef.IsActive == EntityIsActive) &&
            (!EntityDefID.HasValue || d.EntityDefID == EntityDefID)).OrderBy(c => c.Name).Include(includes).AsNoTracking().ToList();
        }
    }
}
