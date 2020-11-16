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
    public class PostAddEditOnEntryFormCodeDTO
    {
        public PostAddEditOnEntryFormCodeDTO() { }

        [DataMember]
        public Guid DynamicFormId { get; set; }
        [DataMember]
        public string DesignCode { get; set; }


    }
}