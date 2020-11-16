using DotNetNuke.Web.Mvc.Framework.Controllers;
using DynamicBusiness.BPMS.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DynamicBusiness.BPMS.SharedPresentation;

namespace DynamicBusiness.BPMS.Controllers
{
    public class BPMSHomeController : BpmsControllerBase
    {
        //
        // GET: /BPMSHome/
        public ActionResult Index()
        {
            ViewBag.rootPage = this.Request.RawUrl.Substring(0, this.Request.RawUrl.IndexOf("/" + this.ActivePage.TabName + "/") + this.ActivePage.TabName.Length + 1);
            return View();
        }  
    
        public PartialViewResult LoadFormGenerator()
        {
            return PartialView();
        }
	}
}