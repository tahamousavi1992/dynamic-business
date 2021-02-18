using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsAPIAccess
    {
        public sysBpmsAPIAccess Update(string name, string ipAddress, string accessKey, bool isActive)
        {
            this.Name = name.ToStringObj().Trim();
            this.IPAddress = ipAddress.ToStringObj().Trim();
            this.AccessKey = accessKey.ToStringObj().Trim();
            this.IsActive = isActive;
            return this;
        }
        public sysBpmsAPIAccess Update(bool isActive)
        {
            this.IsActive = isActive;
            return this;
        }
        
    }
}
