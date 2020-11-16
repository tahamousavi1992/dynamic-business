using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using DotNetNuke.Collections;
using DotNetNuke.Security;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Entities.Portals;

namespace DynamicBusiness.BPMS.SingleAction.Controllers
{
    public class SingleActionHomeController : SingleActionControllerBase
    {
        public static string BaseUrl { get; set; }
        public static string WebServicePass { get; set; }
        public static Guid? DefaultApplicationPageID { get; set; }
        public static Guid? DefaultProcessID { get; set; }
        public static bool ShowCardBody { get; set; }
        public static Guid? ProcessEndFormID { get; set; }

        public ActionResult Index()
        {
            BaseUrl = PortalController.GetPortalSetting(SingleActionSettingDTO.e_SettingType.SingleAction_WebApiAddress.ToString(), base.PortalSettings.PortalId, string.Empty);
            WebServicePass = PortalController.GetPortalSetting(SingleActionSettingDTO.e_SettingType.SingleAction_WebServicePass.ToString(), base.PortalSettings.PortalId, string.Empty);
            DefaultApplicationPageID = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault(SingleActionSettingDTO.e_SettingType.SingleAction_ApplicationPageID.ToString(), string.Empty).ToGuidObjNull();
            DefaultProcessID = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault(SingleActionSettingDTO.e_SettingType.SingleAction_ProcessID.ToString(), string.Empty).ToGuidObjNull();
            ShowCardBody = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault(SingleActionSettingDTO.e_SettingType.SingleAction_ShowCardBody.ToString(), string.Empty).ToStringObj().ToLower() == "true";
            ProcessEndFormID = ModuleContext.Configuration.ModuleSettings.GetValueOrDefault(SingleActionSettingDTO.e_SettingType.SingleAction_ProcessEndFormID.ToString(), string.Empty).ToGuidObjNull();
            DomainUtility.SingleActionHomeUr = base.ActivePage.FullUrl;
            if (string.IsNullOrWhiteSpace(BaseUrl) || (!DefaultProcessID.HasValue && !DefaultApplicationPageID.HasValue))
            {
                ViewBag.Message = "Setting is not complete";
                return View();
            }
            return View();
        }

    }
}