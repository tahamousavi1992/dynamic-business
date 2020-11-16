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
    public class FormModel
    {
        public FormModel() { }
        public FormModel(JObject obj, HtmlElementHelperModel helper, List<sysBpmsStep> listStep, sysBpmsStep currentStep, sysBpmsDynamicForm dynamicForm, bool isFormReadOnly)
        {
            helper?.AddScript(dynamicForm?.ConfigXmlModel.OnLoadFunctionBody);
            helper?.AddStyleSheet(dynamicForm?.ConfigXmlModel.StyleSheetCode);
            this.ContentHtml = new ContentHtml(obj, helper, dynamicForm.ID, isFormReadOnly);
            if (helper?.FormAction == HtmlElementHelperModel.e_FormAction.Onload||
                helper?.FormAction == HtmlElementHelperModel.e_FormAction.Preview)
                this.FillDependentControls();
            this.SetParams(listStep, currentStep);
            this.FormName = dynamicForm?.Name ?? string.Empty;
            this.IsFormReadOnly = isFormReadOnly;
            this.IsEncrypted = helper.IsEncrypted;
            this.DynamicFormID = dynamicForm?.ID ?? Guid.Empty;
            this.ResultOperation = helper?.ResultOperation ?? new ResultOperation();
            this.ContentHtml.CheckAllVisibility(this);
        }

        /// <summary>
        /// because dependent controls , ListItemElementBase , dont fill thier options until all controls are loaded.
        /// </summary>
        private void FillDependentControls()
        {
            List<QueryModel> listFormQueryModel = null;
            foreach (object item in this.ContentHtml.FindDependentControls())
            {
                //COMBOSEARCH does not need to fill in onLoad form action.
                if (((ElementBase)item).Type != ElementBase.e_Type.COMBOSEARCH.ToString())
                {
                    listFormQueryModel = new List<QueryModel>();
                    if (item is BindingElementBase)
                    {
                        ((BindingElementBase)item).ListDependentParameterModel.ForEach(c =>
                           {
                               listFormQueryModel.Add(new QueryModel()
                               {
                                   Key = c.Name,
                                   Value = ((BindingElementBase)this.ContentHtml.FindControlByID(c.ControlID))?.Value
                               });
                           });
                        ((BindingElementBase)item).FillData(listFormQueryModel);
                    }
                }

            }
        }

        private void SetParams(List<sysBpmsStep> listStep, sysBpmsStep currentStep)
        {
            this.HasSubmitButton = this.ContentHtml.Rows.Any(r => (r is RowHtml ? ((RowHtml)r).Columns : ((AccordionHtml)r).GetListColumn()).Any(d => d.children.Any(f => f is ButtonHtml && ((ButtonHtml)f).subtype == ButtonHtml.e_subtype.submit)));

            //if this is call from pop up form which does not have stepID.
            if (listStep != null && currentStep != null)
            {
                listStep = listStep.OrderBy(c => c.Position).ToList();
                this.StepID = currentStep.ID;
                this.StepName = currentStep.Name;
                this.IsMultiStep = listStep.Count > 1;
                this.IsLasStep = listStep.LastOrDefault().ID == currentStep.ID;
                this.IsFirstStep = listStep.FirstOrDefault().ID == currentStep.ID;
                if (this.IsMultiStep && this.HasSubmitButton)
                {
                    //set IsMultiStep and IsLastStep into all submit buttons.
                    foreach (ButtonHtml item in this.ContentHtml.FindControlByType<ButtonHtml>().Where(c => c.subtype == ButtonHtml.e_subtype.submit))
                    {
                        item.IsMultiStep = true;
                        item.IsLastStep = this.IsLasStep;
                    }
                }
                if (this.IsMultiStep && !this.IsLasStep)
                    this.NextStepID = listStep.FirstOrDefault(c => c.Position > currentStep.Position).ID;
                if (this.IsMultiStep && !this.IsFirstStep)
                    this.PreviousStepID = listStep.LastOrDefault(c => c.Position < currentStep.Position).ID;
            }
        }
        public ResultOperation ResultOperation { get; set; }
        [DataMember]
        public ContentHtml ContentHtml { get; set; }
        [DataMember]
        public string FormName { get; set; }
        [DataMember]
        public bool IsFormReadOnly { get; set; }
        [DataMember]
        public bool HasSubmitButton { get; set; }
        [DataMember]
        public bool IsMultiStep { get; private set; }
        [DataMember]
        public bool IsLasStep { get; private set; }
        [DataMember]
        public bool IsFirstStep { get; private set; }
        [DataMember]
        public Guid StepID { get; private set; }
        [DataMember]
        public Guid? NextStepID { get; private set; }
        [DataMember]
        public Guid? PreviousStepID { get; private set; }
        [DataMember]
        public string StepName { get; set; }
        [DataMember]
        public bool IsEncrypted { get; set; }
        [DataMember]
        public Guid DynamicFormID { get; set; }
    }
}
