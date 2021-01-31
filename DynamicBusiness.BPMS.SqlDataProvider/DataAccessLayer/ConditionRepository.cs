using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ConditionRepository : IConditionRepository
    {
        private Db_BPMSEntities Context;
        public ConditionRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsCondition Condition)
        {
            Condition.ID = Guid.NewGuid();
            this.Context.sysBpmsConditions.Add(Condition);
        }

        public void Update(sysBpmsCondition Condition)
        {
            sysBpmsCondition retVal = (from p in this.Context.sysBpmsConditions
                                   where p.ID == Condition.ID
                                   select p).FirstOrDefault();
            retVal.Load(Condition);
        }

        public void Delete(Guid ConditionId)
        {
            sysBpmsCondition Condition = this.Context.sysBpmsConditions.FirstOrDefault(d => d.ID == ConditionId);
            if (Condition != null)
            {
                this.Context.sysBpmsConditions.Remove(Condition);
            }
        }

        public sysBpmsCondition GetInfo(Guid ID)
        {
            return (from P in this.Context.sysBpmsConditions
                    where P.ID == ID
                    select P).AsNoTracking().FirstOrDefault();

        }

        public List<sysBpmsCondition> GetList(Guid? gatewayID, Guid? sequenceFlowID, Guid? processId)
        {
            List<sysBpmsCondition> rettVal = this.Context.sysBpmsConditions.Where(d =>
            (!processId.HasValue || d.Gateway.ProcessID == processId) &&
            (!gatewayID.HasValue || d.GatewayID == gatewayID) &&
            (!sequenceFlowID.HasValue || d.SequenceFlowID == sequenceFlowID)).AsNoTracking().ToList();

            return rettVal;
        }
    }
}
