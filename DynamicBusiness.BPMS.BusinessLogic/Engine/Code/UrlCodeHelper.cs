using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class UrlCodeHelper : IUrlCodeHelper
    {
        private List<QueryModel> BaseQueryModel { get; set; }
        public RedirectUrlModel RedirectUrlModel { get; set; }
        public UrlCodeHelper(List<QueryModel> baseQueryModel)
        {
            this.BaseQueryModel = baseQueryModel;
        }

        public void RedirectUrl(string url, bool NewTab = false)
        {
            RedirectUrlModel = new RedirectUrlModel(url, NewTab);
        }

        public void RedirectForm(object applicationPageId, bool NewTab, params string[] parameters)
        {
            RedirectUrlModel = new RedirectUrlModel(applicationPageId.ToGuidObj(), (parameters ?? new string[] { }).ToList(), NewTab);
        }

        public void RedirectForm(object applicationPageId, params string[] parameters)
        {
            RedirectUrlModel = new RedirectUrlModel(applicationPageId.ToGuidObj(), (parameters ?? new string[] { }).ToList(), false);
        }

        public string GetParameter(string ParameterName)
        {
            if (this.BaseQueryModel != null)
            {
                if (this.BaseQueryModel.Any(c => c.Key == ParameterName))
                    return this.BaseQueryModel.FirstOrDefault(c => c.Key == ParameterName).Value?.ToString();
            }
            return null;
        }
    }
}
