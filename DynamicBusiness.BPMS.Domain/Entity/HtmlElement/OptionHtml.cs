using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    [KnownType(typeof(OptionHtml))]
    public class OptionHtml
    {
        public OptionHtml() { }
        public OptionHtml(JObject obj)
        {
            this.Label = DomainUtility.toString(obj["label"]);
            this.Value = DomainUtility.toString(obj["value"]);
            this.Selected = (obj["selected"] != null && (bool)obj["selected"])||
                            (obj["checked"] != null && (bool)obj["checked"]);
        }
        [DataMember]
        public string Label { get; set; }
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public bool Selected { get; set; }
    }
}
