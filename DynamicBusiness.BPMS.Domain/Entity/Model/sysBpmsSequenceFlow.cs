
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class sysBpmsSequenceFlow
    {
        
        public sysBpmsSequenceFlow()
        {
            this.Conditions = new HashSet<sysBpmsCondition>();
            this.Gateways = new HashSet<sysBpmsGateway>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(Element))]
        public string ElementID { get; set; }
        [ForeignKey(nameof(Process))]
        public System.Guid ProcessID { get; set; }
        public string Name { get; set; }
        public string SourceElementID { get; set; }
        public string TargetElementID { get; set; }
    
        
        public virtual ICollection<sysBpmsCondition> Conditions { get; set; }
        public virtual sysBpmsElement Element { get; set; }
        
        public virtual ICollection<sysBpmsGateway> Gateways { get; set; }
        public virtual sysBpmsProcess Process { get; set; }
    }
}
