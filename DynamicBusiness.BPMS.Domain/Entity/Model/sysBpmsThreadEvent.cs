
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class sysBpmsThreadEvent
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(Thread))]
        public System.Guid ThreadID { get; set; }
        [ForeignKey(nameof(Event))]
        public System.Guid EventID { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime ExecuteDate { get; set; }
        public int StatusLU { get; set; }
        [ForeignKey(nameof(ThreadTask))]
        public Nullable<System.Guid> ThreadTaskID { get; set; }
    
        public virtual sysBpmsEvent Event { get; set; }
        public virtual sysBpmsThread Thread { get; set; }
        public virtual sysBpmsThreadTask ThreadTask { get; set; }
    }
}
