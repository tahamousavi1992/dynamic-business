
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsVariableDependency
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        public Guid DependentVariableID { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(250)]
        public string DependentPropertyName { get; set; }
        public Guid? ToVariableID { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(250)]
        public string ToPropertyName { get; set; }
        public string Description { get; set; }

        public virtual sysBpmsVariable DependentVariable { get; set; }
        public virtual sysBpmsVariable ToVariable { get; set; }
    }
}
