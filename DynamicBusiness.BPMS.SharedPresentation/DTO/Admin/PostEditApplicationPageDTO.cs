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
    public partial class PostEditApplicationPageDTO
    {
        [DataMember]
        public DynamicFormDTO DynamicFormDTO { get; set; }

        [DataMember]
        public List<ApplicationPageAccessDTO> listRole { get; set; }
        [DataMember]
        public List<ApplicationPageAccessDTO> listUser { get; set; }
    }
}