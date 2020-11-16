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
    public class BpmsStyleSheetController : BpmsAdminApiControlBase
    {
        #region .:: private Properties ::.
        private string DirectoryFolder
        {
            get
            {
                return BPMSResources.FilesRoot + BPMSResources.StyleSheetRoot;
            }
        }
        #endregion

        #region .:: public Method :
        [HttpGet]
        public object GetList()
        {
            //base.SetMenuIndex(AdminMenuIndex.StyleSheetIndex);
            return this.GetFiles();
        }

        [HttpPost]
        public object PostFile()
        {
            //base.SetMenuIndex(AdminMenuIndex.StyleSheetIndex);
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
        [HttpGet]
        public object GetAddEdit(string fileName = "")
        {
            StyleSheetDTO styleSheetDTO = new StyleSheetDTO();
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                if (System.IO.File.Exists(this.DirectoryFolder + "\\" + this.GetFileName(fileName)))
                {
                    styleSheetDTO.StyleCode = System.IO.File.ReadAllText(this.DirectoryFolder + "\\" + this.GetFileName(fileName));
                    styleSheetDTO.FileName = Path.GetFileNameWithoutExtension(fileName);
                    styleSheetDTO.CurrentFileName = styleSheetDTO.FileName;
                }
            }
            return styleSheetDTO;
        }
        [HttpPost]
        public object PostAddEdit(StyleSheetDTO styleSheetDTO)
        {
            if (string.IsNullOrWhiteSpace(styleSheetDTO.FileName))
            {
                return new PostMethodMessage(string.Format(SharedLang.Get("FormatRequired.Text"),"File Name"), DisplayMessageType.error);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(styleSheetDTO.CurrentFileName))
                {
                    if (System.IO.File.Exists(this.DirectoryFolder + "\\" + this.GetFileName(styleSheetDTO.CurrentFileName)))
                    {
                        System.IO.File.Delete(this.DirectoryFolder + "\\" + this.GetFileName(styleSheetDTO.CurrentFileName));
                    }
                }
                string fullName = this.GetFileName(styleSheetDTO.FileName);
                if (System.IO.File.Exists(this.DirectoryFolder + "\\" + fullName))
                {
                    System.IO.File.Delete(this.DirectoryFolder + "\\" + fullName);
                }
                System.IO.File.Create(this.DirectoryFolder + "\\" + fullName).Close();
                System.IO.File.WriteAllText(this.DirectoryFolder + "\\" + fullName, styleSheetDTO.StyleCode);
                return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
            }
        }

        #endregion

        #region .:: private Method ::.

        private List<StyleSheetDTO> GetFiles()
        {
            if (!System.IO.Directory.Exists(this.DirectoryFolder))
            {
                System.IO.DirectoryInfo DirectoryInfoObject = System.IO.Directory.CreateDirectory(this.DirectoryFolder);
            }
            return new DirectoryInfo(this.DirectoryFolder).GetFiles("*.css").Select(c => new StyleSheetDTO(c)).ToList();
        }

        private ResultOperation SaveFile(HttpPostedFile FileCss, string previousFileName)
        {
            ResultOperation resultOperation = new ResultOperation();

            if (Path.GetExtension(FileCss.FileName).ToLower() != ".css")
            {
                resultOperation.AddError("the file is not valid.");
                return resultOperation;
            }

            if (!string.IsNullOrWhiteSpace(previousFileName))
            {
                System.IO.File.Delete(this.DirectoryFolder + "\\" + previousFileName);
            }

            if (System.IO.File.Exists(this.DirectoryFolder + "\\" + FileCss.FileName))
            {
                System.IO.File.Delete(this.DirectoryFolder + "\\" + FileCss.FileName);
            }

            using (System.IO.FileStream saveStream = System.IO.File.Create(this.DirectoryFolder + "\\" + FileCss.FileName))
            {
                byte[] bytes = new byte[1024];
                int lenght = 0;
                while ((lenght = FileCss.InputStream.Read(bytes, 0, bytes.Length)) > 0)
                {
                    saveStream.Write(bytes, 0, lenght);
                }
            }
            return resultOperation;
        }

        private string GetFileName(string name)
        {
            return name + ".css";
        }
        #endregion
    }
}