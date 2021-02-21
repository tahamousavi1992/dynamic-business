 
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class sysBpmsApplicationPageAccess
    {
        
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(ApplicationPage))]
        public System.Guid ApplicationPageID { get; set; }
        [ForeignKey(nameof(Department))]
        public Nullable<System.Guid> DepartmentID { get; set; }
        public Nullable<int> RoleLU { get; set; }
        [ForeignKey(nameof(User))]
        public Nullable<System.Guid> UserID { get; set; }
        public bool AllowAdd { get; set; }
        public bool AllowEdit { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowView { get; set; }
    
        public virtual sysBpmsApplicationPage ApplicationPage { get; set; }
        public virtual sysBpmsDepartment Department { get; set; }
        public virtual sysBpmsUser User { get; set; }
    }
}
