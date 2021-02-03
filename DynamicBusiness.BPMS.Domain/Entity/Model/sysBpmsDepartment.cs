
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsDepartment
    {
        
        public sysBpmsDepartment()
        {
            this.ApplicationPageAccesses = new HashSet<sysBpmsApplicationPageAccess>();
            this.ChildrenDepartments = new HashSet<sysBpmsDepartment>();
            this.DepartmentMembers = new HashSet<sysBpmsDepartmentMember>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(ParentDepartment))]
        public Nullable<System.Guid> DepartmentID { get; set; }
        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
    
        
        public virtual ICollection<sysBpmsApplicationPageAccess> ApplicationPageAccesses { get; set; }
        
        public virtual ICollection<sysBpmsDepartment> ChildrenDepartments { get; set; }
        public virtual sysBpmsDepartment ParentDepartment { get; set; }
        
        public virtual ICollection<sysBpmsDepartmentMember> DepartmentMembers { get; set; }
    }
}
