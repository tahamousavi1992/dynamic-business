using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ControlCodeHelper : IControlCodeHelper
    {
        public FormModel FormModel { get; set; }
        public ControlCodeHelper(FormModel formModel)
        {
            this.FormModel = formModel;
        }

        public void SetValue(string controlId, object value)
        {
            object calculatedValue;
            if (value is VariableModel)
            {
                calculatedValue = ((VariableModel)value).Value;
            }
            else
                calculatedValue = value;

            BindingElementBase bindingElementBase = (BindingElementBase)this.FormModel.ContentHtml.FindControlByID(controlId);
            if (bindingElementBase != null)
                bindingElementBase.SetValue(calculatedValue);
            else throw new Exception($"{controlId} is not found.");
        }
        public void BindDataSource(string controlId, object value)
        {
            BindingElementBase bindingElementBase = (BindingElementBase)this.FormModel.ContentHtml.FindControlByID(controlId);
            if (bindingElementBase != null)
                bindingElementBase.BindDataSource(value);
            else
                throw new Exception($"{controlId} is not found.");
        }

        public object GetValue(string controlId)
        {
            BindingElementBase bindingElementBase = (BindingElementBase)this.FormModel.ContentHtml.FindControlByID(controlId);
            if (bindingElementBase != null)
            {
                if (BindingElementBase.e_Type.FILEUPLOAD.ToString() == bindingElementBase.Type)
                {
                    return (((Dictionary<Guid, object>)bindingElementBase.Value).ToDictionary(c => c.Key.ToGuidObj(), c => c.Value == null ? null : c.Value is System.Web.HttpPostedFileBase ? (System.Web.HttpPostedFileBase)c.Value :
                    new System.Web.HttpPostedFileWrapper((System.Web.HttpPostedFile)c.Value))).Values.FirstOrDefault();

                }
                return bindingElementBase.Value;
            }
            throw new Exception($"{controlId} is not found.");
        }

        public void SetVisibility(string controlId, bool visibility)
        {
            ElementBase elementBase = (ElementBase)this.FormModel.ContentHtml.FindControlByID(controlId);
            if (elementBase != null)
            {
                elementBase.SetVisibility(visibility);
            }
            else throw new Exception($"{controlId} is not found.");
        }
    }
}
