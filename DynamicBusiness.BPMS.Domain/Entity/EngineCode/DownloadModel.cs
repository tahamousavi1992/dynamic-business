using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    public class DownloadModel
    {
        public DownloadModel() { }
        public DownloadModel(string fileData, string fileName)
        {
            this.FileData = fileData;
            this.FileName = fileName;
            this.SetMimeType();
        }

        private void SetMimeType()
        {
            switch (System.IO.Path.GetExtension(this.FileName).Trim('.'))
            {
                case "jpg":
                    this.MimeType = "image/jpeg";
                    break;
                case "png":
                    this.MimeType = "image/png";
                    break;
                case "gif":
                    this.MimeType = "image/gif";
                    break;
                case "doc":
                    this.MimeType = "application/msword";
                    break;
                case "docx":
                    this.MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case "zip":
                    this.MimeType = "application/zip";
                    break;
                case "pdf":
                    this.MimeType = "application/pdf";
                    break;
                case "rar":
                    this.MimeType = "application/zip";
                    break;
                case "wav":
                    this.MimeType = "audio/wav";
                    break;
                case "wave":
                    this.MimeType = "audio/wav";
                    break;
                case "wax":
                    this.MimeType = "audio/x-ms-wax";
                    break;
                case "wma":
                    this.MimeType = "audio/mid";
                    break;
                case "mid":
                    this.MimeType = "audio/x-ms-wax";
                    break;
                case "midi":
                    this.MimeType = "audio/mid";
                    break;
                case "mp3":
                    this.MimeType = "audio/mpeg";
                    break;
                case "3gp":
                    this.MimeType = "video/3gpp";
                    break;
                case "avi":
                    this.MimeType = "video/x-msvideo";
                    break;
                case "flv":
                    this.MimeType = "video/x-flv";
                    break;
                case "mov":
                    this.MimeType = "video/quicktime";
                    break;
                case "movie":
                    this.MimeType = "video/x-sgi-movie";
                    break;
                case "mp4":
                    this.MimeType = "video/mp4";
                    break;
                case "mpe":
                    this.MimeType = "video/mpeg";
                    break;
                case "mpeg":
                    this.MimeType = "video/mpeg";
                    break;
                case "wmv":
                    this.MimeType = "video/x-ms-wmv";
                    break;
            }
        }
        [DataMember]
        public string FileData { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string MimeType { get; set; }
    }
}
