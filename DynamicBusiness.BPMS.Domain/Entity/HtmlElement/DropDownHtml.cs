﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
namespace DynamicBusiness.BPMS.Domain
{
    [KnownType(typeof(DropDownHtml))]
    [DataContract]
    public class DropDownHtml : ListItemElementBase
    {
        public DropDownHtml()
        {

        }
        public DropDownHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId, isFormReadOnly)
        {
            this.Options = new List<OptionHtml>();
            this.IsRequired = obj["isRequired"] != null && (bool)obj["isRequired"];
            this.FontIconCssClass = DomainUtility.toString(obj["fontIconCssClass"]); 
            this.OptionalCaption = obj["optionalCaption"].ToStringObj();
            this.FillValue(obj);
        }
        [DataMember]
        public bool IsRequired { get; set; }
        [DataMember]
        public string FontIconCssClass { get; set; }
 
        [DataMember]
        public string OptionalCaption { get; set; }

        private void FillValue(JObject obj)
        {
            if (this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.Onload ||
                this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.Preview)
            {
                if (this.Helper.DataManageHelper != null)
                {
                    if (!string.IsNullOrWhiteSpace(this.Fill))
                        this.Value = this.Helper.DataManageHelper.GetValueByBinding(this.Fill).ToStringObjNull();
                    if (!string.IsNullOrWhiteSpace(this.FillList))
                    {
                        this.Options.Clear();
                        if (string.IsNullOrWhiteSpace(this.Parameter))
                        {
                            this.FillData();
                        }
                    }
                    else
                    {
                        this.Options = obj["options"].Select(c => new OptionHtml((JObject)c)).ToList();
                        if (this.Options.Any(c => c.Value == this.Value.ToStringObj()))
                            this.Options.ForEach(c => c.Selected = c.Value == this.Value.ToStringObj());
                    }
                }
            }
            else
            {
                if (this.Helper?.FormAction != HtmlElementHelperModel.e_FormAction.FillMode)
                    this.Value = this.Helper?.ListFormQueryModel?.FirstOrDefault(c => c.Key == this.Id)?.Value.ToStringObj() ?? "";
            }
        }

        public override void FillData(List<QueryModel> listFormQueryModel = null)
        {
            this.Helper?.DataManageHelper?.ClearVariable(this.FillList);
            foreach (DataModel item in this.Helper.DataManageHelper.GetEntityByBinding(this.FillList, listFormQueryModel).Items)
            {
                this.Options.Add(new OptionHtml()
                {
                    Label = item[this.Text].ToStringObj(),
                    Value = StringCipher.EncryptFormValues(item[this.Key].ToStringObj(), base.Helper.ApiSessionId, base.Helper.IsEncrypted),
                    Selected = this.Value.ToStringObj() == item[this.Key].ToStringObj(),
                });
            }
        }

        public override void BindDataSource(object value)
        {

            
            foreach (DataModel item in ((value is DataTable ? ((VariableModel)(DataTable)value) : ((VariableModel)value))?.Items ?? new List<DataModel>()))
            {
                this.Options.Add(new OptionHtml()
                {
                    Label = item[this.Text].ToStringObj(),
                    Value = StringCipher.EncryptFormValues(item[this.Key].ToStringObj(), base.Helper.ApiSessionId, base.Helper.IsEncrypted),
                    Selected = this.Value.ToStringObj() == item[this.Key].ToStringObj(),
                });
            }
        }
    }
}
