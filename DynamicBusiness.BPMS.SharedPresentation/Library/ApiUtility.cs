using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using MultipartDataMediaFormatter;
using MultipartDataMediaFormatter.Infrastructure;
namespace DynamicBusiness.BPMS.SharedPresentation
{
    public static class ApiUtility
    {
        /// <param name="token">it is password given to work with main api server</param>
        /// <param name="userName">it is current user name</param>
        public static T PostData<T>(string url, List<QueryModel> values, string token = "", string userName = "", string clientIP = "", string clientId = "", bool isEncrypted = false)
        {
            HttpClient client = new HttpClient();
            AddDefaultHeader(client, token, userName, clientIP, clientId, isEncrypted);
            using (var form = new MultipartFormDataContent())
            {
                foreach (var item in values)
                {
                    if (item.Value is string)
                        form.Add(new StringContent(item.Value.ToString()), item.Key);
                    else
                    {
                        if (((HttpPostedFileBase)item.Value).InputStream != null)
                        {
                            var byteArrayContent = new StreamContent(((HttpPostedFileBase)item.Value).InputStream);
                            byteArrayContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = ((HttpPostedFileBase)item.Value).FileName, Name = item.Key };
                            byteArrayContent.Headers.ContentType = MediaTypeHeaderValue.Parse(((HttpPostedFileBase)item.Value).ContentType);
                            form.Add(byteArrayContent);
                        }
                    }
                }
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                //The content type sent in the request header that tells the server what kind of response it will accept in return.
                client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.PostAsync(url, form).Result;
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsAsync<T>().Result;
                }
            }

