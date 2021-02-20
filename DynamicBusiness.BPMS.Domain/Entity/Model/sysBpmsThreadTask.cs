
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsThreadTask
    {
        
        public sysBpmsThreadTask()
        {
            this.ThreadEvents = new HashSet<sysBpmsThreadEvent>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(Thread))]
        public System.Guid ThreadID { get; set; }
        [ForeignKey(nameof(Task))]
        public System.Guid TaskID { get; set; }
        [ForeignKey(nameof(User))]
        public Nullable<System.Guid> OwnerUserID { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        [Required(AllowEmptyStrings = true)]
        public string Description { get; set; }
        public string OwnerRole { get; set; }
        public Nullable<int> PriorityLU { get; set; }
        public int StatusLU { get; set; }
    
        public virtual sysBpmsTask Task { get; set; }
        public virtual sysBpmsThread Thread { get; set; }
        
        public virtual ICollection<sysBpmsThreadEvent> ThreadEvents { get; set; }
        public virtual sysBpmsUser User { get; set; }


    }
}
