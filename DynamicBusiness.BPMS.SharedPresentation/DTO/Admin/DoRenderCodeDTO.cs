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
    public class DoRenderCodeDTO
    {
        public DoRenderCodeDTO() { }
        [DataMember]
        public string XmlCode { get; set; }
        [DataMember]
        public bool? AddGotoLabel { get; set; }
        [DataMember]
        public bool OnlyConditional { get; set; }
        [DataMember]
        public Guid? ProcessId { get; set; }
        [DataMember]
        public Guid? ApplicationPageId { get; set; }
    }
}