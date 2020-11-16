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
    [KnownType(typeof(ImageHtml))]
    public class ImageHtml : BindingElementBase
    {
        public ImageHtml() { }
        public ImageHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId, isFormReadOnly)
        {
            this.Width = obj["width"] != null && !string.IsNullOrWhiteSpace(obj["width"].ToString()) ? Convert.ToInt32(obj["width"]) : (int?)null;
            this.Height = obj["height"] != null && !string.IsNullOrWhiteSpace(obj["height"].ToString()) ? Convert.ToInt32(obj["height"]) : (int?)null;
            this.Address = DomainUtility.toString(obj["address"]);

            this.FillValue();
        }
        [DataMember]
        public int? Width { get; set; }
        [DataMember]
        public int? Height { get; set; }
        [DataMember]
        public string Address { get; set; }
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
            this.Value = this.Helper.DataManageHelper.GetValueByBinding(this.Fill, listFormQueryModel);
            if (!string.IsNullOrWhiteSpace(this.Value.ToStringObj()))
                this.Address = this.Value.ToStringObj();
        }
    }
}
