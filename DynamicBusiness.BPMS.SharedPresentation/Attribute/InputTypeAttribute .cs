using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.SharedPresentation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class InputTypeAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly TextBoxHtml.e_TextBoxType TextBoxType;

        public InputTypeAttribute(TextBoxHtml.e_TextBoxType textBoxType)
        {
            this.TextBoxType = textBoxType;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return ValidationResult.Success;
        }
         
        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var modelClientValidationRule = new ModelClientValidationRule
            {
                ValidationType = TextBoxType.ToString().ToLower(),
                ErrorMessage = ErrorMessage
            };
            return new List<ModelClientValidationRule> { modelClientValidationRule };
        }
    }
}