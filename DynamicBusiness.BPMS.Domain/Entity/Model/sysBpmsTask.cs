 

namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class sysBpmsTask
    {
        
        public sysBpmsTask()
        {
            this.Steps = new HashSet<sysBpmsStep>();
            this.ThreadTasks = new HashSet<sysBpmsThreadTask>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(Element))]
        public string ElementID { get; set; }
        public int TypeLU { get; set; }
        public string Code { get; set; }
        public Nullable<int> MarkerTypeLU { get; set; }
        public Nullable<int> OwnerTypeLU { get; set; }
        public string RoleName { get; set; }
        public string Rule { get; set; }
        public string UserID { get; set; }
        [ForeignKey(nameof(Process))]
        public System.Guid ProcessID { get; set; }
    
        public virtual sysBpmsElement Element { get; set; }
        public virtual sysBpmsProcess Process { get; set; }
        
        public virtual ICollection<sysBpmsStep> Steps { get; set; }
        
        public virtual ICollection<sysBpmsThreadTask> ThreadTasks { get; set; }
    }
}
