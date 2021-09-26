using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
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

namespace DynamicBusiness.BPMS.Cartable
{
    public class BpmsFormTokenAuth : AuthorizeAttributeBase, IOverrideDefaultAuthLevel
    {
        public override bool IsAuthorized(AuthFilterContext context)
        {
            using (APIAccessService apiAccessService = new APIAccessService())
            {
                return DomainUtility.IsTestEnvironment ? true :
                    FormTokenUtility.ValidateFormToken(context.ActionContext.RequestContext.Url.Request.GetHttpContext().Request.QueryString[FormTokenUtility.FormToken], HttpContext.Current.Session.SessionID);
            }
        }
    }
}