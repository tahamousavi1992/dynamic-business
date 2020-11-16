using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    [KnownType(typeof(TextBoxHtml))]
    public class TextBoxHtml : BindingElementBase
    {
        public TextBoxHtml() { }
        public TextBoxHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId, isFormReadOnly)
        {
            this.IsRequired = obj["isRequired"] != null && ((bool)obj["isRequired"]);
            this.FontIconCssClass = DomainUtility.toString(obj["fontIconCssClass"]);
            this.SubType = DomainUtility.toString(obj["subtype"]);
            this.PlaceHolderText = DomainUtility.toString(obj["placeholderText"]);
            this.IsMultiline = obj["isMultiline"] != null && ((bool)obj["isMultiline"]);
            this.Pattern = obj["pattern"].ToStringObj();
            this.MaxLength = obj["maxLength"]?.ToIntObj();
            this.FillValue();
        }
        [DataMember]
        public bool IsRequired { get; set; }
        [DataMember]
        public string FontIconCssClass { get; set; }
        [DataMember]
        public string SubType { get; set; }
        [DataMember]
        public bool IsMultiline { get; set; }
        [DataMember]
        public string InputType
        {
            get
            {
                return this.SubType == e_TextBoxType.password.ToString() ? this.SubType : "text";
            }
            private set { }
        }
        [DataMember]
        public string PlaceHolderText { get; set; }
        [DataMember]
        public string Pattern { get; set; }
        [DataMember]
        public int? MaxLength { get; set; }
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
 
        public enum e_TextBoxType
        {
            text,
            threadTaskDescription,
            password,
            email,
            codeMeli,
            justCharacter,
            justNumber,
            postalCode,
            mobile,
            justEnglishCharacter,
            justPersianCharacter,
            symbol,
            characterAndNumber,
            sepratorNumber,
            phone,
            enCharacterAndNumber,
            allExceptInPersian,
            codeMeliCompany,
        }
    }

}
