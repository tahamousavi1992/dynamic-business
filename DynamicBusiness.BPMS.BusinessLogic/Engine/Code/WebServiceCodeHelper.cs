using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class WebServiceCodeHelper : IWebServiceCodeHelper
    {
        private Domain.CodeBaseSharedModel CodeBaseShared { get; set; }

        public WebServiceCodeHelper(IDataManageEngine dataManageEngine, Domain.CodeBaseSharedModel codeBaseShared)
        {
            this.DataManageHelperService = dataManageEngine;
            this.CodeBaseShared = codeBaseShared;
        }

        private IDataManageEngine DataManageHelperService { get; set; }

        /// <param name="contentType">is application/json , application/x-www-form-urlencoded</param>
        public object Post(string url, string contentType, List<QueryModel> headers, params QueryModel[] parameterModel)
        {
            object retValue = null;
            HttpClient client = new HttpClient();
            AddHeaders(client, headers);
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            HttpResponseMessage response = null;
            if (contentType.ToLower().Contains("application/json"))
            {
                string jsonContent = "{{" + string.Join(",", parameterModel?.ToList().Select(c => $"\"{c.Key}\":\"{c.Value.ToStringObj()}\"")) + "}";
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                response = client.PostAsync(url, content).Result;
            }
            else
            {
                var values = new Dictionary<string, string>();
                parameterModel?.ToList().ForEach((item) =>
                {
                    values.Add(item.Key, item.Value.ToStringObj());
                });
                var content = new FormUrlEncodedContent(values);
                response = client.PostAsync(url, content).Result;
            }
            if (response.IsSuccessStatusCode)
            {
                retValue = response.Content.ReadAsAsync<object>().Result;
            }
            return retValue;
        }

        /// <param name="contentType">is application/json , application/x-www-form-urlencoded</param>
        public object Put(string url, string contentType, List<QueryModel> headers, params QueryModel[] parameterModel)
        {
            object retValue = null;
            HttpClient client = new HttpClient();
            AddHeaders(client, headers);
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            HttpResponseMessage response = null;
            if (contentType.ToLower().Contains("application/json"))
            {
                string jsonContent = "{{" + string.Join(",", parameterModel?.ToList().Select(c => $"\"{c.Key}\":\"{c.Value.ToStringObj()}\"")) + "}";
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                response = client.PostAsync(url, content).Result;
            }
            else
            {
                var values = new Dictionary<string, string>();
                parameterModel?.ToList().ForEach((item) =>
                {
                    values.Add(item.Key, item.Value.ToStringObj());
                });
                var content = new FormUrlEncodedContent(values);
                response = client.PutAsync(url, content).Result;
            }
            if (response.IsSuccessStatusCode)
            {
                retValue = response.Content.ReadAsAsync<object>().Result;
            }
            return retValue;
        }

        public object Get(string url, List<QueryModel> headers, params QueryModel[] parameterModel)
        {
            object retValue = null;
            var values = new Dictionary<string, string>();
            if (parameterModel != null && parameterModel.Any())
            {
                string parameters = string.Join("&", parameterModel?.ToList().Select(c => $"{c.Key}={c.Value.ToStringObj()}"));
                url = (url.Contains("?") ? (url + "&") : (url + "?")) + parameters;
            }
            HttpClient client = new HttpClient();
            AddHeaders(client, headers);
            var content = new FormUrlEncodedContent(values);
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                retValue = response.Content.ReadAsAsync<object>().Result;
            }
            return retValue;
        }

        private void AddHeaders(HttpClient client, List<QueryModel> headers)
        {
            if (headers != null)
                foreach (var item in headers)
                    client.DefaultRequestHeaders.Add(item.Key, item.Value.ToStringObj());
        }
    }
}
