using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Authentication;
using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DynamicBusiness.BPMS.Cartable.Controllers
{
    public class CartableHomeController : KartableControllerBase
    {
        //It retrieves open threads which are in person kartable or step.
        public ActionResult Index()
        {
            List<(string name, string url, int id, int parentId)> tmp = new List<(string, string, int, int)>();
            IDictionaryEnumerator hs = TabController.Instance.GetTabsByPortal(base.PortalSettings.PortalId).GetEnumerator();
            while (hs.MoveNext())
            {
                TabInfo tab = (TabInfo)hs.Entry.Value;
                if (tab.TabName != this.ActivePage.TabName && !tab.IsDeleted && tab.IsVisible && tab.ParentId == -1 && DotNetNuke.Security.Permissions.TabPermissionController.CanViewPage(tab))
                    tmp.Add((tab.TabName, tab.FullUrl, tab.TabID, tab.ParentId));
            }
            ViewBag.DnnMenu = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(tmp.Select(c => new
            {
                c.name,
                c.url,
                c.id,
                c.parentId
            }).ToList());
            ViewBag.CartableUrl = UrlUtility.GetApiBase(base.Request, base.PortalSettings.DefaultPortalAlias, "BpmsCartableApi").TrimEnd('/') + "/";
            ViewBag.EngineUrl = UrlUtility.GetApiBase(base.Request, base.PortalSettings.DefaultPortalAlias, "BpmsApi").TrimEnd('/') + "/";
            ViewBag.rootPage = this.Request.RawUrl.Substring(0, this.Request.RawUrl.IndexOf("/" + this.ActivePage.TabName + "/") + this.ActivePage.TabName.Length + 1);
            ViewBag.LoginUrl = ViewUtility.LoginUrl();
            ViewBag.SignOutUrl = DotNetNuke.Common.Globals.NavigateURL("LogOff");
            ViewBag.UserFullName = base.User?.DisplayName;
            base.AddUserIfNotExist();
            DomainUtility.CartableHomeUr = base.ActivePage.FullUrl;
            return View();
        }
    }
}