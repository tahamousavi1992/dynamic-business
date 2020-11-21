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
        public static SingleActionSettingDTO Setting { get; set; }
        public ActionResult Index()
        {
            Setting = new SingleActionSettingDTO();
            Setting.GetFromModule(Request, base.PortalSettings.PortalId, ModuleContext.Configuration.ModuleSettings);
            DomainUtility.SingleActionHomeUr = base.ActivePage.FullUrl;

            ViewBag.LoadUserPanelJquery = Setting.LoadjQuery;
            ViewBag.LoadUserPanelBootstrap = Setting.LoadBootstrap;

            ViewBag.SingleActionUrl = UrlUtility.GetApiBase(base.Request, base.PortalSettings.DefaultPortalAlias, "BpmsSingleActionApi").TrimEnd('/') + "/";
            ViewBag.rootPage = this.Request.RawUrl.Substring(0, this.Request.RawUrl.IndexOf("/" + this.ActivePage.TabName) + this.ActivePage.TabName.Length + 1);

            if ((!Setting.ProcessID.HasValue && !Setting.ApplicationPageID.HasValue))
            {
                ViewBag.Message = "Setting is not complete";
                return View();
            }
            return View();
        }

    }
}