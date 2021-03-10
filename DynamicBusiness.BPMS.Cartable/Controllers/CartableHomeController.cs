using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Web.Api;
using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DynamicBusiness.BPMS.Cartable.Controllers
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.ViewPermissions)]
    public class CartableHomeController : KartableControllerBase
    {
        //It retrieves open threads which are in person kartable or step.
        public ActionResult Index()
        {
            ViewBag.DnnMenu = ViewUtility.GetMenu(this.ActivePage, base.PortalSettings.PortalId);
            ViewBag.CartableUrl = UrlUtility.GetApiBase(base.Request, base.PortalSettings.DefaultPortalAlias, "BpmsCartableApi").TrimEnd('/') + "/";
            ViewBag.EngineUrl = UrlUtility.GetApiBase(base.Request, base.PortalSettings.DefaultPortalAlias, "BpmsApi").TrimEnd('/') + "/";
            ViewBag.rootPage = this.Request.RawUrl.Substring(0, this.Request.RawUrl.IndexOf("/" + this.ActivePage.TabName) + this.ActivePage.TabName.Length + 2);
            ViewBag.LoginUrl = ViewUtility.LoginUrl();
            ViewBag.SignOutUrl = DotNetNuke.Common.Globals.NavigateURL("LogOff");
            ViewBag.UserFullName = base.User?.DisplayName;
            ViewBag.ShowUserPanelWithNoSkin = base.ShowUserPanelWithNoSkin;
            ViewBag.LoadUserPanelJquery = base.LoadUserPanelJquery;
            ViewBag.LoadUserPanelBootstrap = base.LoadUserPanelBootstrap;
            base.AddUserIfNotExist();
            DomainUtility.CartableHomeUr = base.ActivePage.FullUrl;
            return View();
        }
    }
}