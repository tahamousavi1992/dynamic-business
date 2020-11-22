using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DynamicBusiness.BPMS.SingleAction
{
    public class SingleActionControllerBase : DnnController
    { 
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        { 
            base.Initialize(requestContext);
            this.ViewBag.PortalAlias = base.PortalSettings.DefaultPortalAlias;
            this.ViewBag.ModuleContext = ModuleContext;
            Session["dt"] = DateTime.Now.Date;
            DotNetNuke.Framework.JavaScriptLibraries.JavaScript.RequestRegistration(DotNetNuke.Framework.JavaScriptLibraries.CommonJs.DnnPlugins);
        }
 
    }
}