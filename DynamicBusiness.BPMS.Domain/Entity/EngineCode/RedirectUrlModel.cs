using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    public class RedirectUrlModel
    {
        public RedirectUrlModel() { }
        public RedirectUrlModel(string url, bool newTab)
        {
            this.Url = url;
            this.NewTab = newTab;
        }
        public RedirectUrlModel(Guid applicationPageId, List<string> listParameter, bool newTab)
        {
            this.ApplicationPageId = applicationPageId;
            this.NewTab = newTab;
            this.ListParameter = listParameter;
        }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public bool NewTab { get; set; }
        [DataMember]
        public Guid? ApplicationPageId { get; set; }
        [DataMember]
        public List<string> ListParameter { get; set; }
    }
}
