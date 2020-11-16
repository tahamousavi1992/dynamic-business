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
    public class ColumnHtml : ElementBase
    {
        public ColumnHtml()
        {

        }

        public ColumnHtml(JObject obj, HtmlElementHelperModel helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, helper, dynamicFormId)
        {
            this.children =
                obj["children"].Select(c => ElementBase.GetElement((JObject)c, helper, dynamicFormId, isFormReadOnly))
                .Where(c => c != null/**for cheking controls having not access **/).ToList();
        }

        [DataMember]
        public List<object> children { get; set; }

    }
}
