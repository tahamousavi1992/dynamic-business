using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsProcessGroup
    {
        public ResultOperation Update(Guid? processGroupID, string name, string description)
        {
            this.ProcessGroupID = processGroupID;
            this.Name = name;
            this.Description = description;
            return new ResultOperation();
        }

        public void Load(sysBpmsProcessGroup sysBpmsProcessGroup)
        {
            this.ID = sysBpmsProcessGroup.ID;
            this.ProcessGroupID = sysBpmsProcessGroup.ProcessGroupID;
            this.Name = sysBpmsProcessGroup.Name;
            this.Description = sysBpmsProcessGroup.Description;
        }
    }
}
