using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    public class EngineProcessProxy
    {
        public string Token { get; set; }
        public string BaseApiUrl { get; set; }
        public string UserName { get; set; }
        public string ClientIp { get; set; }
        public string ClientId { get; set; }
        public bool IsEncrypted { get; set; }
        public EngineProcessProxy(string baseApiUrl, string token, string userName, string clientIp, string clientId, bool isEncrypted)
        {
            this.BaseApiUrl = baseApiUrl;
            this.Token = token;
            this.ClientIp = clientIp;
            this.ClientId = clientId;
            this.UserName = userName;
            this.IsEncrypted = isEncrypted;
        }

        public BeginTaskResponseModel BeginTask(Guid processID, List<QueryModel> baseQueryModel)
        {
            string url = UrlUtility.GetApiUrl(this.BaseApiUrl, "BeginTask", "EngineProcess", this.Token, new string[] { "processID=" + processID.ToStringObj() });
            BeginTaskResponseModel beginTaskResponseVM = ApiUtility.PostData<BeginTaskResponseModel>(url, baseQueryModel, this.Token, this.UserName, this.ClientIp, this.ClientId, this.IsEncrypted);
            return beginTaskResponseVM;
        }

        public GetTaskFormResponseModel GetTaskForm(Guid threadTaskID, Guid? stepID, List<QueryModel> baseQueryModel)
        {
            var parameters = this.MixParams(baseQueryModel,
                $"threadTaskID={threadTaskID.ToStringObj()}",
                (!stepID.HasValue ? "" : $"stepID={ stepID}"));
            string url = UrlUtility.GetApiUrl(this.BaseApiUrl, "GetTaskForm", "EngineProcess", this.Token, parameters);
            GetTaskFormResponseModel getTaskFormResponseModel = ApiUtility.GetData<GetTaskFormResponseModel>(url, this.Token, this.UserName, this.ClientIp, this.ClientId, this.IsEncrypted);
            //convert jobject to object
            getTaskFormResponseModel?.EngineFormModel?.FormModel?.ContentHtml?.ConvertChildrenToObject();
            return getTaskFormResponseModel;
        }

        /// <summary>
        /// it is called when a pop up should be open.
        /// </summary>
        public GetTaskFormResponseModel GetForm(Guid threadTaskID, Guid formID, List<QueryModel> baseQueryModel, bool? chechAccess = null)
        {
            var parameters = this.MixParams(baseQueryModel, $"threadTaskID={threadTaskID.ToStringObj()}",
                $"formID={ formID}", (!chechAccess.HasValue ? "" : $"cAccess={chechAccess.Value}"));
            string url = UrlUtility.GetApiUrl(this.BaseApiUrl, "GetForm", "EngineProcess", this.Token, parameters);
            GetTaskFormResponseModel getTaskFormResponseModel = ApiUtility.GetData<GetTaskFormResponseModel>(url, this.Token, this.UserName, this.ClientIp, this.ClientId, this.IsEncrypted);
            //convert jobject to object
            getTaskFormResponseModel.EngineFormModel?.FormModel?.ContentHtml?.ConvertChildrenToObject();
            return getTaskFormResponseModel;
        }

        public PostTaskFormResponseModel PostTaskForm(Guid threadTaskID, string controlId, Guid stepID, bool? goNext, List<QueryModel> baseQueryModel)
        {
            string url = UrlUtility.GetApiUrl(this.BaseApiUrl, "PostTaskForm", "EngineProcess", this.Token,
                $"threadTaskID={threadTaskID.ToStringObj()}",
                (string.IsNullOrWhiteSpace(controlId) ? "" : $"controlId={controlId}"),
                ($"stepID={stepID}"),
                (!goNext.HasValue ? "" : $"goNext={goNext.Value}"));
            PostTaskFormResponseModel postTaskFormResponseModel = ApiUtility.PostData<PostTaskFormResponseModel>(url, baseQueryModel, this.Token, this.UserName, this.ClientIp, this.ClientId, this.IsEncrypted);
            return postTaskFormResponseModel;
        }

        public PostTaskFormResponseModel PostForm(Guid threadTaskID, Guid formID, string controlId, List<QueryModel> baseQueryModel)
        {
            string url = UrlUtility.GetApiUrl(this.BaseApiUrl, "PostForm", "EngineProcess", this.Token,
                $"threadTaskID={threadTaskID.ToStringObj()}",
                (string.IsNullOrWhiteSpace(controlId) ? "" : $"controlId={controlId}"),
                ($"formID={formID}"));
            PostTaskFormResponseModel postTaskFormResponseModel = ApiUtility.PostData<PostTaskFormResponseModel>(url, baseQueryModel, this.Token, this.UserName, this.ClientIp, this.ClientId, this.IsEncrypted);
            return postTaskFormResponseModel;
        }

        public ProcessDTO GetInfo(Guid processId)
        {
            return ApiUtility.GetData<ProcessDTO>(UrlUtility.GetApiUrl(this.BaseApiUrl, "GetInfo", "EngineProcess", this.Token, "processId=" + processId), this.Token, this.UserName, this.ClientIp, this.ClientId, this.IsEncrypted);
        }

        public List<Guid> GetAccessibleThreadTasks(Guid threadId)
        {
            return ApiUtility.GetData<List<Guid>>(UrlUtility.GetApiUrl(this.BaseApiUrl, "GetAccessibleThreadTasks", "EngineProcess", this.Token, "threadId=" + threadId), this.Token, this.UserName, this.ClientIp, this.ClientId, this.IsEncrypted);
        }

        public ThreadDetailDTO GetThreadDetails(Guid threadId)
        {
            var result = ApiUtility.GetData<ThreadDetailDTO>(UrlUtility.GetApiUrl(this.BaseApiUrl, "GetThreadDetails", "EngineProcess", this.Token, "threadId=" + threadId), this.Token, this.UserName, this.ClientIp, this.ClientId, this.IsEncrypted);
            result.ListOverviewForms.ForEach(c => c?.FormModel?.ContentHtml?.ConvertChildrenToObject());
            return result;
        }

        private string[] MixParams(List<QueryModel> baseQueryModel, params string[] parameters)
        {
            return baseQueryModel.Select(c => c.Key + "=" + c.Value).Union(parameters.Where(c => !string.IsNullOrWhiteSpace(c)).ToList()).Distinct().ToArray();
        }
    }
}