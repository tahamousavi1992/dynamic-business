using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IConditionRepository
    {
        void Add(sysBpmsCondition Condition);
        void Update(sysBpmsCondition Condition);
        void Delete(Guid ConditionId);
        sysBpmsCondition GetInfo(Guid ID);
        List<sysBpmsCondition> GetList(Guid? gatewayID, Guid? SequenceFlowID, Guid? processId);
    }
}
