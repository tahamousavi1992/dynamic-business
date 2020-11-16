using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.SharedPresentation
{
    public static class BpmsCombo
    {
        public static MvcHtmlString BpmsComboSearch(this HtmlHelper helper, string name, string value, string text, string sourceUrl, object htmlAttributes = null)
        {

            string RealName = name;

            //htmlAttributes
            string attributes = string.Empty;
            if (htmlAttributes != null)
            {
                var _htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                foreach (string key in _htmlAttributes.Keys)
                {
                    attributes += " " + key + "='" + _htmlAttributes[key] + "'";
                }
            }
            string OutPut = $@"<select id='{RealName}' name='{RealName}' {attributes} data-isComboSearch='true' data-sourceUrl={sourceUrl}>";
            if (!string.IsNullOrWhiteSpace(value))
            {
                OutPut += $@"<option value='{value}' selected> {text} </option>";
            }
            OutPut += "</select>";
            return new MvcHtmlString(OutPut);
        }

        public static MvcHtmlString BpmsComboSearchFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> ex, string text, string sourceUrl, object htmlAttributes = null)
        {

            var metadata = ModelMetadata.FromLambdaExpression(ex, helper.ViewData);
            string RealName = ex.Body.ToString().Remove(0, ex.Body.ToString().IndexOf('.') + 1);
            IDictionary<string, object> validationAttributes = helper.
                            GetUnobtrusiveValidationAttributes
                            (metadata.PropertyName, metadata);
            Dictionary<string, string> Validations = new Dictionary<string, string>();
            foreach (string key in validationAttributes.Keys)
            {
                Validations.Add(key, validationAttributes[key].ToString());
            }

            //htmlAttributes
            string attributes = string.Empty;
            if (htmlAttributes != null)
            {
                var _htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                foreach (string key in _htmlAttributes.Keys)
                {
                    attributes += " " + key + "='" + _htmlAttributes[key] + "'";
                }
            }
            string OutPut = $@"<select id='{RealName}'  name='{RealName}' {attributes} data-isComboSearch='true' data-sourceUrl={sourceUrl}>";
            string DataValue = metadata.Model.ToStringObj();
            if (!string.IsNullOrWhiteSpace(DataValue))
            {
                OutPut += $@"<option value='{DataValue}' selected> {text} </option>";
            }

            OutPut += "</select>";
            return new MvcHtmlString(OutPut);
        }
    }
}