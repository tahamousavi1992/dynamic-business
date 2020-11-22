//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    
    public partial class sysBpmsThreadTask
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public sysBpmsThreadTask()
        {
            this.sysBpmsThreadEvents = new HashSet<sysBpmsThreadEvent>();
        }
    
        public System.Guid ID { get; set; }
        public System.Guid ThreadID { get; set; }
        public System.Guid TaskID { get; set; }
        public Nullable<System.Guid> OwnerUserID { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string Description { get; set; }
        public string OwnerRole { get; set; }
        public Nullable<int> PriorityLU { get; set; }
        public int StatusLU { get; set; }
    
        public virtual sysBpmsTask sysBpmsTask { get; set; }
        public virtual sysBpmsThread sysBpmsThread { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sysBpmsThreadEvent> sysBpmsThreadEvents { get; set; }
        public virtual sysBpmsUser sysBpmsUser { get; set; }
    }
}
