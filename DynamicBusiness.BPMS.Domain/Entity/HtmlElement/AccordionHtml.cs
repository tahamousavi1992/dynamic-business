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
    public class AccordionHtml : ElementBase
    {
        public AccordionHtml()
        {

        }
        public AccordionHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId)
        {
            this.Cards = obj["cards"].Select(c => new CardHtml((JObject)c, _helper, dynamicFormId, isFormReadOnly)).ToList();
        }
        [DataMember]
        public List<CardHtml> Cards { get; set; }

        public List<ColumnHtml> GetListColumn()
        {
            return this.Cards.SelectMany(c => c.Rows.SelectMany(f => f.Columns)).ToList();
        }
    }
}
