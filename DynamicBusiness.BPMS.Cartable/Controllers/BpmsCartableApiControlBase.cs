using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Web.Api;
using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;

namespace DynamicBusiness.BPMS.Cartable
{
    [BpmsCartableAuth]
    public class BpmsCartableApiControlBase : DnnApiController
    {
        public BpmsCartableApiControlBase()
        {

            if (this.MyRequest.Headers.AllKeys.Contains("clientIp"))
                this.ClientIp = this.MyRequest.Headers["clientIp"].ToStringObj();
            else
                this.ClientIp = ApiUtility.GetIPAddress();

            using (APIAccessService apiAccessService = new APIAccessService())
            {
                //api call using toke header,which is password, or formToken ,which is a parameter like antiforgerytoken cosist of sessionId and mainDynamicFormId encripted by sessionId. 
                if (!this.MyRequest.Headers.AllKeys.Contains("token"))
                {
                    this.ClientUserName = DomainUtility.IsTestEnvironment ? "bpms_expert" : base.UserInfo.Username;
                    this.ClientFormToken = this.MyRequest.QueryString[FormTokenUtility.FormToken].ToStringObj();
                    this.ClientId = HttpContext.Current.Session.SessionID;
                    this.ApiSessionId = DomainUtility.CreateApiSessionID(this.ClientId, this.ClientIp);
                    this.IsEncrypted = FormTokenUtility.GetIsEncrypted(this.ClientFormToken, this.ClientId);
                }
                else
                {
                    if (this.MyRequest.Headers.AllKeys.Contains("userName"))
                        this.ClientUserName = this.MyRequest.Headers["userName"].ToStringObj();

                    this.ClientId = this.MyRequest.Headers["clientId"].ToStringObj();
                    this.ApiSessionId = DomainUtility.CreateApiSessionID(this.ClientId, this.ClientIp); ;
                    //set ApiSessionID 
                    if (!apiAccessService.HasAccess(ApiUtility.GetIPAddress(), this.MyRequest.Headers.GetValues("token").FirstOrDefault()))
                    {
                        throw new Exception("You are not authorized to access this application.");
                    }
                    this.IsEncrypted = this.MyRequest.Headers["isEncrypted"].ToStringObj() == "1";
                }
            }
        }
        protected HttpRequest MyRequest
        {
            get
            {
                return HttpContext.Current.Request;
            }
        }
        private sysBpmsUser myUser { get; set; }
        public sysBpmsUser MyUser
        {
            get
            {
                if (this.myUser == null)
                {
                    using (UserService userService = new UserService())
                    {
                        this.myUser = userService.GetInfo(this.ClientUserName ?? "") ?? new Domain.sysBpmsUser();
                    }
                }
                return this.myUser;
            }
        }
        /// <summary>
        /// it is session id ,and used to encript form data passed to client.
        /// </summary>
        protected string ClientId { get; private set; }
        protected string ClientIp { get; private set; }
        protected string ClientUserName { get; private set; }
        /// <summary>
        /// it is set in parameter request and is a encripted string consist of sessionId and mainDynamicFormId
        /// </summary>
        protected string ClientFormToken { get; private set; }
        //this could be clientId or a generated key using clientId and clientIp
        protected string ApiSessionId { get; private set; }
        public bool IsEncrypted { get; private set; }
        protected void Download(sysBpmsDocument Document)
        {
            if (Document != null)
            {
                string fileType = Document.FileExtention;
                string contentType = "";
                switch (fileType.ToLower().TrimStart('.'))
                {
                    case "jpg":
                        contentType = "image/jpeg";
                        break;
                    case "png":
                        contentType = "image/png";
                        break;
                    case "gif":
                        contentType = "image/gif";
                        break;
                    case "doc":
                        contentType = "application/msword";
                        break;
                    case "docx":
                        contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        break;
                    case "zip":
                        contentType = "application/zip";
                        break;
                    case "pdf":
                        contentType = "application/pdf";
                        break;
                    case "rar":
                        contentType = "application/zip";
                        break;
                    case "wav":
                        contentType = "audio/wav";
                        break;
                    case "wave":
                        contentType = "audio/wav";
                        break;
                    case "wax":
                        contentType = "audio/x-ms-wax";
                        break;
                    case "wma":
                        contentType = "audio/mid";
                        break;
                    case "mid":
                        contentType = "audio/x-ms-wax";
                        break;
                    case "midi":
                        contentType = "audio/mid";
                        break;
                    case "mp3":
                        contentType = "audio/mpeg";
                        break;
                    case "3gp":
                        contentType = "video/3gpp";
                        break;
                    case "avi":
                        contentType = "video/x-msvideo";
                        break;
                    case "flv":
                        contentType = "video/x-flv";
                        break;
                    case "mov":
                        contentType = "video/quicktime";
                        break;
                    case "movie":
                        contentType = "video/x-sgi-movie";
                        break;
                    case "mp4":
                        contentType = "video/mp4";
                        break;
                    case "mpe":
                        contentType = "video/mpeg";
                        break;
                    case "mpeg":
                        contentType = "video/mpeg";
                        break;
                    case "wmv":
                        contentType = "video/x-ms-wmv";
                        break;
                    case "tiff":
                        contentType = "image/tiff";
                        break;
                    case "bmp":
                        contentType = "image/bmp";
                        break;
                    default:
                        contentType = "application/octet-stream";
                        break;
                }
                Document.CaptionOf = string.IsNullOrWhiteSpace(Document.CaptionOf) ? "File" : Document.CaptionOf;

                if (System.IO.File.Exists(BPMSResources.FilesRoot + Document.GUID))
                {
                    using (FileStream fsSource = new FileStream(BPMSResources.FilesRoot + Document.GUID, FileMode.Open, FileAccess.Read))
                    {
                        HttpContext.Current.Response.Buffer = false;
                        HttpContext.Current.Response.Clear();
                        HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + Document.CaptionOf + "." + fileType);
                        HttpContext.Current.Response.ContentType = contentType;
                        int length = 1024 * 10000;
                        int index = 0;
                        byte[] fileBytes = new byte[length];
                        while ((index = fsSource.Read(fileBytes, 0, length)) > 0)
                        {
                            HttpContext.Current.Response.OutputStream.Write(fileBytes, 0, index);
                        }
                        HttpContext.Current.Response.Flush();
                        HttpContext.Current.Response.End();
                    }
                }
            }
        }

        protected string GetRedirectUrl(RedirectUrlModel redirectModel)
        {
            if (redirectModel != null)
            {
                if (redirectModel.ApplicationPageId.HasValue)
                    return UrlUtility.GetCartableUrl("GetCartablePage", (redirectModel.ListParameter ?? new List<string>()).
                        Union(new List<string>() { $"applicationPageId={redirectModel.ApplicationPageId}" }).ToArray());
                else
                    return redirectModel.Url;
            }
            else
                return null;
        }
    }
}