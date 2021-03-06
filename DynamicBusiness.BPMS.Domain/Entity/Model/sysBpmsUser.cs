
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsUser
    {
        
        public sysBpmsUser()
        {
            this.ApplicationPageAccesses = new HashSet<sysBpmsApplicationPageAccess>();
            this.DepartmentMembers = new HashSet<sysBpmsDepartmentMember>();
            this.Threads = new HashSet<sysBpmsThread>();
            this.ThreadTasks = new HashSet<sysBpmsThreadTask>();
        }
        
        public System.Guid ID { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string Username { get; set; }
        [MaxLength(500)]
        public string FirstName { get; set; }
        [MaxLength(500)]
        public string LastName { get; set; }
        [MaxLength(500)]
        public string Email { get; set; }
        [MaxLength(30)]
        public string Tel { get; set; }
        [MaxLength(30)]
        public string Mobile { get; set; }
    
        
        public virtual ICollection<sysBpmsApplicationPageAccess> ApplicationPageAccesses { get; set; }
        
        public virtual ICollection<sysBpmsDepartmentMember> DepartmentMembers { get; set; }
        
        public virtual ICollection<sysBpmsThread> Threads { get; set; }
        
        public virtual ICollection<sysBpmsThreadTask> ThreadTasks { get; set; }
    }
}
