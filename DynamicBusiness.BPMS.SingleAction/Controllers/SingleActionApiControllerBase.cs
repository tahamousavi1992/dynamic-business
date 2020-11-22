using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DynamicBusiness.BPMS.SingleAction
{
    [System.Web.Http.AllowAnonymous]
    public class SingleActionApiControllerBase : DnnApiController
    {
        protected string ApiSessionId { get { return HttpContext.Current.Session.SessionID; } }
        protected HttpRequest MyRequest
        {
            get
            {
                return HttpContext.Current.Request;
            }
        }
        protected string ClientUserName { get { return DomainUtility.IsTestEnvironment() ? "bpms_expert" : UserInfo?.Username; } }
        protected string GetRedirectUrl(RedirectUrlModel redirectModel)
        {
            if (redirectModel != null)
            {
                if (redirectModel.ApplicationPageId.HasValue)
                    return UrlUtility.GetSingleActionUrl("GetAppPageIndex",
                        (redirectModel.ListParameter ?? new List<string>()).
                        Union(new List<string>() { $"applicationPageId={redirectModel.ApplicationPageId}" }).ToArray());
                else
                    return redirectModel.Url;
            }
            else
                return null;
        }
    }
}