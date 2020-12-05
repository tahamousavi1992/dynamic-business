
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Installer.Packages;
using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
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
                return new DirectoryInfo(directoryFolder).GetFiles("*.css").Select(c => "/" + directoryVirtualFolder + "/" + c.Name).ToList();
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


        /// <summary>
        /// maintained last sql script was executed.
        /// </summary>
        private static string LastSqlVersionExecuted { get; set; }
        /// <summary>
        /// maintained last sql script was executed.
        /// </summary>
        private static string CurrentBpmsVersion { get; set; }

        public static bool IsUpdatedSqlQueryVesrion(int portalId)
        {
            if (string.IsNullOrWhiteSpace(CurrentBpmsVersion))
            {
                PackageInfo packageInfo = PackageController.Instance.GetExtensionPackage(portalId, (p) => { return p.Name == "DynamicBusiness.BPMS"; });
                CurrentBpmsVersion = packageInfo.Version.Major.ToString("D2") + "." + packageInfo.InstalledVersion.Minor.ToString("D2") + "." + packageInfo.InstalledVersion.Build.ToString("D2");
            }
            if (string.IsNullOrWhiteSpace(LastSqlVersionExecuted))
            {
                using (SettingValueService settingValueService = new SettingValueService())
                {
                    try
                    {
                        LastSqlVersionExecuted = settingValueService.GetValue(sysBpmsSettingDef.e_NameType.LastSqlUpdatedVersion.ToString());
                        if (string.IsNullOrWhiteSpace(LastSqlVersionExecuted))
                            LastSqlVersionExecuted = "00.00.00";
                    }
                    catch
                    {
                        //error because it is first time and there is no table.
                        LastSqlVersionExecuted = "00.00.00";
                    }

                }
            }

            return CurrentBpmsVersion == LastSqlVersionExecuted;
        }

        //It used used for built in upgrade system for now it is not in work.
        public static void UpdatedSqlQuery(int portalId)
        {
            if (!IsUpdatedSqlQueryVesrion(portalId))
            {
                using (DataBaseQueryService dataBaseQueryService = new DataBaseQueryService())
                {
                    List<string> queries = new List<string>();
                    string XslFile = System.Web.Hosting.HostingEnvironment.MapPath(BPMSResources.SqlDataProvider);
                    foreach (string fileName in Directory.GetFiles(XslFile, "*.SqlDataProvider").Select(Path.GetFileNameWithoutExtension))
                    {
                        if (fileName != "Uninstall.SqlDataProvider" && fileName.CompareTo(LastSqlVersionExecuted) == 1)
                        {
                            queries.AddRange(File.ReadAllText(XslFile + fileName + ".SqlDataProvider").Split(new string[1] { "GO" }, StringSplitOptions.None).Where(c => !string.IsNullOrWhiteSpace(c)).ToList());
                        }
                    }
                    dataBaseQueryService.UpdatedSqlQuery(queries);

                    using (SettingValueService settingValueService = new SettingValueService())
                    {
                        using (SettingDefService settingDefService = new SettingDefService())
                        {
                            sysBpmsSettingDef def = settingDefService.GetInfo(sysBpmsSettingDef.e_NameType.LastSqlUpdatedVersion.ToString());
                            sysBpmsSettingValue settingValue = settingValueService.GetList(null, "", def.ID).LastOrDefault();
                            if (settingValue == null)
                            {
                                settingValue = new sysBpmsSettingValue();
                                settingValue.SettingDefID = def.ID;
                                settingValue.SetDate = DateTime.Now;
                                settingValue.UserName = "System";
                                settingValue.Value = CurrentBpmsVersion;
                                settingValueService.Add(settingValue);
                            }
                            else
                            {
                                settingValue.SetDate = DateTime.Now;
                                settingValue.Value = CurrentBpmsVersion;
                                settingValueService.Update(settingValue);
                            }
                            LastSqlVersionExecuted = CurrentBpmsVersion;
                            UrlUtility.NoSkinPath = settingValueService.GetValue(sysBpmsSettingDef.e_NameType.NoSkinPath.ToString());
                        }
                    }
                }
            }
        }
    }
}
