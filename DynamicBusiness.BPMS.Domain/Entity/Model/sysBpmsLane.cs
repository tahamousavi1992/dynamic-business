
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsLane
    {
        
        public System.Guid ID { get; set; }
        //[ForeignKey(nameof(Element))]
        [Required]
        [MaxLength(100)]
        public string ElementID { get; set; }
        [ForeignKey(nameof(Process))]
        public System.Guid ProcessID { get; set; }
    
        public virtual sysBpmsElement Element { get; set; }
        public virtual sysBpmsProcess Process { get; set; }
    }
}
