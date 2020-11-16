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
    public class RowHtml : ElementBase
    {
        public RowHtml()
        {

        }
        public RowHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId)
        {
            this.IsFooter = obj["isFooter"] != null && (bool)obj["isFooter"];
            this.Columns = obj["columns"].Select(c => new ColumnHtml((JObject)c, _helper, dynamicFormId, isFormReadOnly)).ToList();
        }

        [DataMember]
        public bool IsFooter { get; set; }

        [DataMember]
        public List<ColumnHtml> Columns { get; set; }
    }
}
