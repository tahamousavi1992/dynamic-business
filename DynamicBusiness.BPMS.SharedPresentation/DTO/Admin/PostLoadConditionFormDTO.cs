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
    public class PostLoadConditionFormDTO
    {
        public PostLoadConditionFormDTO() { }

 
        [DataMember]
        public bool IsOutputYes { get; set; }
        [DataMember]
        public string Data { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string DynamicFormId { get; set; }
        [DataMember]
        public string ShapeId { get; set; }
        [DataMember]
        public string ParentShapeId { get; set; }
        [DataMember]
        public bool IsFirst { get; set; } 
    }
}