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
    public class CardHtml : ElementBase
    {
        public CardHtml()
        {

        }
        public CardHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId)
        {
            this.Rows = obj["rows"].Select(c => new RowHtml((JObject)c, _helper, dynamicFormId, isFormReadOnly)).ToList();
        }
        [DataMember]
        public List<RowHtml> Rows { get; set; }
    }
}
