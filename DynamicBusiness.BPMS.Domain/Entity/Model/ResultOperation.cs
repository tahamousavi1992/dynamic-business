using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DynamicBusiness.BPMS.Domain
{
    public class ResultOperation
    {
        public object CurrentObject { get; set; }
        public ResultOperation() { ValidationResult = new CustomValidationResult(); }
        public ResultOperation(CustomValidationResult validationResult)
        {
            ValidationResult = validationResult;
        }
        public ResultOperation(object currentObject)
        {
            ValidationResult = new CustomValidationResult();
            this.CurrentObject = currentObject;
        }
        protected CustomValidationResult ValidationResult { get; set; }
        public bool IsSuccess { get { return this.HasError != true && !ValidationResult.ErrorList.Any(); } }
        private bool? HasError { get; set; }
        public string GetErrors()
        {
            return string.Join("<br>", ValidationResult.ErrorList);
        }
        public void AddError(string errorText)
        {
            ValidationResult.AddError(errorText);
        }
        public void SetHasError()
        {
            this.HasError = true;
        }
    }
}
