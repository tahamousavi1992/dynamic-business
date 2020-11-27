using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    public class ElementBase
    {
        public ElementBase()
        {

        }

        public ElementBase(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId)
        {
            this.DynamicFormID = dynamicFormId;
            this.Id = DomainUtility.toString(obj["id"]);
            this.Type = DomainUtility.toString(obj["type"]);
            this.CssClass = DomainUtility.toString(obj["cssClass"]);
            this.HtmlType = DomainUtility.toString(obj["htmlType"]);
            this.ExpressionVisibilityCode = obj["expressionVisibilityCode"].ToStringObj().FromBase64();
            if (!string.IsNullOrWhiteSpace(this.ExpressionVisibilityCode))
            {
                this.VisibilityDesignCodeModel = DesignCodeUtility.GetDesignCodeFromXml(this.ExpressionVisibilityCode);
            }

            this.Label = DomainUtility.toString(obj["label"]);
            this.HelpMessageText = DomainUtility.toString(obj["helpMessageText"]);
            this.AccessType = !string.IsNullOrWhiteSpace(DomainUtility.toString(obj["accessType"])) ?
              (e_AccessType)Enum.Parse(typeof(e_AccessType), DomainUtility.toString(obj["accessType"]), true) : (e_AccessType?)null;

            this.Helper = _helper;
            this.FillElementEventModel(obj);
        }

        [DataMember]
        public Guid DynamicFormID { get; set; }
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string CssClass { get; set; }
        [DataMember]
        public DesignCodeModel VisibilityDesignCodeModel { get; set; }
        [DataMember]
        public string ExpressionVisibilityCode { get; private set; }
        [DataMember]
        public string HtmlType { get; set; }
        [DataMember]
        public string Label { get; set; }
        [DataMember]
        public string HelpMessageText { get; set; }
        [DataMember]
        public e_AccessType? AccessType { get; set; }
        [DataMember]
        public HtmlElementHelperModel Helper { get; set; }
        [DataMember]
        public List<EventDataAttributesModel> EventDataAttributes { get; set; }
        [DataMember]
        public bool? Visibility { get; private set; }

        public void FillElementEventModel(JObject obj)
        {
            if (obj["events"] != null)
            {
                this.EventDataAttributes = new List<EventDataAttributesModel>();
                obj["events"].Select(c => (JObject)c).ToList().ForEach(c =>
                {
                    this.EventDataAttributes.Add(new EventDataAttributesModel()
                    {
                        AttrName = ("data-eventfunction-" + DomainUtility.toString(c["eventName"])),
                        FunctionName = DomainUtility.toString(c["eventFunction"]),
                    });
                    this.Helper.AddScript(DomainUtility.toString(c["eventCode"]));
                });

            }
            else this.EventDataAttributes = new List<EventDataAttributesModel>();
        }

        public void AddElementEventModel(ElementEventScriptModel elementEventScriptModel, bool clear)
        {
            if (elementEventScriptModel != null)
            {
                if (clear)
                    this.EventDataAttributes = new List<EventDataAttributesModel>();
                this.EventDataAttributes = this.EventDataAttributes ?? new List<EventDataAttributesModel>();
                this.EventDataAttributes.Add(new EventDataAttributesModel()
                {
                    AttrName = ("data-eventfunction-" + elementEventScriptModel.EventName),
                    FunctionName = elementEventScriptModel.FunctionName,
                });
                this.Helper.AddScript(elementEventScriptModel.ScriptBody);
            }
        }
        /// <summary>
        /// this method is used only in application pages.
        /// </summary>
        /// <returns></returns>
        public bool CheckAccess()
        {
            return
                   this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.FillMode ||
                   this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.Preview ||
                   (this.Helper?.DataManageHelper?.ProcessID.HasValue ?? false) ||
                   (this.Helper.applicationPageEngine?.CheckUserAccessByForm(this.DynamicFormID, this.AccessType ?? e_AccessType.AllowView) ?? true);
        }

        public bool CheckVisibility(FormModel formModel)
        {
            if (this.Visibility.HasValue)
                return this.Visibility.Value;

            if (this.VisibilityDesignCodeModel != null && !string.IsNullOrWhiteSpace(this.VisibilityDesignCodeModel.Code) &&
                (this.Helper.FormAction == HtmlElementHelperModel.e_FormAction.Onload || this.Helper.FormAction == HtmlElementHelperModel.e_FormAction.Preview))
                this.Visibility = this.Helper.DynamicCodeEngine?.ExecuteBooleanCode(this.VisibilityDesignCodeModel, formModel) ?? true;
            else
                this.Visibility = true;
            return this.Visibility.Value;
        }

        public static object GetElement(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly)
        {
            object element = null;
            switch ((e_Type)Enum.Parse(typeof(e_Type), obj["type"].ToString(), true))
            {
                case e_Type.DROPDOWNLIST:
                    element = new DropDownHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
                case e_Type.TEXTBOX:
                    element = new TextBoxHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
                case e_Type.RADIOBUTTONLIST:
                    element = new RadioButtonListHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
                case e_Type.CHECKBOXLIST:
                    element = new CheckBoxListHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
                case e_Type.CHECKBOX:
                    element = new CheckBoxHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
                case e_Type.IMAGE:
                    element = new ImageHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
                case e_Type.LINK:
                    element = new LinkHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
                case e_Type.CAPTCHA:
                    element = new CaptchaHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
                case e_Type.TITLE:
                    element = new TitleHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
                case e_Type.DATEPICKER:
                    element = new DatePickerHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
                case e_Type.CKEDITOR:
                    element = new CkeditorHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
                case e_Type.HTMLCODE:
                    element = new HtmlCodeHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
                case e_Type.FILEUPLOAD:
                    element = new FileUploadHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
                case e_Type.BUTTON:
                    element = new ButtonHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    if (!((ElementBase)element).CheckAccess())
                        element = null;
                    break;
                case e_Type.DOWNLOADLINK:
                    element = new DownloadLinkHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
                case e_Type.COMBOSEARCH:
                    element = new ComboSearchHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
                case e_Type.DATAGRID:
                    element = new DataGridHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
                case e_Type.FORM:
                    element = new FormHtml(obj, _helper, obj["formId"].ToGuidObj(), isFormReadOnly);
                    break;
                case e_Type.ACCORDION:
                    element = new AccordionHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
                case e_Type.CARD:
                    element = new CardHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
                case e_Type.ROW:
                    element = new RowHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
                case e_Type.CHART:
                    element = new ChartHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
                case e_Type.WORDCAPTCHA:
                    element = new WordCaptchaHtml(obj, _helper, dynamicFormId, isFormReadOnly);
                    break;
            }
            return element;
        }

        public static object JObjectTOObject(JObject obj)
        {
            if (obj == null) return null;
            switch ((e_Type)Enum.Parse(typeof(e_Type), obj[nameof(ElementBase.Type)].ToString(), true))
            {
                case e_Type.DROPDOWNLIST:
                    return obj.ToObject<DropDownHtml>();
                case e_Type.TEXTBOX:
                    return obj.ToObject<TextBoxHtml>();
                case e_Type.RADIOBUTTONLIST:
                    return obj.ToObject<RadioButtonListHtml>();
                case e_Type.CHECKBOXLIST:
                    return obj.ToObject<CheckBoxListHtml>();
                case e_Type.CHECKBOX:
                    return obj.ToObject<CheckBoxHtml>();
                case e_Type.IMAGE:
                    return obj.ToObject<ImageHtml>();
                case e_Type.LINK:
                    return obj.ToObject<LinkHtml>();
                case e_Type.CAPTCHA:
                    return obj.ToObject<CaptchaHtml>();
                case e_Type.TITLE:
                    return obj.ToObject<TitleHtml>();
                case e_Type.DATEPICKER:
                    return obj.ToObject<DatePickerHtml>();
                case e_Type.CKEDITOR:
                    return obj.ToObject<CkeditorHtml>();
                case e_Type.HTMLCODE:
                    return obj.ToObject<HtmlCodeHtml>();
                case e_Type.FILEUPLOAD:
                    return obj.ToObject<FileUploadHtml>();
                case e_Type.BUTTON:
                    return obj.ToObject<ButtonHtml>();
                case e_Type.DOWNLOADLINK:
                    return obj.ToObject<DownloadLinkHtml>();
                case e_Type.COMBOSEARCH:
                    return obj.ToObject<ComboSearchHtml>();
                case e_Type.DATAGRID:
                    return obj.ToObject<DataGridHtml>();
                case e_Type.FORM:
                    FormHtml formHtml = obj.ToObject<FormHtml>();
                    formHtml.ContentHtml.ConvertChildrenToObject();
                    return formHtml;
                case e_Type.ROW:
                    return obj.ToObject<RowHtml>();
                case e_Type.ACCORDION:
                    return obj.ToObject<AccordionHtml>();
                case e_Type.CARD:
                    return obj.ToObject<CardHtml>();
                case e_Type.CHART:
                    return obj.ToObject<ChartHtml>();
                case e_Type.WORDCAPTCHA:
                    return obj.ToObject<WordCaptchaHtml>();
                default: return null;
            }
        }

        public void SetVisibility(bool visibility)
        {
            this.Visibility = visibility;
        }

        public enum e_Type
        {
            DROPDOWNLIST,
            TEXTBOX,
            RADIOBUTTONLIST,
            CHECKBOXLIST,
            CHECKBOX,
            IMAGE,
            LINK,
            CAPTCHA,
            TITLE,
            DATEPICKER,
            CKEDITOR,
            HTMLCODE,
            FILEUPLOAD,
            BUTTON,
            DOWNLOADLINK,
            COMBOSEARCH,
            DATAGRID,
            FORM,
            ACCORDION,
            ROW,
            CARD,
            CHART,
            WORDCAPTCHA,
        }
        public enum e_AccessType
        {
            AllowAdd = 1,
            AllowEdit = 2,
            AllowDelete = 3,
            AllowView = 4,
        }
        public enum e_EventName
        {
            click,
            change,
            blur,
            focus,
        }
    }
}
