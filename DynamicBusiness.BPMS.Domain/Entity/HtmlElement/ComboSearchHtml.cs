using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    [KnownType(typeof(ComboSearchHtml))]
    public class ComboSearchHtml : ListItemElementBase
    {
        public ComboSearchHtml()
        {

        }
        public ComboSearchHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId, isFormReadOnly)
        {
            this.IsRequired = obj["isRequired"] != null && (bool)obj["isRequired"];
            this.FontIconCssClass = DomainUtility.toString(obj["fontIconCssClass"]);
            this.FillValue();
        }
        [DataMember]
        public bool IsRequired { get; set; }
        [DataMember]
        public string FontIconCssClass { get; set; }
        [DataMember]
        public string TextOfSelectedValue { get; set; }

        private void FillValue()
        {
            if (this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.Onload ||
                this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.Preview)
            {
                this.Value = StringCipher.EncryptFormValues(this.Helper.DataManageHelper.GetValueByBinding(this.Fill).ToStringObj(), base.Helper.ApiSessionId, base.Helper.IsEncrypted);

                //Set TextOfSelectedValue
                if (string.IsNullOrWhiteSpace(this.Value.ToStringObj()))
                    this.TextOfSelectedValue = string.Empty;
                else
                {
                    string varName = this.FillList.Split(',').FirstOrDefault();
                    this.TextOfSelectedValue = this.Helper.DataManageHelper.GetEntityWithKeyValue(varName, new Dictionary<string, object>() { { this.Key, this.Value } })?[this.Text].ToStringObj();
                }
            }
            else
                if (this.Helper?.FormAction != HtmlElementHelperModel.e_FormAction.FillMode)
                this.Value = this.Helper?.ListFormQueryModel?.FirstOrDefault(c => c.Key == this.Id)?.Value.ToStringObj() ?? "";


        }

        public override void FillData(List<QueryModel> listFormQueryModel = null)
        {
            this.Options = this.Options ?? new List<OptionHtml>();
            foreach (DataModel item in this.Helper.DataManageHelper.GetEntityByBinding(this.FillList, listFormQueryModel, null, $"select * from ({{0}}) as _bpmstd where {this.Text} like N'%'+@query+'%' ").Items)
            {
                this.Options.Add(new OptionHtml()
                {
                    Label = item[this.Text].ToStringObj(),
                    Value = StringCipher.EncryptFormValues(item[this.Key].ToStringObj(), base.Helper.ApiSessionId, base.Helper.IsEncrypted),
                });
            }
        }


    }
}
