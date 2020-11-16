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
    [KnownType(typeof(CheckBoxHtml))]
    public class CheckBoxHtml : BindingElementBase
    {
        public CheckBoxHtml()
        {

        }

        public CheckBoxHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId, isFormReadOnly)
        {
            this.IsRequired = obj["isRequired"] != null && (bool)obj["isRequired"];
            this.FontIconCssClass = DomainUtility.toString(obj["fontIconCssClass"]);
            this.IsSwitch = obj["isSwitch"] != null && (bool)obj["isSwitch"];
            this.IsInline = obj["isInline"] != null && obj["isInline"].ToBoolObj();
            this.FillValue();
        }

        [DataMember]
        public bool IsInline { get; set; }
        [DataMember]
        public bool IsRequired { get; set; }
        [DataMember]
        public string FontIconCssClass { get; set; }
        [DataMember]
        public bool IsSwitch { get; set; }
        [DataMember]
        public bool Checked { get { return this.Value.ToBoolObj(); } private set { } }

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
                this.Value = this.Helper?.ListFormQueryModel?.FirstOrDefault(c => c.Key == this.Id)?.Value.ToBoolObj();
        }

        public override void FillData(List<QueryModel> listFormQueryModel = null)
        {
            this.Value = this.Helper.DataManageHelper.GetValueByBinding<bool?>(this.Fill, listFormQueryModel);
        }

    }
}
