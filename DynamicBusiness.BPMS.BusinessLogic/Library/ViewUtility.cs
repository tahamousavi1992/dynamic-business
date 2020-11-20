
using DotNetNuke.Entities.Tabs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public static class ViewUtility
    {

        public static List<string> LoadStyleFiles()
        {
            string directoryFolder = BPMSResources.FilesRoot + BPMSResources.StyleSheetRoot;
            string directoryVirtualFolder = (BPMSResources.FilesVirtualRoot + BPMSResources.StyleSheetRoot);
            if (System.IO.Directory.Exists(directoryFolder))
            {
                return new DirectoryInfo(directoryFolder).GetFiles("*.css").Select(c => "~/" + directoryVirtualFolder + "/" + c.Name).ToList();
            }
            else return new List<string>();
        }

        public static string LoginUrl()
        {
            string returnUrl = HttpContext.Current.Request.RawUrl;
            if (returnUrl.IndexOf("?returnurl=", StringComparison.Ordinal) != -1)
            {
                returnUrl = returnUrl.Substring(0, returnUrl.IndexOf("?returnurl=", StringComparison.Ordinal));
            }
            returnUrl = HttpUtility.UrlEncode(returnUrl);
            return DotNetNuke.Common.Globals.LoginURL(returnUrl, (HttpContext.Current.Request.QueryString["override"] != null));
        }

        public static string GetMenu(TabInfo currentTab, int portalId)
        {
            List<(string name, string url, int id, int parentId)> tmp = new List<(string, string, int, int)>();
            IDictionaryEnumerator hs = TabController.Instance.GetTabsByPortal(portalId).GetEnumerator();
            while (hs.MoveNext())
            {
                TabInfo tab = (TabInfo)hs.Entry.Value;
                if (tab.TabID != currentTab.TabID && !tab.IsDeleted && tab.IsVisible && tab.ParentId == -1 && DotNetNuke.Security.Permissions.TabPermissionController.CanViewPage(tab))
                    tmp.Add((tab.TabName, tab.FullUrl, tab.TabID, tab.ParentId));
            }
            return new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(tmp.Select(c => new
            {
                c.name,
                c.url,
                c.id,
                c.parentId
            }).ToList());
        }

    }
}
