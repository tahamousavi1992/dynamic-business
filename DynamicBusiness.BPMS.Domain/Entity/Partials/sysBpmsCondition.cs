using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsCondition
    {
        public sysBpmsCondition() { }
        public ResultOperation Update(Guid gatewayID, Guid? sequenceFlowID, string Code)
        { 
            this.GatewayID = gatewayID;
            this.SequenceFlowID = sequenceFlowID;
            this.Code = Code;
            ResultOperation resultOperation = new ResultOperation(this);
            if (string.IsNullOrWhiteSpace(this.Code))
                resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsCondition.Code), nameof(sysBpmsCondition)));
            if (!this.SequenceFlowID.HasValue)
                resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsCondition.SequenceFlowID), nameof(sysBpmsCondition)));
            return resultOperation;
        }

        public void Load(sysBpmsCondition Condition)
        {
            this.ID = Condition.ID;
            this.GatewayID = Condition.GatewayID;
            this.SequenceFlowID = Condition.SequenceFlowID;
            this.Code = Condition.Code;
        }
    }

}
