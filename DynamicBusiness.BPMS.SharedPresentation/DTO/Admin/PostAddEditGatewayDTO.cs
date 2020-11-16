using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class PostAddEditGatewayDTO
    {
        [DataMember]
        public Guid GatewayID { get; set; }
        [DataMember]
        public Guid ProcessId { get; set; }
        [DataMember] 
        public List<ConditionDTO> ListConditions { get; set; }
 
    }
}