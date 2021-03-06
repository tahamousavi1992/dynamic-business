
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsConfiguration
    {
        
        public System.Guid ID { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string Label { get; set; }
        [Required(AllowEmptyStrings = true)]
        public string DefaultValue { get; set; }
        [Required(AllowEmptyStrings = true)]
        public string Value { get; set; }
        public Nullable<System.DateTime> LastUpdateOn { get; set; }
    }
}
