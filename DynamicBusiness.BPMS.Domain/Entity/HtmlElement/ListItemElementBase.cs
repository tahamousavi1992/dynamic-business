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
    public class ListItemElementBase : BindingElementBase
    {
        public ListItemElementBase()
        {

        }
        public ListItemElementBase(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId, isFormReadOnly)
        {
            this.Key = DomainUtility.toString(obj["fillKey"]);
            this.Text = DomainUtility.toString(obj["fillText"]);
            this.Text = string.IsNullOrWhiteSpace(this.Text) ? nameof(this.Text) : this.Text;
            this.Key = string.IsNullOrWhiteSpace(this.Key) ? nameof(this.Key) : this.Key;

            this.FillList = DomainUtility.toString(obj["fillListBinding"]);
        }

        [DataMember]
        public string FillList { get; set; }

        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public List<OptionHtml> Options { get; set; }
 
    }
}
