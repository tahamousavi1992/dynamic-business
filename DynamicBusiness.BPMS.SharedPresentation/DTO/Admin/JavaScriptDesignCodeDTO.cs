using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class JavaScriptDesignCodeDTO
    {
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string CallBack { get; set; }
        [DataMember]
        public string CancelFunction { get; set; }
        [DataMember]
        public List<QueryModel> GetControls { get; set; }
        [DataMember]
        public List<QueryModel> GetAllJavaMethods { get; set; }
        [DataMember]
        public Guid DynamicFormId { get; set; }
    }
}