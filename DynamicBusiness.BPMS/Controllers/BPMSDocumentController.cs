using DynamicBusiness.BPMS.BusinessLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DynamicBusiness.BPMS.Domain;
using System.Web.Script.Serialization;
using DynamicBusiness.BPMS.SharedPresentation;
using DotNetNuke.Common.Utilities;

namespace DynamicBusiness.BPMS.Controllers
{
    [System.Web.Http.AllowAnonymous]
    public class BpmsDocumentController : BpmsAdminApiControlBase
    {
        #region .:: Document ::.
        [HttpGet]
        public object GetFolderList(Guid? selectedID = null)
        {
            using (DocumentFolderService documentFolderService = new DocumentFolderService())
                return Json(new
                {
                    SelectedID = selectedID,
                    FolderList = documentFolderService.HelperGetTreeList(selectedID)
                });
        }

        [HttpGet]
        public object GetAddEditFolder(Guid? id = null, Guid? parentId = null)
        {
            if (!id.IsNullOrEmpty())
            {
                using (DocumentFolderService documentFolderService = new DocumentFolderService())
                {
                    return new DocumentFolderDTO(documentFolderService.GetInfo(id.Value));
                }
            }
            else
                return new DocumentFolderDTO() { DocumentFolderID = parentId };
        }

        [HttpPost]
        public object PostAddEditFolder(DocumentFolderDTO documentFolderDTO)
        {
            using (DocumentFolderService documentFolderService = new DocumentFolderService())
            {
                sysBpmsDocumentFolder documentFolder = !documentFolderDTO.ID.IsNullOrEmpty() ? documentFolderService.GetInfo(documentFolderDTO.ID) : new sysBpmsDocumentFolder();
                documentFolder.Update(documentFolderDTO.DocumentFolderID, documentFolderDTO.NameOf, documentFolderDTO.DisplayName, true);

                ResultOperation resultOperation = null;
                if (!documentFolderDTO.ID.IsNullOrEmpty())
                    resultOperation = documentFolderService.Update(documentFolder);
                else
                    resultOperation = documentFolderService.Add(documentFolder);

                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success, documentFolder.ID);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }

        [HttpGet]
        public object GetInActiveFolder(Guid id)
        {
            using (DocumentFolderService documentFolderService = new DocumentFolderService())
            {
                sysBpmsDocumentFolder sysBpmsDocumentFolder = documentFolderService.GetInfo(id);
                sysBpmsDocumentFolder.InActive();
                ResultOperation resultOperation = documentFolderService.Update(sysBpmsDocumentFolder);

                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }

        #endregion

        #region .:: DocumentDef ::.

        public object GetDocumentDefListx([System.Web.Http.FromUri] DocumentDefIndexSearchDTO indexSearchVM)
        { 
            using (DocumentDefService documentDefService = new DocumentDefService())
            {
                indexSearchVM.Update(documentDefService.GetList(indexSearchVM.DocumentFolderID, "", "", true, null, indexSearchVM.GetPagingProperties).Select(c => new DocumentDefDTO(c)).ToList());
                return indexSearchVM;
            }
        }

        [HttpGet]
        public object GetAddEditDocumentDef(Guid? id = null, Guid? documentFolderID = null)
        {
            if (!id.IsNullOrEmpty())
            {
                using (DocumentDefService documentDefService = new DocumentDefService())
                {
                    return new DocumentDefDTO(documentDefService.GetInfo(id.Value));
                }
            }
            else
            {
                return new DocumentDefDTO() { DocumentFolderID = documentFolderID.Value, IsActive = true };
            }
        }

        [HttpPost]
        public object PostAddEditDocumentDef(DocumentDefDTO DocumentDefDTO)
        {
            using (DocumentDefService documentDefService = new DocumentDefService())
            {
                sysBpmsDocumentDef documentDef = DocumentDefDTO.ID != Guid.Empty ? documentDefService.GetInfo(DocumentDefDTO.ID) : new sysBpmsDocumentDef();
                documentDef.Update(DocumentDefDTO.DocumentFolderID, DocumentDefDTO.NameOf, DocumentDefDTO.DisplayName, DocumentDefDTO.MaxSize, DocumentDefDTO.ValidExtentions, DocumentDefDTO.IsMandatory, DocumentDefDTO.Description, DocumentDefDTO.IsSystemic, DocumentDefDTO.IsActive);
                ResultOperation resultOperation = null;
                if (documentDef.ID != Guid.Empty)
                    resultOperation = documentDefService.Update(documentDef);
                else
                    resultOperation = documentDefService.Add(documentDef);

                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }

        [HttpGet]
        public object GetInActiveDocumentDef(Guid id)
        {
            using (DocumentDefService documentDefService = new DocumentDefService())
            {
                sysBpmsDocumentDef documentDef = documentDefService.GetInfo(id);
                documentDef.IsActive = false;
                ResultOperation resultOperation = documentDefService.Update(documentDef);

                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }

        #endregion
    }
}