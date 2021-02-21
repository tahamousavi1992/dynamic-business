
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsSequenceFlow
    {
        
        public sysBpmsSequenceFlow()
        {
            this.Conditions = new HashSet<sysBpmsCondition>();
            this.Gateways = new HashSet<sysBpmsGateway>();
        }
        
        public System.Guid ID { get; set; }
        [Required]
        [MaxLength(100)]
        public string ElementID { get; set; }
        [ForeignKey(nameof(Process))]
        public System.Guid ProcessID { get; set; }
        [MaxLength(500)]
        [Required(AllowEmptyStrings = true)]
        public string Name { get; set; }
        [MaxLength(100)]
        [Required(AllowEmptyStrings = true)]
        public string SourceElementID { get; set; }
        [MaxLength(100)]
        public string TargetElementID { get; set; }
    
        
        public virtual ICollection<sysBpmsCondition> Conditions { get; set; }
        public virtual sysBpmsElement Element { get; set; }
        
        public virtual ICollection<sysBpmsGateway> Gateways { get; set; }
        public virtual sysBpmsProcess Process { get; set; }

        public sysBpmsSequenceFlow Clone()
        {
            return new sysBpmsSequenceFlow
            {
                ID = this.ID,
                ElementID = this.ElementID,
                ProcessID = this.ProcessID,
                Name = this.Name,
                SourceElementID = this.SourceElementID,
                TargetElementID = this.TargetElementID,
            };
        }
    }
}
