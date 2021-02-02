
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class sysBpmsLane
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(Element))]
        public string ElementID { get; set; }
        [ForeignKey(nameof(Process))]
        public System.Guid ProcessID { get; set; }
    
        public virtual sysBpmsElement Element { get; set; }
        public virtual sysBpmsProcess Process { get; set; }
    }
}
