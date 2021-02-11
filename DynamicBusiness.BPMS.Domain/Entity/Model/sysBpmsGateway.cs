
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsGateway
    {
        
        public sysBpmsGateway()
        {
            this.Conditions = new HashSet<sysBpmsCondition>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(Element))]
        [Required(AllowEmptyStrings = true)]
        [MaxLength(100)]
        public string ElementID { get; set; }
        [ForeignKey(nameof(SequenceFlow))]
        public Nullable<System.Guid> DefaultSequenceFlowID { get; set; }
        public Nullable<int> TypeLU { get; set; }
        public string TraceToStart { get; set; }
        [ForeignKey(nameof(Process))]
        public System.Guid ProcessID { get; set; }
    
        
        public virtual ICollection<sysBpmsCondition> Conditions { get; set; }
        public virtual sysBpmsElement Element { get; set; }
        public virtual sysBpmsProcess Process { get; set; }
        public virtual sysBpmsSequenceFlow SequenceFlow { get; set; }
    }
}
