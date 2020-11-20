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

namespace DynamicBusiness.BPMS.SharedPresentation
{
    public class KartableControllerBase : DnnController
    {
        public bool ShowUserPanelWithNoSkin { get; set; }
        public bool LoadUserPanelJquery { get; set; }
        public bool LoadUserPanelBootstrap { get; set; }
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            this.ViewBag.PortalAlias = base.PortalSettings.DefaultPortalAlias;
            this.ViewBag.ModuleContext = ModuleContext;
            ViewBag.ProcessId = Request.QueryString["processId"];
            Session["dt"] = DateTime.Now.Date;

            using (SettingValueService settingValueService = new SettingValueService())
            {
                this.ShowUserPanelWithNoSkin = settingValueService.GetValue(sysBpmsSettingDef.e_NameType.ShowUserPanelWithNoSkin.ToString()).ToLower() == "true";
                this.LoadUserPanelJquery = settingValueService.GetValue(sysBpmsSettingDef.e_NameType.LoadUserPanelJquery.ToString()).ToLower() == "true";
                this.LoadUserPanelBootstrap = settingValueService.GetValue(sysBpmsSettingDef.e_NameType.LoadUserPanelBootstrap.ToString()).ToLower() == "true";
            }

            if (!this.Request.Url.ToStringObj().Contains("SkinSrc") &&
                !string.IsNullOrWhiteSpace(UrlUtility.NoSkinPath) &&
                this.ShowUserPanelWithNoSkin)
                this.Response.Redirect(UrlUtility.MakeNoSkin(this.Request.Url.ToStringObj()));
        }

        protected void AddUserIfNotExist()
        {
            using (SettingValueService settingValueService = new SettingValueService())
            {
                bool addUser = settingValueService.GetValue(sysBpmsSettingDef.e_NameType.AddUserAutomatically.ToString()).ToLower() == "true";
                using (UserService userService = new UserService())
                {
                    sysBpmsUser sysBpmsUser = userService.GetInfo(base.User.Username);
                    if (addUser && !string.IsNullOrWhiteSpace(base.User?.Username) && sysBpmsUser == null)
                    {
                        sysBpmsUser = userService.GetInfoByEmail(base.User.Email);
                        if (sysBpmsUser != null)
                        {
                            sysBpmsUser.Username = base.User.Username;
                            userService.Update(sysBpmsUser, null);
                        }
                        else
                        {
                            sysBpmsUser = new sysBpmsUser(Guid.Empty, base.User.Username, base.User.FirstName, base.User.LastName, base.User.Email, base.User.Profile.Telephone, base.User.Profile.Cell);
                            userService.Add(sysBpmsUser, null);
                        }
                    }
                }
            }
        }

    }
}
