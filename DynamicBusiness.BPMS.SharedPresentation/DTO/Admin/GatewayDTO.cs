using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class GatewayDTO
    {
        public GatewayDTO() { }
        public GatewayDTO(sysBpmsGateway gateway)
        {
            this.ID = gateway.ID;
            this.ElementID = gateway.ElementID;
            this.ProcessID = gateway.ProcessID;
            this.DefaultSequenceFlowID = gateway.DefaultSequenceFlowID;
        }
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public Guid ProcessID { get; set; }
        [DataMember]
        public string ElementID { get; set; }
        [DataMember]
        public Guid? DefaultSequenceFlowID { get; set; }
    }
}