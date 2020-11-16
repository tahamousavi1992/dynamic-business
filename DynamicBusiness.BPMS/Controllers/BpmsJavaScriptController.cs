using DynamicBusiness.BPMS.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System.IO;

namespace DynamicBusiness.BPMS.Controllers
{ 
    public class BpmsJavaScriptController : BpmsAdminApiControlBase
    {
        #region .:: private Properties ::.
        private string DirectoryFolder
        {
            get
            {
                return BPMSResources.FilesRoot + BPMSResources.JavaScriptRoot;
            }
        }
        #endregion

        #region .:: public Method :
        [HttpGet]
        public object GetList()
        {
            //base.SetMenuIndex(AdminMenuIndex.JavaScript);
            return this.GetFiles();
        }

        [HttpPost]
        public object PostFile()
        {
            ResultOperation resultOperation = this.SaveFile(base.MyRequest.Files[0], "");
            if (!resultOperation.IsSuccess)
                return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            else
                return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
        }

        [HttpDelete]
        public object Delete(string fileName)
        {
            if (System.IO.File.Exists(this.DirectoryFolder + "\\" + this.GetFileName(fileName)))
            {
                System.IO.File.Delete(this.DirectoryFolder + "\\" + this.GetFileName(fileName));
            }
            return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
        }

        #endregion

        #region .:: private Method ::.

        private List<JsFileDTO> GetFiles()
        {
            if (!System.IO.Directory.Exists(this.DirectoryFolder))
            {
                System.IO.DirectoryInfo DirectoryInfoObject = System.IO.Directory.CreateDirectory(this.DirectoryFolder);
            }
            return new DirectoryInfo(this.DirectoryFolder).GetFiles("*.js").Select(c => new JsFileDTO(c)).ToList();
        }

        private ResultOperation SaveFile(HttpPostedFile FileJs, string previousFileName)
        {
            ResultOperation resultOperation = new ResultOperation();

            if (Path.GetExtension(FileJs.FileName).ToLower() != ".js")
            {
                resultOperation.AddError("File is no a js file.");
                return resultOperation;
            }

            if (!string.IsNullOrWhiteSpace(previousFileName))
            {
                System.IO.File.Delete(this.DirectoryFolder + "\\" + previousFileName);
            }

            if (System.IO.File.Exists(this.DirectoryFolder + "\\" + FileJs.FileName))
            {
                System.IO.File.Delete(this.DirectoryFolder + "\\" + FileJs.FileName);
            }

            using (System.IO.FileStream saveStream = System.IO.File.Create(this.DirectoryFolder + "\\" + FileJs.FileName))
            {
                byte[] bytes = new byte[1024];
                int lenght = 0;
                while ((lenght = FileJs.InputStream.Read(bytes, 0, bytes.Length)) > 0)
                {
                    saveStream.Write(bytes, 0, lenght);
                }
            }
            return resultOperation;
        }

        private string GetFileName(string name)
        {
            return name + ".js";
        }

        #endregion
    }
}