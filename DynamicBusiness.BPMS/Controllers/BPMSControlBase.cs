using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

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
            if (!this.Request.IsAjaxRequest() && !this.Request.Url.ToStringObj().Contains("SkinSrc"))
                this.Response.Redirect(UrlUtility.MakeNoSkin(this.Request.Url.ToStringObj()));
        }
    }
}
