 
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class sysBpmsCondition
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(Gateway))]
        public System.Guid GatewayID { get; set; }
        [ForeignKey(nameof(SequenceFlow))]
        public Nullable<System.Guid> SequenceFlowID { get; set; }
        public string Code { get; set; }
    
        public virtual sysBpmsGateway Gateway { get; set; }
        public virtual sysBpmsSequenceFlow SequenceFlow { get; set; }
    }
}
