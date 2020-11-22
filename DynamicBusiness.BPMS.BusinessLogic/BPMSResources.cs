using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.HtmlControls;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public static class BPMSResources
    {
        #region [[ Constants ]]
        public static string FilesRoot { get { return DotNetNuke.Entities.Portals.PortalController.Instance.GetCurrentPortalSettings()?.HomeDirectoryMapPath?.Trim('\\') + "\\DynamicBusiness.Bpms\\"; } }
        public static string FilesVirtualRoot { get { return DotNetNuke.Entities.Portals.PortalController.Instance.GetCurrentPortalSettings().HomeDirectory.Trim('/') + "/DynamicBusiness.Bpms/"; } }
        public const string Root = "~/DesktopModules/MVC/DynamicBusiness.Bpms/";
        public const string ResourcesRoot = Root + "Resources/assets/";
        public const string ResourcesFormGenerator = ResourcesRoot + "Form Generator/";
        public const string ScriptResourcesFormGenerator = ResourcesFormGenerator + "JS/";
        public const string HtmlEditorRoot = ResourcesRoot + "CKEditor/";
        public const string TemplatesRoot = ResourcesRoot + "Templates/";
        public const string ImageResourcesRoot = ResourcesRoot + "Images/";
        public const string Repository = ResourcesRoot + "Repository/";
        public const string SqlDataProvider = Repository + "SqlDataProvider/"; 
        public const string ScriptResourcesRoot = ResourcesRoot + "Js/";
        public const string StyleResourcesRoot = ResourcesRoot + "Styles/";
        public const string ReportResourcesRoot = ResourcesRoot + "Reports/";
        public const string SmsResourceRoot = ResourcesRoot + "SMS/";
        public const string EmailResourceRoot = ResourcesRoot + "Email/";
        public const string ReportResourceRoot = ResourcesRoot + "Report/";
        public const string StyleSheetRoot = "StyleSheet";
        public const string JavaScriptRoot = "JavaScript";
        public const string AssemblyRoot = "bin";
        public const string AssemblyCompiledRoot = "compiled";
 
        #endregion
 
    }
}
