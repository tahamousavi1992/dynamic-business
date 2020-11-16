using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class PostOnLoadScriptCodeDTO
    {
        public PostOnLoadScriptCodeDTO() { } 
         
        [DataMember]
        public Guid DynamicFormId { get; set; }
         
        [DataMember]
        public string FunctionCode { get; set; }
 
    }
}