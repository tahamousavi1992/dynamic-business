using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class SequenceFlowRepository : ISequenceFlowRepository
    {
        private Db_BPMSEntities Context;
        public SequenceFlowRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsSequenceFlow SequenceFlow)
        {
            this.Context.sysBpmsSequenceFlows.Add(SequenceFlow);
        }

        public void Update(sysBpmsSequenceFlow SequenceFlow)
        {
            sysBpmsSequenceFlow retVal = (from p in this.Context.sysBpmsSequenceFlows
                                      where p.ID == SequenceFlow.ID
                                      select p).FirstOrDefault();
            retVal.Load(SequenceFlow);
        }

        public void Delete(Guid id)
        {
            sysBpmsSequenceFlow SequenceFlow = this.Context.sysBpmsSequenceFlows.FirstOrDefault(d => d.ID == id);
            if (SequenceFlow != null)
            {
                this.Context.sysBpmsSequenceFlows.Remove(SequenceFlow);
            }
        }

        public sysBpmsSequenceFlow GetInfo(Guid id)
        {
            return (from P in this.Context.sysBpmsSequenceFlows
                    where P.ID == id
                    select P).AsNoTracking().FirstOrDefault();
        }

        public sysBpmsSequenceFlow GetInfo(string elementId, Guid processId)
        {
            return (from P in this.Context.sysBpmsSequenceFlows
                    where P.ElementID == elementId && P.ProcessID == processId
                    select P).AsNoTracking().FirstOrDefault();
        }

        public List<sysBpmsSequenceFlow> GetList(Guid ProcessID, string TargetElementID, string SourceElementID, string Name)
        {
            List<sysBpmsSequenceFlow> rettVal = new List<sysBpmsSequenceFlow>();
            Name = DomainUtility.toString(Name);
            TargetElementID = DomainUtility.toString(TargetElementID);
            SourceElementID = DomainUtility.toString(SourceElementID);

            rettVal = this.Context.sysBpmsSequenceFlows.Where(d =>
            (d.sysBpmsElement.ProcessID == ProcessID) &&
            (TargetElementID == string.Empty || d.TargetElementID == TargetElementID) &&
            (SourceElementID == string.Empty || d.SourceElementID == SourceElementID) &&
            (Name == string.Empty || d.Name == Name)).AsNoTracking().ToList();

            return rettVal;
        }

        public List<sysBpmsSequenceFlow> GetList(Guid ProcessID)
        {
            return this.Context.sysBpmsSequenceFlows.Where(d =>
                (d.ProcessID == ProcessID || d.ProcessID == ProcessID)).AsNoTracking().ToList();
        }
    }
}
