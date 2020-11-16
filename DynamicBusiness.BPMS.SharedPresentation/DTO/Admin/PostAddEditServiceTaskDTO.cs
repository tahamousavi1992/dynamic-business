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
    public class PostAddEditServiceTaskDTO
    {
        public PostAddEditServiceTaskDTO() { }


        [DataMember]
        public string ElementId { get; set; }
        [DataMember]
        public string DesignCode { get; set; }

    }
}