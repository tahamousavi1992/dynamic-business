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
    public class PostDesignCodePostIndexDTO
    {
        public PostDesignCodePostIndexDTO() { }

        [DataMember]
        public string DynamicFormId { get; set; }
        [DataMember]
        public string CallBack { get; set; }
        [DataMember]
        public string DesignCode { get; set; }
        [DataMember] 
        public int CodeType { get; set; }
    }
}