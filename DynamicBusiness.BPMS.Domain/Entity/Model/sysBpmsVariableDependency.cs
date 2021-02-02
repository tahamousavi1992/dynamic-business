
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class sysBpmsVariableDependency
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(VariableDependent))]
        public System.Guid DependentVariableID { get; set; }
        public string DependentPropertyName { get; set; }
        [ForeignKey(nameof(VariableTo))]
        public Nullable<System.Guid> ToVariableID { get; set; }
        public string ToPropertyName { get; set; }
        public string Description { get; set; }
    
        public virtual sysBpmsVariable VariableDependent { get; set; }
        public virtual sysBpmsVariable VariableTo { get; set; }
    }
}
