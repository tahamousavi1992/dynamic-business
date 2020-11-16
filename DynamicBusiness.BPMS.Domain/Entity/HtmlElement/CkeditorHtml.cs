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
    [KnownType(typeof(CkeditorHtml))]
    public class CkeditorHtml : BindingElementBase
    {
        public CkeditorHtml() { }
        public CkeditorHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId, isFormReadOnly)
        {
            this.IsRequired = obj["isRequired"] != null && ((bool)obj["isRequired"]);
            this.FillValue();
        }
        [DataMember]
        public bool IsRequired { get; set; }
        private void FillValue()
        {
            if (this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.Onload ||
                this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.Preview)
            {
                if (string.IsNullOrWhiteSpace(this.Parameter))
                    this.FillData();
            }
            else
                if (this.Helper?.FormAction != HtmlElementHelperModel.e_FormAction.FillMode)
                this.Value = this.Helper.ListFormQueryModel?.FirstOrDefault(c => c.Key == this.Id)?.Value.ToStringObj() ?? "";
        }

        public override void FillData(List<QueryModel> listFormQueryModel = null)
        {
            this.Value = this.Helper.DataManageHelper.GetValueByBinding(this.Fill, listFormQueryModel).ToStringObj();
        }

        public override void IsValid(ResultOperation resultOperation)
        {
            if (this.IsRequired && string.IsNullOrWhiteSpace(this.Value?.ToStringObj()))
            {
                resultOperation.AddError(string.Format("کاربر گرامی مقدار فیلد {0} الزامی میباشد", this.Label));
            }
        }
    }
}
