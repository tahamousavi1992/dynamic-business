using DotNetNuke.Entities.Portals;
using DotNetNuke.Web.Api;
using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace DynamicBusiness.BPMS.SingleAction.Controllers
{
    [System.Web.Http.AllowAnonymous]
    public class BpmsGeneralController : DnnApiController
    {

        protected HttpRequest MyRequest
        {
            get
            {
                return HttpContext.Current.Request;
            }
        }

        [HttpGet]
        public System.Net.Http.HttpResponseMessage GetData(string controller, string action, string formToken = "")
        {
            if (FormTokenUtility.ValidateFormToken(formToken, HttpContext.Current.Session.SessionID))
            {
                //when calling main bpms api from client application, there  is no need to pass formToken to main bpms api.
                string url = UrlUtility.GetApiUrl(SingleActionHomeController.Setting.WebApiAddress, action, controller, "", this.GetParameters().ToArray());
                return ApiUtility.GetData(url, SingleActionHomeController.Setting.WebServicePass, base.UserInfo.Username, ApiUtility.GetIPAddress(), HttpContext.Current.Session.SessionID, FormTokenUtility.GetIsEncrypted(formToken, HttpContext.Current.Session.SessionID));
            }
            else
                throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.Unauthorized);
        }

        [HttpPost]
        public System.Net.Http.HttpResponseMessage PostData(string controller, string action, string formToken = "")
        {

            if (FormTokenUtility.ValidateFormToken(formToken, HttpContext.Current.Session.SessionID))
            {
                //when calling main api from client application, there  is no need to pass formToken to main bpms api.
                string url = UrlUtility.GetApiUrl(SingleActionHomeController.Setting.WebApiAddress, action, controller, "", this.GetParameters().ToArray());
                return ApiUtility.PostData(url, QueryModel.GetFormDataList(this.MyRequest).ToList(), SingleActionHomeController.Setting.WebServicePass, base.UserInfo.Username, ApiUtility.GetIPAddress(), HttpContext.Current.Session.SessionID, FormTokenUtility.GetIsEncrypted(formToken, HttpContext.Current.Session.SessionID));
            }
            else
                throw new System.Web.Http.HttpResponseException(System.Net.HttpStatusCode.Unauthorized);
        }

        private string[] GetParameters()
        {
            List<string> Params = new List<string>();
            foreach (var item in this.MyRequest.QueryString.Keys)
            {
                if (!QueryModel.ForbidenList.Where(c => c != "controlid").Contains(item.ToStringObj().ToLower()))
                    Params.Add(item.ToString() + "=" + this.MyRequest.QueryString[item.ToString()].Trim());
            }
            return Params.ToArray();
        }

    }
}