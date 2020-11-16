using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DynamicBusiness.BPMS.Cartable.Controllers
{
    public class CartableHomeController : KartableControllerBase
    {
        //It retrieves open threads which are in person kartable or step.
        public ActionResult Index()
        {
            ViewBag.CartableUrl = UrlUtility.GetApiBase(base.Request, base.PortalSettings.DefaultPortalAlias, "BpmsCartableApi").TrimEnd('/') + "/";
            ViewBag.EngineUrl = UrlUtility.GetApiBase(base.Request, base.PortalSettings.DefaultPortalAlias, "BpmsApi").TrimEnd('/') + "/";
            ViewBag.rootPage = this.Request.RawUrl.Substring(0, this.Request.RawUrl.IndexOf("/" + this.ActivePage.TabName + "/") + this.ActivePage.TabName.Length + 1);
            base.AddUserIfNotExist();
            DomainUtility.CartableHomeUr = base.ActivePage.FullUrl;
            return View();
        }
    }
}