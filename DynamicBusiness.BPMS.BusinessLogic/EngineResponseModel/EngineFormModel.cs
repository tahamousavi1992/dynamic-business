
using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Routing;
using System.Xml.Serialization;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    [DataContract]
    public class EngineFormModel
    {
        public EngineFormModel() { }
        public EngineFormModel(FormModel formModel, Guid? threadID, Guid? threadTaskID, Guid? processID)
        {
            this.FormModel = formModel;
            this.ThreadID = threadID;
            this.ThreadTaskID = threadTaskID;
            this.ProcessID = processID;
            this.CssFiles = string.Join(" ", ViewUtility.LoadStyleFiles().Select(c => $"<link href='{c}' rel='stylesheet'/>"));
            this.ScriptFiles = this.LoadScriptFiles();
            //this.ScriptFiles = string.Join(" ", this.LoadScriptFiles().Select(c => $"<script src='{c}'></script>"));
        }

        public EngineFormModel(FormModel formModel, Guid? applicationID)
        {
            this.FormModel = formModel;
            this.ApplicationID = applicationID;
            this.CssFiles = string.Join(" ", ViewUtility.LoadStyleFiles().Select(c => $"<link href='{c}' rel='stylesheet'>"));
            this.ScriptFiles = this.LoadScriptFiles();
            //this.ScriptFiles = string.Join(" ", this.LoadScriptFiles().Select(c => $"<script src='{c}'></script>"));
        }

        [DataMember]
        public FormModel FormModel { get; set; }
        [DataMember]
        public Guid? ThreadID { get; set; }
        [DataMember]
        public Guid? ProcessID { get; set; }
        [DataMember]
        public Guid? ThreadTaskID { get; set; }
        [DataMember]
        public Guid? ApplicationID { get; set; }

        #region .:: Api Url ::.

        [DataMember]
        public string GetPopUpUrl { get; set; }
        [DataMember]
        public string GetPostUrl { get; set; }
        [DataMember]
        public string GetDataGridElementUrl { get; set; }
        [DataMember]
        public string GetOperationUrl { get; set; }
        [DataMember]
        public string GetConfirmResultUrl { get; set; }
        [DataMember]
        public string GetControlValueUrl { get; set; }
        [DataMember]
        public string GetListElementUrl { get; set; }
        [DataMember]
        public string DeleteFileUrl { get; set; }
        [DataMember]
        public string DownloadFileUrl { get; set; }
        [DataMember]
        public string GetDataGridReport { get; set; }
        [DataMember]
        public string GetDataGridPagingUrl { get; set; }
        [DataMember]
        public string GetChartElementUrl { get; set; }
        [DataMember]
        public string GetExecuteCodeUrl { get; set; }
        [DataMember]
        public string GetCaptchaUrl { get; set; }
        [DataMember]
        public string CssFiles { get; set; }
        [DataMember]
        public List<string> ScriptFiles { get; set; }
        [DataMember]
        public string PageParams { get; set; }
        #endregion

        /// <summary>
        /// It is used to set required url.
        /// </summary>
        /// <param name="apiBaseUrl">It contains base address with portal alias.</param>
        public void SetUrlsForSingleAction(string PortalAlias, HttpRequestBase request, string getPopUpUrl, string getPostUrl, string formToken, int tabmid)
        {
            string apiBaseUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/');
            PortalAlias = PortalAlias.Replace("http://", "").Replace("https://", "");
            apiBaseUrl = apiBaseUrl + (PortalAlias.Split('/').Length == 2 ? ("/" + PortalAlias.Split('/')[1]) : "") + $"/API/BpmsSingleActionApi/{tabmid}";

            this.GetPopUpUrl = UrlUtility.AddParamsToUrl(getPopUpUrl, "formToken", formToken);
            this.GetPostUrl = string.IsNullOrWhiteSpace(getPostUrl) ? string.Empty : UrlUtility.AddParamsToUrl(getPostUrl, "formToken", formToken);
            this.PageParams = UrlUtility.GetParams(request);
            string[] arrayParams = UrlUtility.GetParamsAsArray(request,
            string.Format("threadTaskID={0}", this.ThreadTaskID),
            string.Format("threadId={0}", this.ThreadID),
            string.Format("applicationPageId={0}", this.ApplicationID)).ToArray();

            this.DeleteFileUrl = this.GetGeneralApiUrl(apiBaseUrl, "GetDelete", "EngineDocument", formToken, true);
            this.DownloadFileUrl = this.GetGeneralApiUrl(apiBaseUrl, "GetDownload", "EngineDocument", formToken, true);
            this.GetDataGridElementUrl = this.GetGeneralApiUrl(apiBaseUrl, "GetDataGridElement", "EngineHtmlElement", formToken, true, arrayParams);
            this.GetOperationUrl = this.GetGeneralApiUrl(apiBaseUrl, "Execute", "EngineOperation", formToken, false, arrayParams);
            this.GetConfirmResultUrl = this.GetGeneralApiUrl(apiBaseUrl, "GetConfirmResult", "EngineCode", formToken, true, arrayParams);
            this.GetExecuteCodeUrl = this.GetGeneralApiUrl(apiBaseUrl, "ExecuteCode", "EngineCode", formToken, true, arrayParams);
            this.GetControlValueUrl = this.GetGeneralApiUrl(apiBaseUrl, "GetValue", "EngineHtmlElement", formToken, true, arrayParams);
            this.GetListElementUrl = this.GetGeneralApiUrl(apiBaseUrl, "GetListElement", "EngineVariable", formToken, true, arrayParams);
            this.GetChartElementUrl = this.GetGeneralApiUrl(apiBaseUrl, "GetChartElement", "EngineHtmlElement", formToken, true, arrayParams);
            this.GetCaptchaUrl = this.GetGeneralApiUrl(apiBaseUrl, "Get", "EngineCaptcha", formToken, true, arrayParams);
            //GetDataGridReport
            this.GetDataGridReport = this.GetGeneralApiUrl(apiBaseUrl, "GetDataGridReport", "EngineReport", formToken, true, arrayParams);
            this.GetDataGridPagingUrl = this.GetGeneralApiUrl(apiBaseUrl, "GetDataGridElement", "EngineHtmlElement", formToken, true);
        }

        public void SetUrls(string getPopUpUrl, string getPostUrl, HttpRequestBase request, string portalAlias, string formToken)
        {
            this.GetPopUpUrl = string.IsNullOrWhiteSpace(getPopUpUrl) ? string.Empty : UrlUtility.AddParamsToUrl(getPopUpUrl, "formToken", formToken);
            this.GetPostUrl = string.IsNullOrWhiteSpace(getPostUrl) ? string.Empty : UrlUtility.AddParamsToUrl(getPostUrl, "formToken", formToken);

            this.PageParams = UrlUtility.GetParams(request);
            string[] arrayParams = UrlUtility.GetParamsAsArray(request,
             string.Format("threadTaskID={0}", this.ThreadTaskID),
             string.Format("threadId={0}", this.ThreadID),
             string.Format("applicationPageId={0}", this.ApplicationID)).ToArray();
            using (ConfigurationService configurationService = new ConfigurationService())
            {
                this.DeleteFileUrl = UrlUtility.GetApiUrl(request, portalAlias, "GetDelete", "EngineDocument", formToken);
                this.DownloadFileUrl = UrlUtility.GetApiUrl(request, portalAlias, "GetDownload", "EngineDocument", formToken);
                this.GetDataGridElementUrl = UrlUtility.GetApiUrl(request, portalAlias, "GetDataGridElement", "EngineHtmlElement", formToken, arrayParams);
                this.GetOperationUrl = UrlUtility.GetApiUrl(request, portalAlias, "Execute", "EngineOperation", formToken, arrayParams);
                this.GetConfirmResultUrl = UrlUtility.GetApiUrl(request, portalAlias, "GetConfirmResult", "EngineCode", formToken, arrayParams);
                this.GetExecuteCodeUrl = UrlUtility.GetApiUrl(request, portalAlias, "ExecuteCode", "EngineCode", formToken, arrayParams);
                this.GetControlValueUrl = UrlUtility.GetApiUrl(request, portalAlias, "GetValue", "EngineHtmlElement", formToken, arrayParams);
                this.GetListElementUrl = UrlUtility.GetApiUrl(request, portalAlias, "GetListElement", "EngineVariable", formToken, arrayParams);
                this.GetChartElementUrl = UrlUtility.GetApiUrl(request, portalAlias, "GetChartElement", "EngineHtmlElement", formToken, arrayParams);
                this.GetCaptchaUrl = UrlUtility.GetApiUrl(request, portalAlias, "Get", "EngineCaptcha", string.Empty);
                //GetDataGridReport
                this.GetDataGridReport = UrlUtility.GetApiUrl(request, portalAlias, "GetDataGridReport", "EngineReport", formToken, arrayParams);
                this.GetDataGridPagingUrl = UrlUtility.GetApiUrl(request, portalAlias, "GetDataGridElement", "EngineHtmlElement", formToken);
            }
        }

        public void SetReadOnlyUrls(string getPopUpUrl, HttpRequestBase request, string portalAlias, string formToken)
        {
            this.GetPopUpUrl = string.IsNullOrWhiteSpace(getPopUpUrl) ? string.Empty : UrlUtility.AddParamsToUrl(getPopUpUrl, "formToken", formToken);

            this.PageParams = UrlUtility.GetParams(request);
            string[] arrayParams = UrlUtility.GetParamsAsArray(request,
             string.Format("threadTaskID={0}", this.ThreadTaskID),
             string.Format("threadId={0}", this.ThreadID),
             string.Format("applicationPageId={0}", this.ApplicationID)).ToArray();
            using (ConfigurationService configurationService = new ConfigurationService())
            {
                this.DownloadFileUrl = UrlUtility.GetApiUrl(request, portalAlias, "GetDownload", "EngineDocument", formToken);
                this.GetDataGridElementUrl = UrlUtility.GetApiUrl(request, portalAlias, "GetDataGridElement", "EngineHtmlElement", formToken, arrayParams);
                this.GetControlValueUrl = UrlUtility.GetApiUrl(request, portalAlias, "GetValue", "EngineHtmlElement", formToken, arrayParams);
                this.GetListElementUrl = UrlUtility.GetApiUrl(request, portalAlias, "GetListElement", "EngineVariable", formToken, arrayParams);
                this.GetChartElementUrl = UrlUtility.GetApiUrl(request, portalAlias, "GetChartElement", "EngineHtmlElement", formToken, arrayParams);
                //GetDataGridReport
                this.GetDataGridReport = UrlUtility.GetApiUrl(request, portalAlias, "GetDataGridReport", "EngineReport", formToken, arrayParams);
                this.GetDataGridPagingUrl = UrlUtility.GetApiUrl(request, portalAlias, "GetDataGridElement", "EngineHtmlElement", formToken);
            }
        }

        private List<string> LoadScriptFiles()
        {
            string directoryFolder = (BPMSResources.FilesRoot + BPMSResources.JavaScriptRoot);
            string directoryVirtualFolder = (BPMSResources.FilesVirtualRoot + BPMSResources.JavaScriptRoot);
            if (System.IO.Directory.Exists(directoryFolder))
            {
                return new DirectoryInfo(directoryFolder).GetFiles("*.js").Select(c => "/" + directoryVirtualFolder + "/" + c.Name).ToList();
            }
            else return new List<string>();
        }

        private string GetGeneralApiUrl(string apiBaseUrl, string actionName, string controllerName, string token, bool isGetData, params string[] parameters)
        {
            string baseUrl = apiBaseUrl.TrimEnd('/') + $@"/BpmsGeneral/";
            baseUrl += $"{(isGetData ? "GetData" : "PostData")}?controller={controllerName.TrimStringEnd("Controller")}&action={actionName}{(string.IsNullOrWhiteSpace(token) ? "" : ("&formToken=" + token))}";
            if (parameters != null)
            {
                parameters = parameters.Where(c => !string.IsNullOrWhiteSpace(c.Split('=').LastOrDefault())).ToArray();
            }
            return baseUrl + (parameters == null || parameters.Count() == 0 ? "" : ((baseUrl.Contains("?") ? "&" : "?") + string.Join("&", parameters.Select(c => c))));
        }
    }
}