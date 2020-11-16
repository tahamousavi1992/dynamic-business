using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class ComboSearchItem
    {
        public ComboSearchItem() { }
        public ComboSearchItem(string id, string text)
        {
            this.text = text;
            this.id = id;
        }
        [DataMember]
        public string text { get; set; }
        [DataMember]
        public string id { get; set; }
    }
}