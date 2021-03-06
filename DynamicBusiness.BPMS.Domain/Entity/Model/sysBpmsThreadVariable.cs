
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsThreadVariable
    {
        
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(Thread))]
        public System.Guid ThreadID { get; set; }
        [ForeignKey(nameof(Variable))]
        public System.Guid VariableID { get; set; }
        [Required(AllowEmptyStrings = true)]
        public string Value { get; set; }
    
        public virtual sysBpmsThread Thread { get; set; }
        public virtual sysBpmsVariable Variable { get; set; }

        public sysBpmsThreadVariable Clone()
        {
            return new sysBpmsThreadVariable
            {
                ID = this.ID,
                ThreadID = this.ThreadID,
                VariableID = this.VariableID,
                Value = this.Value, 
            };
        }
    }
}
