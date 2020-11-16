using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class ConditionDTO
    {
        public ConditionDTO() { }

        public ConditionDTO(sysBpmsCondition condition)
        {
            this.ID = condition.ID;
            this.GatewayID = condition.GatewayID;
            this.SequenceFlowID = condition.SequenceFlowID;
            this.Code = condition.Code;
        }
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public Guid GatewayID { get; set; } 
        [Required]
        [DataMember]
        public Guid? SequenceFlowID { get; set; } 
        [DataMember]
        public string Code { get; set; }
        //only is used in gateway manage and filled from out
        [DataMember]
        public string RenderCode { get; set; }
    }
}