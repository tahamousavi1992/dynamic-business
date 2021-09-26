using DotNetNuke.Services.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public class SharedLang
    {
        public const string root = "~/DesktopModules/MVC/DynamicBusiness.Bpms/SharedPresentation/";
        public static string Get(string name) { return DomainUtility.IsTestEnvironment ? name : Localization.GetString(name, SharedLang.GetResx()); }
        public static string GetReuired(string name, string modelName) { return string.Format(SharedLang.Get("FormatRequired.Text"), LangUtility.Get(name, modelName)); }
        private static string GetResx()
        {
            string strResxControl;
            if (DomainUtility.GetCulture() != string.Empty)
            {
                strResxControl = LangUtility.root + Localization.LocalResourceDirectory + "/Shared" + "." + DomainUtility.GetCulture() + ".resx";
            }
            else
            {
                strResxControl = LangUtility.root + Localization.LocalResourceDirectory + "/Shared" + ".resx";
            }
            return strResxControl;
        }

    }
}
