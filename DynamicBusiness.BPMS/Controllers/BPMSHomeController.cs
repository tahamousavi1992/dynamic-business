using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Web.Api;
using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DynamicBusiness.BPMS.Controllers
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.ViewPermissions)]
    public class BPMSHomeController : BpmsControllerBase
    {
        //
        // GET: /BPMSHome/
        public ActionResult Index()
        {
            DomainUtility.AdminHomeUr = base.ActivePage.FullUrl;
            ViewBag.DnnMenu = ViewUtility.GetMenu(this.ActivePage, base.PortalSettings.PortalId);
            ViewBag.AdminUrl = UrlUtility.GetApiBase(base.Request, base.PortalSettings.DefaultPortalAlias, "BpmsAdminApi").TrimEnd('/') + "/";
            ViewBag.EngineUrl = UrlUtility.GetApiBase(base.Request, base.PortalSettings.DefaultPortalAlias, "BpmsApi").TrimEnd('/') + "/";
            ViewBag.rootPage = this.Request.RawUrl.Substring(0, this.Request.RawUrl.IndexOf("/" + this.ActivePage.TabName) + this.ActivePage.TabName.Length + 2);
            ViewBag.LoginUrl = ViewUtility.LoginUrl();
            ViewBag.SignOutUrl = DotNetNuke.Common.Globals.NavigateURL("LogOff");
            ViewBag.UserFullName = base.User?.DisplayName;

            return View();
        }

        public PartialViewResult LoadFormGenerator()
        {
            return PartialView();
        }
    }
}