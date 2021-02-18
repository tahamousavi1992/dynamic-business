using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsDepartment
    {
        public void Update(Guid? DepartmentID, string Name)
        {
            this.DepartmentID = DepartmentID.HasValue ? DepartmentID : (Guid?)null;
            this.Name = Name;
            if (this.ID == Guid.Empty)
                this.IsActive = true;
        }

        
    }
}
