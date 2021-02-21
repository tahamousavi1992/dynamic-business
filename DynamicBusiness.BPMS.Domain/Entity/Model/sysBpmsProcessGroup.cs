
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsProcessGroup
    {
        
        public sysBpmsProcessGroup()
        {
            this.Processes = new HashSet<sysBpmsProcess>();
            this.ChildrenProcessGroup = new HashSet<sysBpmsProcessGroup>();
        }
        
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(ProcessGroup))]
        public Nullable<System.Guid> ProcessGroupID { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string Name { get; set; }
        public string Description { get; set; }
    
        
        public virtual ICollection<sysBpmsProcess> Processes { get; set; }
        
        public virtual ICollection<sysBpmsProcessGroup> ChildrenProcessGroup { get; set; }
        public virtual sysBpmsProcessGroup ProcessGroup { get; set; }
    }
}
