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
    
    public partial class sysBpmsThreadEvent
    {
        public System.Guid ID { get; set; }
        public System.Guid ThreadID { get; set; }
        public System.Guid EventID { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime ExecuteDate { get; set; }
        public int StatusLU { get; set; }
        public Nullable<System.Guid> ThreadTaskID { get; set; }
    
        public virtual sysBpmsEvent sysBpmsEvent { get; set; }
        public virtual sysBpmsThread sysBpmsThread { get; set; }
        public virtual sysBpmsThreadTask sysBpmsThreadTask { get; set; }
    }
}
