using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class GatewayRepository : IGatewayRepository
    {
        private Db_BPMSEntities Context;
        public GatewayRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsGateway gateway)
        {
            this.Context.sysBpmsGateways.Add(gateway);
        }

        public void Update(sysBpmsGateway gateway)
        {
            sysBpmsGateway retVal = (from p in this.Context.sysBpmsGateways
                                 where p.ID == gateway.ID
                                 select p).FirstOrDefault();
            retVal.Load(gateway);
            if (gateway.SequenceFlow != null && gateway.SequenceFlow.ID != gateway.DefaultSequenceFlowID)
            {
                retVal.SequenceFlow = gateway.SequenceFlow;
            }
        }

        public void Delete(Guid id)
        {
            sysBpmsGateway Gateway = this.Context.sysBpmsGateways.FirstOrDefault(d => d.ID == id);
            if (Gateway != null)
            {
                this.Context.sysBpmsGateways.Remove(Gateway);
            }
        }

        public sysBpmsGateway GetInfo(Guid id)
        {
            return (from P in this.Context.sysBpmsGateways
                    where P.ID == id
                    select P).AsNoTracking().FirstOrDefault();
        }

        public sysBpmsGateway GetInfo(string elementId, Guid processId)
        {
            return (from P in this.Context.sysBpmsGateways
                    where P.ElementID == elementId && P.ProcessID == processId
                    select P).AsNoTracking().FirstOrDefault(); ;
        }

        public List<sysBpmsGateway> GetList(Guid processID)
        {
            return this.Context.sysBpmsGateways.Include(c => c.Element).Where(d =>
              (d.ProcessID == processID)).OrderBy(c => c.Element.Name).AsNoTracking().ToList();
        }

        public List<sysBpmsGateway> GetListByDefaultSequence(Guid defaultSequenceFlowID)
        {
 
            return this.Context.sysBpmsGateways.Include(c => c.Element).Where(d =>
              (d.DefaultSequenceFlowID == defaultSequenceFlowID)).OrderBy(c => c.Element.Name).AsNoTracking().ToList();
        }
    }
}
