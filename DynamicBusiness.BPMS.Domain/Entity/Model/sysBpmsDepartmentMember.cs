
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class sysBpmsDepartmentMember
    {
        
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(Department))]
        public System.Guid DepartmentID { get; set; }
        public int RoleLU { get; set; }
        [ForeignKey(nameof(User))]
        public System.Guid UserID { get; set; }
    
        public virtual sysBpmsDepartment Department { get; set; }
        public virtual sysBpmsUser User { get; set; }
    }
}
