using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IUrlCodeHelper
    {
        RedirectUrlModel RedirectUrlModel { get; set; }
        void RedirectUrl(string url, bool NewTab = false);
        void RedirectForm(object applicationPageId, bool NewTab, params string[] parameters);
        void RedirectForm(object applicationPageId, params string[] parameters);
        string GetParameter(string ParameterName);
    }
}
