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
    public class PostAddEditStyleSheetDTO
    {
        public PostAddEditStyleSheetDTO() { } 

        [Required]
        [DataMember]
        public Guid DynamicFormId { get; set; }

 
        [Required]
        [DataMember]
        public string StyleCode { get; set; }
 
    }
}