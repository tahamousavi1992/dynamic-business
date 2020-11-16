using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DynamicBusiness.BPMS.SharedPresentation
{
    public class EngineApplicationProxy
    {
        public string Token { get; set; }
        public string BaseApiUrl { get; set; }
        public string UserName { get; set; }
        public string ClientIp { get; set; }
        public string ClientId { get; set; }
        public bool IsEncrypted { get; set; }
        public EngineApplicationProxy(string baseApiUrl, string token, string userName, string clientIp, string clientId, bool isEncrypted)
        {
            this.BaseApiUrl = baseApiUrl;
            this.Token = token;
            this.ClientIp = clientIp;
            this.ClientId = clientId;
            this.UserName = userName;
            this.IsEncrypted = isEncrypted;
        }

        public GetFormResponseModel GetForm(Guid? applicationPageId, Guid? formID, List<QueryModel> baseQueryModel)
        {
            var parameters = this.MixParams(baseQueryModel,
                (!applicationPageId.HasValue ? "" : $"applicationPageId={ applicationPageId}"),
                (!formID.HasValue ? "" : $"formID={ formID}"));
            string url = UrlUtility.GetApiUrl(this.BaseApiUrl, "GetForm", "EngineApplication", "", parameters);
            GetFormResponseModel getFormResponseModel = ApiUtility.GetData<GetFormResponseModel>(url, this.Token, this.UserName, this.ClientIp, this.ClientId, this.IsEncrypted);
            //convert jobject to object
            getFormResponseModel?.EngineFormModel?.FormModel?.ContentHtml?.ConvertChildrenToObject();
            return getFormResponseModel;
        }

        public PostFormResponseModel PostForm(Guid applicationPageId, string controlId, List<QueryModel> baseQueryModel)
        {
            string url = UrlUtility.GetApiUrl(this.BaseApiUrl, "PostForm", "EngineApplication", this.Token,
                $"applicationPageId={applicationPageId.ToStringObj()}",
                $"controlId={controlId}");
            PostFormResponseModel postFormResponseModel = ApiUtility.PostData<PostFormResponseModel>(url, baseQueryModel, this.Token, this.UserName, this.ClientIp, this.ClientId, this.IsEncrypted);
            return postFormResponseModel;
        }

        public DynamicFormDTO GetInfo(Guid applicationPageId)
        {
            return ApiUtility.GetData<DynamicFormDTO>(UrlUtility.GetApiUrl(this.BaseApiUrl, "GetInfo", "EngineApplication", this.Token, "applicationPageId=" + applicationPageId), this.Token, this.UserName, this.ClientIp, this.ClientId, this.IsEncrypted);
        }

        private string[] MixParams(List<QueryModel> baseQueryModel, params string[] parameters)
        {
            return baseQueryModel.Select(c => c.Key + "=" + c.Value.ToStringObj()).Union(parameters.Where(c => !string.IsNullOrWhiteSpace(c)).ToList()).Distinct().ToArray();
        }
    }
}