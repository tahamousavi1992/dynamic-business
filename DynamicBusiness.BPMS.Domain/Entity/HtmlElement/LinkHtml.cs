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
    [KnownType(typeof(LinkHtml))]
    public class LinkHtml : BindingElementBase
    {
        public LinkHtml() { }
        public LinkHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId, isFormReadOnly)
        {
            this.Width = !string.IsNullOrWhiteSpace(DomainUtility.toString(obj["width"])) ? Convert.ToInt32(obj["width"]) : (int?)null;
            this.Height = !string.IsNullOrWhiteSpace(DomainUtility.toString(obj["height"])) ? Convert.ToInt32(obj["height"]) : (int?)null;
            this.Address = DomainUtility.toString(obj["address"]);
            this.LinkType = DomainUtility.toString(obj["linkType"]);

            this.FillValue();
        }
        [DataMember]
        public int? Width { get; set; }
        [DataMember]
        public int? Height { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        public string LinkType { get; set; }
        [DataMember]
        public string Href
        {
            get
            {
                if (this.LinkType == "dnnModal")
                    return "javascript:void(dnnModal.show('" + this.Address + "?popUp=true'" + ",true" + "," + this.Height + "," + this.Width + ",true))";
                else return this.Address;
            }
            private set { }
        }
        [DataMember]
        public string Target
        {
            get
            {
                return this.LinkType == "newTab" ? "_blank" : "";
            }
            private set { }
        }
        private void FillValue()
        {
            if (this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.Onload ||
                this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.Preview)
            {
                this.Value = this.Helper.DataManageHelper.GetValueByBinding(this.Fill);
                if (!string.IsNullOrWhiteSpace(this.Value.ToStringObj()))
                    this.Address = this.Value.ToStringObj();
            }
            else
            {
                if (this.Helper?.FormAction != HtmlElementHelperModel.e_FormAction.FillMode)
                {
                    this.Value = this.Helper?.ListFormQueryModel?.FirstOrDefault(c => c.Key == this.Id)?.Value.ToStringObj() ?? "";
                    if (!string.IsNullOrWhiteSpace(this.Value.ToStringObj()))
                        this.Address = this.Value.ToStringObj();
                }
            }
        }
    }
}
