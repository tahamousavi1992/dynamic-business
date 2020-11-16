using DotNetNuke.Services.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public class LangUtility
    {
        public const string root = "~/DesktopModules/MVC/DynamicBusiness.Bpms/SharedPresentation/";
        public static string Get(string name, string modelName) { return Localization.GetString(name, LangUtility.GetResx(modelName)); }

        private static string GetResx(string modelName)
        {
            string strResxControl = string.Empty;

            if (DomainUtility.GetCulture() != string.Empty &&
             System.IO.File.Exists(LangUtility.root + Localization.LocalResourceDirectory + "/" + modelName + "." + DomainUtility.GetCulture() + ".resx"))
            {
                strResxControl = LangUtility.root + Localization.LocalResourceDirectory + "/" + modelName + "." + DomainUtility.GetCulture() + ".resx";
            }
            else
            {
                strResxControl = LangUtility.root + Localization.LocalResourceDirectory + "/" + modelName + ".resx";
            }
            return strResxControl;
        }

    }
 

}
