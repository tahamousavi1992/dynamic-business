
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class sysBpmsProcessGroup
    {
        
        public sysBpmsProcessGroup()
        {
            this.Processes = new HashSet<sysBpmsProcess>();
            this.ChildrenProcessGroup = new HashSet<sysBpmsProcessGroup>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(ProcessGroup))]
        public Nullable<System.Guid> ProcessGroupID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    
        
        public virtual ICollection<sysBpmsProcess> Processes { get; set; }
        
        public virtual ICollection<sysBpmsProcessGroup> ChildrenProcessGroup { get; set; }
        public virtual sysBpmsProcessGroup ProcessGroup { get; set; }
    }
}
