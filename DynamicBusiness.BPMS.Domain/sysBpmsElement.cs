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
    
    public partial class sysBpmsElement
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public sysBpmsElement()
        {
            this.sysBpmsEvents = new HashSet<sysBpmsEvent>();
            this.sysBpmsGateways = new HashSet<sysBpmsGateway>();
            this.sysBpmsLanes = new HashSet<sysBpmsLane>();
            this.sysBpmsSequenceFlows = new HashSet<sysBpmsSequenceFlow>();
            this.sysBpmsTasks = new HashSet<sysBpmsTask>();
        }
    
        public string ID { get; set; }
        public string Name { get; set; }
        public int TypeLU { get; set; }
        public System.Guid ProcessID { get; set; }
    
        public virtual sysBpmsProcess sysBpmsProcess { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sysBpmsEvent> sysBpmsEvents { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sysBpmsGateway> sysBpmsGateways { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sysBpmsLane> sysBpmsLanes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sysBpmsSequenceFlow> sysBpmsSequenceFlows { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<sysBpmsTask> sysBpmsTasks { get; set; }
    }
}
