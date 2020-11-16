using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsSequenceFlow
    {
        public void Load(sysBpmsSequenceFlow sequenceFlow)
        {
            this.ID = sequenceFlow.ID;
            this.ElementID = sequenceFlow.ElementID;
            this.Name = sequenceFlow.Name;
            this.ProcessID = sequenceFlow.ProcessID;
            this.SourceElementID = sequenceFlow.SourceElementID;
            this.TargetElementID = sequenceFlow.TargetElementID;
        }
         
    }
}