            return default(T);
        }

        /// <param name="token">it is password given to work with main api server</param>
        /// <param name="userName">it is current user name</param>
        public static T GetData<T>(string url, string token = "", string userName = "", string clientIP = "", string clientId = "", bool isEncrypted = false)
        {
            HttpClient client = new HttpClient();
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            AddDefaultHeader(client, token, userName, clientIP, clientId, isEncrypted);

            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<T>().Result;
            }
            return default(T);
        }

        /// <param name="token">it is password given to work with main api server</param>
        /// <param name="userName">it is current user name</param>
        public static HttpResponseMessage PostData(string url, List<QueryModel> values, string token = "", string userName = "", string clientIP = "", string clientId = "", bool isEncrypted = false)
        {
            HttpClient client = new HttpClient();
            AddDefaultHeader(client, token, userName, clientIP, clientId, isEncrypted);
            using (var form = new MultipartFormDataContent())
            {
                foreach (var item in values)
                {
                    if (item.Value is string)
                        form.Add(new StringContent(item.Value.ToString()), item.Key);
                    else
                    {
                        if (((HttpPostedFileBase)item.Value).InputStream != null)
                        {
                            var byteArrayContent = new StreamContent(((HttpPostedFileBase)item.Value).InputStream);
                            byteArrayContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") { FileName = ((HttpPostedFileBase)item.Value).FileName, Name = item.Key };
                            byteArrayContent.Headers.ContentType = MediaTypeHeaderValue.Parse(((HttpPostedFileBase)item.Value).ContentType);
                            form.Add(byteArrayContent);
                        }
                    }
                }
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                return client.PostAsync(url, form).Result;
            }
        }

        /// <param name="token">it is password given to work with main api server</param>
        /// <param name="userName">it is current user name</param>
        public static HttpResponseMessage GetData(string url, string token = "", string userName = "", string clientIP = "", string clientId = "", bool isEncrypted = false)
        {
            HttpClient client = new HttpClient();
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            AddDefaultHeader(client, token, userName, clientIP, clientId, isEncrypted);
            return client.GetAsync(url).Result;
        }

        private static void AddDefaultHeader(HttpClient client, string token = "", string userName = "", string clientIP = "", string clientId = "", bool isEncrypted = false)
        {
            if (!string.IsNullOrWhiteSpace(token))
                client.DefaultRequestHeaders.Add("token", token);

            if (!string.IsNullOrWhiteSpace(token))
                client.DefaultRequestHeaders.Add("userName", userName);

            if (!string.IsNullOrWhiteSpace(token))
                client.DefaultRequestHeaders.Add("clientIP", clientIP);

            if (!string.IsNullOrWhiteSpace(clientId))
                client.DefaultRequestHeaders.Add("clientId", clientId);


            client.DefaultRequestHeaders.Add("isEncrypted", isEncrypted ? "1" : "0");
        }

        public static string GetGeneralApiUrl(HttpRequestBase request, string PortalAlias, string actionName, string controllerName, string formToken, bool isLoader, bool isGetData, params string[] parameters)
        {
            string baseUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/');
            PortalAlias = PortalAlias.Replace("http://", "").Replace("https://", "");
            baseUrl = baseUrl + (PortalAlias.Split('/').Length == 2 ? ("/" + PortalAlias.Split('/')[1]) : "") +$@"/API/{(isLoader ? "BpmsSingleActionApi" : "BpmsApi")}";

            //string baseUrl = ((PortalAlias.Contains("http") ? "" : (request.Url.Scheme + "://")) + PortalAlias.TrimEnd('/')) + $@"/API/{(isLoader ? "BpmsSingleActionApi" : "BpmsApi")}";
            baseUrl += $"/BpmsGeneral/{(isGetData ? "GetData" : "PostData")}?controller={controllerName.TrimStringEnd("Controller")}&action={actionName}{(string.IsNullOrWhiteSpace(formToken) ? "" : ("&formToken=" + formToken))}";
            if (parameters != null)
            {
                parameters = parameters.Where(c => !string.IsNullOrWhiteSpace(c.Split('=').LastOrDefault())).ToArray();
            }
            return baseUrl + (parameters == null || parameters.Count() == 0 ? "" : ((baseUrl.Contains("?") ? "&" : "?") + string.Join("&", parameters.Select(c => c))));
        }

        public static string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string userRequestIPHeader = "HTTP_X_FORWARDED_FOR";
            string userIPAddress = string.Empty;
            if (context.Request.ServerVariables.AllKeys.Contains(userRequestIPHeader))
            {
                userIPAddress = context.Request.ServerVariables[userRequestIPHeader];
                if (!string.IsNullOrEmpty(userIPAddress))
                {
                    string[] addresses = userIPAddress.Split(',');
                    if (addresses.Length != 0)
                        userIPAddress = addresses[0];
                }
            }
            else
            {
                userRequestIPHeader = "X-Forwarded-For";
                if (context.Request.Headers.AllKeys.Contains(userRequestIPHeader))
                {
                    userIPAddress = context.Request.Headers[userRequestIPHeader];
                    userIPAddress = userIPAddress.Split(',')[0];
                }
            }
            if (string.IsNullOrEmpty(userIPAddress))
            {
                var remoteAddrVariable = "REMOTE_ADDR";
                if (context.Request.ServerVariables.AllKeys.Contains(remoteAddrVariable))
                {
                    userIPAddress = context.Request.ServerVariables[remoteAddrVariable];
                }
            }
            if (string.IsNullOrEmpty(userIPAddress))
            {
                userIPAddress = context.Request.UserHostAddress;
            }

            if (string.IsNullOrEmpty(userIPAddress) || userIPAddress.Trim() == "::1")
            {
                userIPAddress = string.Empty;
            }
            if (!string.IsNullOrEmpty(userIPAddress) && !ValidateIPv4(userIPAddress))
            {
                userIPAddress = string.Empty;
            }

            return userIPAddress;
        }

        private static bool ValidateIPv4(string ipString)
        {
            if (ipString.Split('.').Length != 4) return false;
            System.Net.IPAddress address;
            return System.Net.IPAddress.TryParse(ipString, out address);
        }

    }
}