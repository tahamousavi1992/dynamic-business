
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsVariableDependency
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(DependentVariable))]
        public System.Guid DependentVariableID { get; set; }
        [Required]
        [MaxLength(250)]
        public string DependentPropertyName { get; set; }
        [ForeignKey(nameof(ToVariable))]
        public Nullable<System.Guid> ToVariableID { get; set; }
        [Required]
        [MaxLength(250)]
        public string ToPropertyName { get; set; }
        public string Description { get; set; }
    
        public virtual sysBpmsVariable DependentVariable { get; set; }
        public virtual sysBpmsVariable ToVariable { get; set; }
    }
}
