using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DynamicBusiness.BPMS.SharedPresentation
{
    public class EngineFormProxy
    {
        public string Token { get; set; }
        public string BaseApiUrl { get; set; }
        public string UserName { get; set; }
        public string ClientIp { get; set; }
        public string ClientId { get; set; }
        public bool IsEncrypted { get; set; }
        public EngineFormProxy(string baseApiUrl, string token, string userName, string clientIp, string clientId, bool isEncrypted)
        {
            this.BaseApiUrl = baseApiUrl;
            this.Token = token;
            this.ClientIp = clientIp;
            this.ClientId = clientId;
            this.UserName = userName;
            this.IsEncrypted = isEncrypted;
        }

    
        public DynamicFormDTO GetInfo(Guid Id)
        {
            return ApiUtility.GetData<DynamicFormDTO>(UrlUtility.GetApiUrl(this.BaseApiUrl, "GetInfo", "EngineForm", this.Token, "Id=" + Id), this.Token, this.UserName, this.ClientIp, this.ClientId, this.IsEncrypted);
        }

        private string[] MixParams(List<QueryModel> baseQueryModel, params string[] parameters)
        {
            return baseQueryModel.Select(c => c.Key + "=" + c.Value.ToStringObj()).Union(parameters.Where(c => !string.IsNullOrWhiteSpace(c)).ToList()).Distinct().ToArray();
        }
    }
}