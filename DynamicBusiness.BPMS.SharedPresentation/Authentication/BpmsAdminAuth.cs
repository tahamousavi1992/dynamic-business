using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Web.Api;
using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    public class BpmsAdminAuth : AuthorizeAttributeBase, IOverrideDefaultAuthLevel
    {
        public const string AuthModuleFriendlyName = "BPMS Admin Panel";
        public string Permission { get; set; }
        public override bool IsAuthorized(AuthFilterContext context)
        {
            using (APIAccessService apiAccessService = new APIAccessService())
            {
                if (DomainUtility.IsTestEnvironment)
                    return true;
                else
                {
                    ModuleController mc = new ModuleController();

                    ModuleInfo mi = mc.GetModuleByDefinition(PortalController.Instance.GetCurrentPortalSettings().PortalId, AuthModuleFriendlyName);
                    return ModulePermissionController.CanViewModule(mi);
                }
            }
        }
    }
}