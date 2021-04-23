using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using DotNetNuke.Collections;
using DotNetNuke.Security;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DynamicBusiness.BPMS.SharedPresentation;
using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using DotNetNuke.Entities.Portals;

namespace DynamicBusiness.BPMS.SingleAction.Controllers
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [DnnHandleError]
    public class SettingsController : SingleActionControllerBase
    {

        [HttpGet]
        public ActionResult Settings()
        {
            var settings = new SingleActionSettingDTO(base.PortalSettings.PortalId, ModuleContext.Configuration.ModuleSettings);
            try
            {
                if (!string.IsNullOrWhiteSpace(settings.WebApiAddress))
                {
                    settings.ProcessName = settings.ProcessID.HasValue ? new EngineProcessProxy(settings.WebApiAddress, settings.WebServicePass, base.User.Username, ApiUtility.GetIPAddress(), base.Session.SessionID, false).GetInfo(settings.ProcessID.Value)?.Name : "";
                    settings.ProcessEndFormName = settings.ProcessEndFormID.HasValue ? new EngineFormProxy(settings.WebApiAddress, settings.WebServicePass, base.User.Username, ApiUtility.GetIPAddress(), base.Session.SessionID, false).GetInfo(settings.ProcessEndFormID.Value)?.Name : ""; ;
                    settings.ApplicationName = settings.ApplicationPageID.HasValue ? new EngineApplicationProxy(settings.WebApiAddress, settings.WebServicePass, base.User.Username, ApiUtility.GetIPAddress(), base.Session.SessionID, false).GetInfo(settings.ApplicationPageID.Value)?.Name : "";
                }
                else
                {
                    if (settings.ProcessID.HasValue)
                        using (ProcessService processService = new ProcessService())
                            settings.ProcessName = processService.GetInfo(settings.ProcessID.Value).Name;

                    if (settings.ProcessEndFormID.HasValue)
                        using (DynamicFormService dynamicFormService = new DynamicFormService())
                            settings.ProcessEndFormName = dynamicFormService.GetInfo(settings.ProcessEndFormID.Value).Name;

                    if (settings.ApplicationPageID.HasValue)
                        using (DynamicFormService dynamicFormService = new DynamicFormService())
                            settings.ApplicationName = dynamicFormService.GetInfoByPageID(settings.ApplicationPageID.Value).Name;
                }
                if (string.IsNullOrWhiteSpace(settings.ApplicationName) && string.IsNullOrWhiteSpace(settings.ProcessName))
                {
                    settings.ProcessID = null;
                    settings.ApplicationPageID = null;
                }
            }
            catch
            {

            }

            if (!string.IsNullOrWhiteSpace(settings.WebApiAddress))
            {
                ViewBag.ApplicationPageUrl = ApiUtility.GetGeneralApiUrl(base.Request, base.ModuleContext.TabModuleId, base.PortalSettings.DefaultPortalAlias, "GetList", "EngineApplication", FormTokenUtility.GetFormToken(base.Session.SessionID, Guid.Empty, false), true);
                ViewBag.ProcessFormUrl = ApiUtility.GetGeneralApiUrl(base.Request, base.ModuleContext.TabModuleId, base.PortalSettings.DefaultPortalAlias, "GetList", "EngineForm", FormTokenUtility.GetFormToken(base.Session.SessionID, Guid.Empty, false), true);
                ViewBag.ProcessUrl = ApiUtility.GetGeneralApiUrl(base.Request, base.ModuleContext.TabModuleId, base.PortalSettings.DefaultPortalAlias, "GetList", "EngineProcess", FormTokenUtility.GetFormToken(base.Session.SessionID, Guid.Empty, false), true);
            }
            else
            {
                ViewBag.ApplicationPageUrl = UrlUtility.GetApiUrl(base.Request, base.PortalSettings.DefaultPortalAlias, "GetList", "EngineApplication", FormTokenUtility.GetFormToken(base.Session.SessionID, Guid.Empty, false));
                ViewBag.ProcessFormUrl = UrlUtility.GetApiUrl(base.Request, base.PortalSettings.DefaultPortalAlias, "GetList", "EngineForm", FormTokenUtility.GetFormToken(base.Session.SessionID, Guid.Empty, false));
                ViewBag.ProcessUrl = UrlUtility.GetApiUrl(base.Request, base.PortalSettings.DefaultPortalAlias, "GetList", "EngineProcess", FormTokenUtility.GetFormToken(base.Session.SessionID, Guid.Empty, false));
            }

            ViewBag.Url = base.ActivePage.FullUrl + "/controller/Settings/action/UpdatePass";
            return View(settings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supportsTokens"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [DotNetNuke.Web.Mvc.Framework.ActionFilters.ValidateAntiForgeryToken]
        public ActionResult Settings(SingleActionSettingDTO settings)
        {
            PortalController.UpdatePortalSetting(base.PortalSettings.PortalId, SingleActionSettingDTO.e_SettingType.SingleAction_WebApiAddress.ToString(), settings.WebApiAddress);
            PortalController.UpdatePortalSetting(base.PortalSettings.PortalId, SingleActionSettingDTO.e_SettingType.SingleAction_WebServicePass.ToString(), settings.WebServicePass);
            ModuleContext.Configuration.ModuleSettings[SingleActionSettingDTO.e_SettingType.SingleAction_ProcessID.ToString()] = settings.ProcessID.ToStringObj();
            ModuleContext.Configuration.ModuleSettings[SingleActionSettingDTO.e_SettingType.SingleAction_ApplicationPageID.ToString()] = settings.ApplicationPageID.ToStringObj();
            ModuleContext.Configuration.ModuleSettings[SingleActionSettingDTO.e_SettingType.SingleAction_ShowCardBody.ToString()] = settings.ShowCardBody.ToStringObj().ToLower();
            ModuleContext.Configuration.ModuleSettings[SingleActionSettingDTO.e_SettingType.SingleAction_ProcessEndFormID.ToString()] = settings.ProcessEndFormID.ToStringObj();
            ModuleContext.Configuration.ModuleSettings[SingleActionSettingDTO.e_SettingType.SingleAction_Jquery.ToString()] = settings.LoadjQuery.ToStringObj().ToLower();
            ModuleContext.Configuration.ModuleSettings[SingleActionSettingDTO.e_SettingType.SingleAction_Bootstrap.ToString()] = settings.LoadBootstrap.ToStringObj().ToLower();
            ModuleContext.Configuration.ModuleSettings[SingleActionSettingDTO.e_SettingType.SingleAction_AppPageSubmitMessage.ToString()] = settings.ApplicationPageID.HasValue ? settings.AppPageSubmitMessage.ToStringObj() : string.Empty;


            return RedirectToDefaultRoute();
        }

        /// <summary>
        /// It is called from setting view everytime user change the password to get list of processes and pages. 
        /// </summary>
        [HttpPost]
        public bool UpdatePass(SingleActionSettingDTO settings)
        {
            try
            {
                PortalController.UpdatePortalSetting(base.PortalSettings.PortalId, SingleActionSettingDTO.e_SettingType.SingleAction_WebApiAddress.ToString(), settings.WebApiAddress);
                PortalController.UpdatePortalSetting(base.PortalSettings.PortalId, SingleActionSettingDTO.e_SettingType.SingleAction_WebServicePass.ToString(), settings.WebServicePass);
            }
            catch
            {

            }
            return true;
        }

    }
}