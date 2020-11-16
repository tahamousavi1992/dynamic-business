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
            DomainUtility.CartableHomeUr = base.ActivePage.FullUrl;
            return View();
        }
    }
}