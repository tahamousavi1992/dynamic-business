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
    [KnownType(typeof(WordCaptchaHtml))]
    public class WordCaptchaHtml : BindingElementBase
    {
        public WordCaptchaHtml() { }

        public WordCaptchaHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId, isFormReadOnly)
        {
            this.Length = obj["length"].ToIntObj();
            if (this.Length <= 0)
                this.Length = 8;
            this.FillValue();
        }


        [DataMember]
        public int Length { get; set; }
        private void FillValue()
        {
            if (this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.OnPost)
            {
                this.Value = this.Helper?.ListFormQueryModel?.FirstOrDefault(c => c.Key == this.Id)?.Value.ToStringObj();
                this.IsValid(this.Helper.ResultOperation);
            }
        }

        public override void IsValid(ResultOperation resultOperation)
        {
            if (!this.Validate())
            {
                resultOperation.AddError("کاربر گرامی کد امنیتی نا معتبر میباشد.");
            }
        }

        private bool Validate()
        {
            return !string.IsNullOrWhiteSpace(System.Web.HttpContext.Current.Session[this.Id].ToStringObj()) &&
                 System.Web.HttpContext.Current.Session[this.Id].ToStringObj() == this.Value.ToStringObj();
        }
    }
}
