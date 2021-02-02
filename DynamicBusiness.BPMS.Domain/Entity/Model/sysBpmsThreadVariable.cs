
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class sysBpmsThreadVariable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(Thread))]
        public System.Guid ThreadID { get; set; }
        [ForeignKey(nameof(Variable))]
        public System.Guid VariableID { get; set; }
        public string Value { get; set; }
    
        public virtual sysBpmsThread Thread { get; set; }
        public virtual sysBpmsVariable Variable { get; set; }
    }
}
