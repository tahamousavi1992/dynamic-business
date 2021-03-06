﻿using DynamicBusiness.BPMS.BusinessLogic;
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
    public class BpmsAssemblyController : BpmsAdminApiControlBase
    {
        #region .:: private Properties ::.
        private string DirectoryFolder
        {
            get
            {
                return BPMSResources.FilesRoot + BPMSResources.AssemblyRoot;
            }
        }
        #endregion

        #region .:: public Method :
        public object GetList()
        {
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

        private List<AssemblyDTO> GetFiles()
        {
            if (!System.IO.Directory.Exists(this.DirectoryFolder))
            {
                System.IO.DirectoryInfo DirectoryInfoObject = System.IO.Directory.CreateDirectory(this.DirectoryFolder);
            }
            return new DirectoryInfo(this.DirectoryFolder).GetFiles("*.dll").Select(c => new AssemblyDTO(c)).ToList();
        }

        private ResultOperation SaveFile(HttpPostedFile fileAssembly, string previousFileName)
        {
            ResultOperation resultOperation = new ResultOperation();

            if (Path.GetExtension(fileAssembly.FileName).ToLower() != ".dll")
            {
                resultOperation.AddError("the file is not valid.");
                return resultOperation;
            }

            if (!string.IsNullOrWhiteSpace(previousFileName))
            {
                System.IO.File.Delete(this.DirectoryFolder + "\\" + previousFileName);
            }

            if (System.IO.File.Exists(this.DirectoryFolder + "\\" + fileAssembly.FileName))
            {
                System.IO.File.Delete(this.DirectoryFolder + "\\" + fileAssembly.FileName);
            }

            using (System.IO.FileStream saveStream = System.IO.File.Create(this.DirectoryFolder + "\\" + fileAssembly.FileName))
            {
                byte[] bytes = new byte[1024];
                int lenght = 0;
                while ((lenght = fileAssembly.InputStream.Read(bytes, 0, bytes.Length)) > 0)
                {
                    saveStream.Write(bytes, 0, lenght);
                }
            }
            return resultOperation;
        }

        private string GetFileName(string name)
        {
            return name + ".dll";
        }
        #endregion
    }
}