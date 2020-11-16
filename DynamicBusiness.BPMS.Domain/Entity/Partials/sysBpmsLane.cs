using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsLane
    {
        public void Load(sysBpmsLane lane)
        {
            this.ID = lane.ID;
            this.ProcessID = lane.ProcessID;
            this.ElementID = lane.ElementID;
        }

    }
}
