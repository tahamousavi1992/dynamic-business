using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsThreadVariable
    {
        public void Load(sysBpmsThreadVariable ThreadVariable)
        {
            this.ID = ThreadVariable.ID;
            this.ThreadID = ThreadVariable.ThreadID;
            this.VariableID = ThreadVariable.VariableID;
            this.Value = ThreadVariable.Value;
        }
    }
}
