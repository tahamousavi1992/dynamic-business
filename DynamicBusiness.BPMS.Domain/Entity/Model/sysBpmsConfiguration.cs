
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsConfiguration
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
        [Required]
        [MaxLength(500)]
        public string Label { get; set; }
        [Required]
        public string DefaultValue { get; set; }
        [Required]
        public string Value { get; set; }
        public Nullable<System.DateTime> LastUpdateOn { get; set; }
    }
}
