using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsDepartmentMember
    {
        public enum e_RoleLU
        {
            Requester = 1,
        }
        public sysBpmsDepartmentMember() { }

        public void Update(Guid DepartmentID, Guid UserID, int RoleLU)
        {
            this.DepartmentID = DepartmentID;
            this.UserID = UserID;
            this.RoleLU = RoleLU;
        }

      
    }
}
