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
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DynamicBusiness.BPMS.EngineApi
{
    public class BpmsAuth : AuthorizeAttributeBase, IOverrideDefaultAuthLevel
    {
        public override bool IsAuthorized(AuthFilterContext context)
        {
            if (DomainUtility.IsTestEnvironment)
                return true;
            using (APIAccessService apiAccessService = new APIAccessService())
            {
                //when a client is calling main api ,they have to put token,which is password, in header named token
                if (context.ActionContext.Request.Headers.Contains("token"))
                { 
                    return apiAccessService.HasAccess(ApiUtility.GetIPAddress(), context.ActionContext.Request.Headers.GetValues("token").FirstOrDefault());
                }
                else
                {
                    if (AccessUtility.CalledByLocalSA(HttpContext.Current.Request))
                    {
                        //it is called from single action module in same server with same ip.
                        return true;
                    }
                    else
                    {
                        //when bpms user panel is calling engine api,every request should have formToken in its parameters.
                        string formToken = context.ActionContext.RequestContext.Url.Request.GetHttpContext().Request.QueryString[FormTokenUtility.FormToken];
                        return FormTokenUtility.ValidateFormToken(formToken, HttpContext.Current.Session.SessionID);
                    }
                }
            }
        }

    }
}