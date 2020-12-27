using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [KnownType(typeof(ButtonHtml))]
    [DataContract]
    public class ButtonHtml : ElementBase
    {
        public ButtonHtml() { }
        public ButtonHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId)
        {
            this.IsReadonly = isFormReadOnly;
            this.subtype = (e_subtype)Enum.Parse(typeof(e_subtype), DomainUtility.toString(obj["subtype"]), true);
            this.BackendCoding = DomainUtility.toString(obj["backendCoding"]).FromBase64();

            this.OpenFormId = obj["openFormId"].ToStringObj();
            this.OpenFormParameter = obj["openFormParameter"].ToStringObj();
            this.OpenFormCallBackScript = obj["openFormCallBackScript"].ToStringObj().FromBase64();

            //confirm setting
            this.ConfirmText = obj["confirmText"].ToStringObj();
            this.HasConfirm = obj["hasConfirm"].ToStringObj().ToLower() == "true";
            this.HasExpressionConfirm = obj["hasExpressionConfirm"].ToStringObj().ToLower() == "true";
            this.ExpressionConfirmText = obj["expressionConfirmText"].ToStringObj();
            this.ExpressionConfirmCode = obj["expressionConfirmCode"].ToStringObj().FromBase64();
            if (!string.IsNullOrWhiteSpace(this.ExpressionConfirmCode))
                this.ConfirmDesignCodeModel = DesignCodeUtility.GetDesignCodeFromXml(this.ExpressionConfirmCode);

            this.ExpressionConfirmHasFalseAction = obj["expressionConfirmHasFalseAction"].ToStringObj().ToLower() == "true";
            this.FormWidth = obj["formWidth"].ToStringObj();
            this.FormHeight = obj["formHeight"].ToStringObj();

            if (!string.IsNullOrWhiteSpace(this.OpenFormId))
            {
                if (this.Helper?.FormAction != HtmlElementHelperModel.e_FormAction.FillMode &&
                this.Helper?.FormAction != HtmlElementHelperModel.e_FormAction.Preview)
                {
                    base.AddElementEventModel(new ElementEventScriptModel()
                    {
                        EventName = ElementBase.e_EventName.click.ToString(),
                        FunctionName = "openForm" + this.Id,
                        ScriptBody = $@"function openForm{this.Id}(target){{ FormControl.openFormPopUp(target.element,'{this.OpenFormId}','{this.GetParameter}',function(){{ {this.OpenFormCallBackScript} }},{(string.IsNullOrWhiteSpace(this.FormWidth) ? "null" : this.FormWidth)},{(string.IsNullOrWhiteSpace(this.FormHeight) ? "null" : this.FormHeight)});}}",
                    }, true);
                }
            }
            this.ValidationGroup = string.IsNullOrWhiteSpace(obj["validationGroup"].ToStringObj()) ? "nextAction" : obj["validationGroup"].ToStringObj().Trim();

            this.HasPostBack = this.subtype == ButtonHtml.e_subtype.submit ||
                (DesignCodeUtility.GetDesignCodeFromXml(this.BackendCoding).CodeObjects?.Any() ?? false);
        }
        [DataMember]
        public bool IsReadonly { get; set; }
        [DataMember]
        public string BackendCoding { get; set; }
        [DataMember]
        public string OpenFormId { get; set; }
        [DataMember]
        public string FormWidth { get; set; }
        [DataMember]
        public string FormHeight { get; set; }
        [DataMember]
        public string OpenFormParameter { get; set; }
        [DataMember]
        public string OpenFormCallBackScript { get; set; }
        [DataMember]
        public string ValidationGroup { get; set; }
        //confirm setting
        [DataMember]
        public bool HasConfirm { get; set; }
        [DataMember]
        public string ConfirmText { get; set; }
        [DataMember]
        public bool HasExpressionConfirm { get; set; }
        [DataMember]
        public string ExpressionConfirmText { get; private set; }
        [DataMember]
        public string ExpressionConfirmCode { get; set; }
        [DataMember]
        public DesignCodeModel ConfirmDesignCodeModel { get; set; }
        [DataMember]
        public bool ExpressionConfirmHasFalseAction { get; set; }

        [DataMember]
        public bool HasPostBack { get; private set; }
        /// <summary>
        /// it is filled by FormModel
        /// </summary>
        [DataMember]
        public bool? IsMultiStep { get; set; }
        /// <summary>
        /// it is filled by FormModel
        /// </summary>
        [DataMember]
        public bool? IsLastStep { get; set; }
        [DataMember]
        public string GetParameter
        {
            get
            {
                string paramValue = string.Empty;
                string parameters = this.OpenFormParameter;
                foreach (string param in parameters.Split(','))
                {
                    if (!string.IsNullOrWhiteSpace(parameters))
                    {
                        string value = string.Empty;
                        string setValue = param.Split(':')[2];
                        switch (param.Split(':')[1])
                        {
                            case "2"://variable
                                value = this.Helper.DataManageHelper.GetValueByBinding(setValue).ToStringObj();
                                break;
                            case "3"://static
                                value = setValue;
                                break;
                            case "4"://control
                                value = $"[{setValue}]";
                                break;
                        }
                        paramValue += $",{param.Split(':')[0]}={value}";
                    }
                }
                paramValue = paramValue.Trim(',');
                return paramValue;
            }
            set { }
        }
        [DataMember]
        public e_subtype subtype { get; set; }
        [DataMember]
        public string SubTypeName { get { return this.subtype.ToString(); } set { } }
        public enum e_subtype
        {
            button = 0,
            submit = 1,
            reset = 2,
        }

    }
}
