using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class PopUpDTO
    {
        [DataMember]
        public string PopUpID { get; set; }
        [DataMember]
        public string Caption { get; set; }
        [DataMember]
        public string PartialName { get; set; }
        [DataMember]
        public object Model { get; set; }
    }
}