
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class sysBpmsUser
    {
        
        public sysBpmsUser()
        {
            this.ApplicationPageAccesses = new HashSet<sysBpmsApplicationPageAccess>();
            this.DepartmentMembers = new HashSet<sysBpmsDepartmentMember>();
            this.Threads = new HashSet<sysBpmsThread>();
            this.ThreadTasks = new HashSet<sysBpmsThreadTask>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public string Mobile { get; set; }
    
        
        public virtual ICollection<sysBpmsApplicationPageAccess> ApplicationPageAccesses { get; set; }
        
        public virtual ICollection<sysBpmsDepartmentMember> DepartmentMembers { get; set; }
        
        public virtual ICollection<sysBpmsThread> Threads { get; set; }
        
        public virtual ICollection<sysBpmsThreadTask> ThreadTasks { get; set; }
    }
}
