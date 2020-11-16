using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class PostAddEditFormDesignDTO
    {
        [DataMember]
        public string DesignJson { get; set; }
        [DataMember]
        public Guid DynamicFormId { get; set; }
    }
}