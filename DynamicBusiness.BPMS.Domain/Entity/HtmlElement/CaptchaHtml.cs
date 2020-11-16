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
    [KnownType(typeof(CaptchaHtml))]
    public class CaptchaHtml : BindingElementBase
    {
        public CaptchaHtml() { }
        public CaptchaHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId, isFormReadOnly)
        {
            this.SiteKey = DomainUtility.toString(obj["sitekey"]);
            this.PrivateKey = DomainUtility.toString(obj["privatekey"]);
            this.Language = DomainUtility.toString(obj["language"]);
            this.FillValue();
        }

        [DataMember]
        public string SiteKey { get; set; }
        [DataMember]
        public string PrivateKey { get; set; }
        [DataMember]
        public string Language { get; set; }
        private void FillValue()
        {
            if (this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.OnPost)
            {
                this.Value = this.Validate();
                this.IsValid(this.Helper.ResultOperation);
            }
        }
        public override void IsValid(ResultOperation resultOperation)
        {
            if (!this.Value.ToBoolObj())
            {
                resultOperation.AddError("کاربر گرامی کد امنیتی نا معتبر میباشد.");
            }
        }
        private bool Validate()
        {
            string EncodedResponse = this.Helper?.ListFormQueryModel?.FirstOrDefault(c => c.Key == this.Id)?.Value.ToStringObj();

            if (string.IsNullOrEmpty(EncodedResponse) || string.IsNullOrEmpty(this.PrivateKey))
                return false;
            //by pass certificate validation
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            var client = new System.Net.WebClient();
            var GoogleReply = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", this.PrivateKey, EncodedResponse));
            return JObject.Parse(GoogleReply).GetValue("success").ToStringObj().ToLower() == "true";
        }
    }
}
