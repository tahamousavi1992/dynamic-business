using DynamicBusiness.BPMS.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using DynamicBusiness.BPMS.Domain;
using DotNetNuke.Web.Mvc.Routing;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public static class UrlUtility
    {
        public static string GetApiBase(HttpRequestBase request, string PortalAlias, string apiName)
        {
            string defaultUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/');
            PortalAlias = PortalAlias.Replace("http://", "").Replace("https://", "");
            return defaultUrl + (PortalAlias.Split('/').Length == 2 ? ("/" + PortalAlias.Split('/')[1]) : "") + "/API/" + apiName;
        }

        public static string GetCartableApiUrl(HttpRequest request, string PortalAlias, string actionName, string controllerName, string formToken, params string[] parameters)
        {
            string baseUrl = UrlUtility.GetApiBase(new HttpRequestWrapper(request), PortalAlias, "BpmsCartableApi");

            baseUrl += "/" + controllerName.TrimStringEnd("Controller");
            baseUrl += string.IsNullOrWhiteSpace(formToken) ? $"/{actionName}" : $"/{actionName}?formToken={formToken}";
            if (parameters != null)
            {
                if (!string.IsNullOrWhiteSpace(formToken))
                    parameters = parameters.Where(c => c.Split('=').FirstOrDefault() != "formToken").ToArray();
                parameters = parameters.Where(c => !string.IsNullOrWhiteSpace(c.Split('=').LastOrDefault())).ToArray();
            }
            return baseUrl + (parameters == null || parameters.Count() == 0 ? "" : ((baseUrl.Contains("?") ? "&" : "?") + string.Join("&", parameters.Select(c => c))));
        }

        public static string GetSingleActionApiUrl(HttpRequest request, string PortalAlias, string actionName, string controllerName, string formToken, params string[] parameters)
        {
            string baseUrl = UrlUtility.GetApiBase(new HttpRequestWrapper(request), PortalAlias, "BpmsSingleActionApi");

            baseUrl += "/" + controllerName.TrimStringEnd("Controller");
            baseUrl += string.IsNullOrWhiteSpace(formToken) ? $"/{actionName}" : $"/{actionName}?formToken={formToken}";
            if (parameters != null)
            {
                if (!string.IsNullOrWhiteSpace(formToken))
                    parameters = parameters.Where(c => c.Split('=').FirstOrDefault() != "formToken").ToArray();
                parameters = parameters.Where(c => !string.IsNullOrWhiteSpace(c.Split('=').LastOrDefault())).ToArray();
            }
            return baseUrl + (parameters == null || parameters.Count() == 0 ? "" : ((baseUrl.Contains("?") ? "&" : "?") + string.Join("&", parameters.Select(c => c))));
        }

        public static string GetSingleActionUrl(string pageName, params string[] parameters)
        {
            if (parameters != null)
            {
                parameters = parameters.Where(c => !string.IsNullOrWhiteSpace(c.Split('=').LastOrDefault())).Distinct().ToArray();
            }
            string url = DomainUtility.SingleActionHomeUr.TrimEnd('/') + "/" + pageName;
            url = ((url.Contains("?") || parameters == null || !parameters.Any() || parameters.Any(c => c.Contains("/"))) ? url : (url + "?"));
            return url + (parameters == null || parameters.Count() == 0 ? "" : ((url.Contains("?") ? "" : "/") + string.Join((url.Contains("?") ? "&" : "/"), parameters.Select(c => (url.Contains("?") ? c : c.Replace("=", "/").Trim())))));
        }

        public static string GetCartableUrl(string pageName, params string[] parameters)
        {
            if (parameters != null)
            {
                parameters = parameters.Where(c => !string.IsNullOrWhiteSpace(c.Split('=').LastOrDefault())).Distinct().ToArray();
            }
            string url = DomainUtility.CartableHomeUr.TrimEnd('/') + "/" + pageName;
            url = ((url.Contains("?") || parameters == null || !parameters.Any() || parameters.Any(c => c.Contains("/"))) ? url : (url + "?"));
            return url + (parameters == null || parameters.Count() == 0 ? "" : ((url.Contains("?") ? "" : "/") + string.Join((url.Contains("?") ? "&" : "/"), parameters.Select(c => (url.Contains("?") ? c : c.Replace("=", "/").Trim())))));
        }

        /// <param name="formToken">when calling main api from client application, there  is no need to pass formToken to main bpms api.</param>
        public static string GetApiUrl(HttpRequestBase request, string PortalAlias, string actionName, string controllerName, string formToken, params string[] parameters)
        {
            string defaultUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/');
            PortalAlias = PortalAlias.Replace("http://", "").Replace("https://", "");
            string baseUrl = defaultUrl + (PortalAlias.Split('/').Length == 2 ? ("/" + PortalAlias.Split('/')[1]) : "") + "/API/BpmsApi";

            baseUrl += "/" + controllerName.TrimStringEnd("Controller");
            baseUrl += string.IsNullOrWhiteSpace(formToken) ? $"/{actionName}" : $"/{actionName}?formToken={formToken}";
            if (parameters != null)
            {
                if (!string.IsNullOrWhiteSpace(formToken))
                    parameters = parameters.Where(c => c.Split('=').FirstOrDefault() != "formToken").ToArray();
                parameters = parameters.Where(c => !string.IsNullOrWhiteSpace(c.Split('=').LastOrDefault())).ToArray();
            }
            return baseUrl + (parameters == null || parameters.Count() == 0 ? "" : ((baseUrl.Contains("?") ? "&" : "?") + string.Join("&", parameters.Select(c => c))));
        }

        /// <param name="formToken">when calling main api from client application, there  is no need to pass formToken to main bpms api.</param>
        public static string GetApiUrl(string apiBaseUrl, string actionName, string controllerName, string formToken, params string[] parameters)
        {
            string baseUrl = apiBaseUrl.Trim('/') + "/API/BpmsApi";

            baseUrl += "/" + controllerName.TrimStringEnd("Controller");
            baseUrl += string.IsNullOrWhiteSpace(formToken) ? $"/{actionName}" : $"/{actionName}?formToken={formToken}";

            if (parameters != null)
            {
                parameters = parameters.Where(c => !string.IsNullOrWhiteSpace(c.Split('=').LastOrDefault())).ToArray();
            }
            return baseUrl + (parameters == null || parameters.Count() == 0 ? "" : ((baseUrl.Contains("?") ? "&" : "?") + string.Join("&", parameters.Select(c => c))));

        }

        public static string GetParams(HttpRequestBase httpRequest)
        {
            string Params = string.Empty;
            foreach (var item in httpRequest.QueryString.Keys)
            {
                if (!QueryModel.ForbidenList.Contains(item.ToStringObj().ToLower()) && item.ToString() != "PageIndex" && !Params.Contains(item.ToString() + "="))
                    Params += item.ToString() + "=" + httpRequest.QueryString[item.ToString()] + "&";
            }
            foreach (var item in httpRequest.Form.Keys)
            {
                if (!QueryModel.ForbidenList.Contains(item.ToStringObj().ToLower()) && item.ToString() != "PageIndex" && !Params.Contains(item.ToString() + "="))
                    Params += item.ToString() + "=" + httpRequest.Form[item.ToString()] + "&";
            }
            return Params.TrimEnd('&');
        }

        /// <summary>
        /// this method remove repetitive parameters like skinsrc,containersrc,language,controller,action,moduleid,tabid,amp;skinsrc, controlId
        /// </summary>
        public static List<string> GetParamsAsArray(HttpRequestBase httpRequest, params string[] additionalParams)
        {
            List<string> Params = new List<string>();
            if (additionalParams != null)
            {
                foreach (string item in additionalParams)
                {
                    Params.Add(item);
                }
            }

            foreach (var item in httpRequest.QueryString.Keys)
            {
                if (!QueryModel.ForbidenList.Contains(item.ToStringObj().ToLower()) && item.ToString() != "PageIndex" && !Params.Any(c => c.StartsWith(item.ToString() + "=")))
                    Params.Add(item.ToString() + "=" + httpRequest.QueryString[item.ToString()].Trim());
            }

            foreach (var item in httpRequest.Form.Keys)
            {
                if (!QueryModel.ForbidenList.Contains(item.ToStringObj().ToLower()) && item.ToString() != "PageIndex" && !Params.Any(c => c.StartsWith(item.ToString() + "=")))
                    Params.Add(item.ToString() + "=" + httpRequest.Form[item.ToString()].Trim());
            }

            return Params;
        }

        public static string AddParamsToUrl(string url, string key, string value)
        {
            string param = key + "=" + value;
            return url + (url.Contains("?") ? ("&" + param) : ("?" + param));
        }

        public static string MakeNoSkin(string Url)
        {
            if (string.IsNullOrWhiteSpace(UrlUtility.NoSkinPath))
            {
                UrlUtility.NoSkinPath = "SkinSrc=[G]" + DotNetNuke.Common.Globals.QueryStringEncode(DotNetNuke.UI.Skins.SkinController.RootSkin + "/" + DotNetNuke.Common.Globals.glbHostSkinFolder + "/" + "No Skin");
            }
            Url = Url.Contains("SkinSrc") && !string.IsNullOrWhiteSpace(UrlUtility.NoSkinPath) ? Url : (Url.Contains("?") ? Url + "&" + UrlUtility.NoSkinPath : Url + "?" + UrlUtility.NoSkinPath);
            Url = Url.Contains("ContainerSrc") ? Url : !string.IsNullOrWhiteSpace(UrlUtility.NoContainerPath) ? (Url.Contains("?") ? Url + "&" + UrlUtility.NoContainerPath : Url + "?" + UrlUtility.NoContainerPath) : Url;
            return Url;
        }

        public static string NoContainerPath = new SettingValueService().GetValue(sysBpmsSettingDef.e_NameType.NoContainerPath.ToString());
        public static string NoSkinPath = new SettingValueService().GetValue(sysBpmsSettingDef.e_NameType.NoSkinPath.ToString());
    }
}