using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Web.Api;
using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Routing;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [System.Web.Http.AllowAnonymous]
    //[DnnAuthorize]
    public class BpmsAdminApiControlBase : DnnApiController
    {
        public BpmsAdminApiControlBase()
        {
        }
        protected HttpRequest MyRequest
        {
            get
            {
                return HttpContext.Current.Request;
            }
        }

        public Guid? ProcessId { get; set; }
        public Guid? ApplicationPageId { get; set; }

        //public ViewDataDictionary ViewData = new ViewDataDictionary();
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            this.ProcessId = this.MyRequest.QueryString["processId"]?.ToGuidObjNull() ?? this.MyRequest.Form["processId"]?.ToGuidObjNull();
            this.ApplicationPageId = this.MyRequest.QueryString["ApplicationPageId"]?.ToGuidObjNull() ?? this.MyRequest.Form["ApplicationPageId"]?.ToGuidObjNull();
            //Because json data is not put in request.Form
            var data = controllerContext.Request.Content.ReadAsStringAsync().Result;
            if (data != null &&
                (DomainUtility.GetRegularValue("\"ApplicationPageId\":\"", "\"", data).Any() ||
                 DomainUtility.GetRegularValue("\"ProcessId\":\"", "\"", data).Any()))
            {
                this.ProcessId = DomainUtility.GetRegularValue("\"ProcessId\":\"", "\"", data).FirstOrDefault().ToGuidObjNull();
                this.ApplicationPageId = DomainUtility.GetRegularValue("\"ApplicationPageId\":\"", "\"", data).FirstOrDefault().ToGuidObjNull();
            }
        }
 
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
    }
}