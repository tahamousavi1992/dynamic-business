using DotNetNuke.Web.Mvc.Framework.Controllers;
using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using System;

namespace DynamicBusiness.BPMS
{
    public abstract class BpmsControllerBase : DnnController
    {
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            this.ViewBag.UserInfo = base.User;
            this.ViewBag.ModuleContext = ModuleContext;
            ViewBag.ProcessId = Request.QueryString["processId"]?.ToGuidObjNull() ?? Request.Form["processId"]?.ToGuidObjNull();
            ViewBag.ApplicationPageId = Request.QueryString["applicationPageId"]?.ToGuidObjNull() ?? Request.Form["applicationPageId"]?.ToGuidObjNull();
            ViewData["PortalAlias"] = base.PortalSettings.DefaultPortalAlias;
            Session["dt"] = DateTime.Now.Date;
            this.RedirectNoSkin();
        }

        protected void RedirectNoSkin()
        {
            if (!this.Request.Url.ToStringObj().Contains("SkinSrc") && !string.IsNullOrWhiteSpace(UrlUtility.NoSkinPath))
                this.Response.Redirect(UrlUtility.MakeNoSkin(this.Request.Url.ToStringObj()));
        }
    }
}
