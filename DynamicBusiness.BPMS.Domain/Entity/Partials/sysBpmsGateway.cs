using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsGateway
    {
        public enum e_TypeLU
        {
            ExclusiveGateWay = 1,
            InclusiveGateWay = 2,
            ParallelGateWay = 3,
        }

        public sysBpmsGateway(Guid id, Guid processId, string elementID, Guid? defaultSequenceFlowID, int typeLU, string traceToStart)
        {
            this.ID = id;
            this.ElementID = elementID;
            this.ProcessID = processId;
            this.DefaultSequenceFlowID = defaultSequenceFlowID;
            this.TypeLU = typeLU;
            this.TraceToStart = traceToStart;
        }

        
    }
}
