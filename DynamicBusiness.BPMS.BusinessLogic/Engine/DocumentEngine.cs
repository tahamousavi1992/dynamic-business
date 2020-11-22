using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using Newtonsoft.Json.Linq;
using System.Web;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DocumentEngine : BaseEngine, IDocumentEngine
    {
        private DocumentService DocumentService { get; set; }
        public DocumentEngine(EngineSharedModel engineSharedModel, IUnitOfWork unitOfWork = null) : base(engineSharedModel, unitOfWork)
        {
            DocumentService = new DocumentService(base.UnitOfWork);
        }

        public ResultOperation IsValid(FileUploadHtml fileUploadHtml, sysBpmsVariable variable, Guid? entityId, Guid? entitydefid, string currentUserName)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (variable != null && !entityId.HasValue)
            {
                sysBpmsEntityDef entityDef = new EntityDefService(base.UnitOfWork).GetInfo(variable.EntityDefID.Value);
                entitydefid = variable.EntityDefID.Value;
                entityId = new EntityDefEngine(base.EngineSharedModel, base.UnitOfWork).GetEntityID(entityDef, variable, base.EngineSharedModel.BaseQueryModel);
            }
            Dictionary<Guid, HttpPostedFileBase> files = fileUploadHtml.Value == null ? new Dictionary<Guid, HttpPostedFileBase>() : (((Dictionary<Guid, object>)fileUploadHtml.Value).ToDictionary(c => c.Key.ToGuidObj(), c => c.Value == null ? null : c.Value is HttpPostedFileBase ? (HttpPostedFileBase)c.Value : new HttpPostedFileWrapper((HttpPostedFile)c.Value)));
            if (files.Any())
            {
                foreach (var item in files)
                {
                    ResultOperation result = this.IsValid(entitydefid, entityId, item.Value?.InputStream, item.Value?.FileName, item.Key);
                    if (!result.IsSuccess)
                        resultOperation.AddError(result.GetErrors());
                }
            }
            else
            {
                resultOperation = this.IsValid(entitydefid, entityId, null, null, fileUploadHtml.DocumentdefId.ToGuidObjNull().Value);
            }

            return resultOperation;
        }

        public ResultOperation IsValid(Guid? EntityDefID, Guid? EntityID, System.IO.Stream InputStream, string FileName, Guid DocumentDefID)
        {
            ResultOperation resultOperation = new ResultOperation();
            sysBpmsDocumentDef documentDef = new DocumentDefService(base.UnitOfWork).GetInfo(DocumentDefID);
            if (InputStream != null && InputStream.Length > 0)
            {
                string fe = System.IO.Path.GetExtension(FileName).Trim('.').ToLower();
                if (!string.IsNullOrWhiteSpace(documentDef.ValidExtentions) &&
                 !documentDef.ValidExtentions.ToStringObj().Trim().ToLower().Split(',').Contains(fe))
                {
                    resultOperation.AddError(string.Format(LangUtility.Get("FileNotValid.Text", "Engine"), documentDef.DisplayName));
                    return resultOperation;
                }
                if (documentDef.MaxSize > 0 && documentDef.MaxSize * 1024 < InputStream.Length)
                {
                    resultOperation.AddError(string.Format(LangUtility.Get("FileSizeError.Text", "Engine"), documentDef.DisplayName));
                    return resultOperation;
                }
            }

            if (InputStream == null && documentDef.IsMandatory &&
                ((EntityDefID.HasValue && !EntityID.HasValue) || !this.DocumentService.GetList(documentDef.ID, EntityDefID, EntityID, "", false, null, (EntityDefID.HasValue ? (Guid?)null : base.EngineSharedModel.CurrentThreadID)).Any()))
                resultOperation.AddError(string.Format(LangUtility.Get("RequiredFile.Text", "Engine"), documentDef.DisplayName));
            return resultOperation;


        }

        public ResultOperation SaveFile(FileUploadHtml fileUploadHtml, sysBpmsVariable variable, Guid? entityId, Guid? entitydefid, string captionOf, string currentUserName)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (variable != null && !entityId.HasValue)
            {
                sysBpmsEntityDef entityDef = new EntityDefService(base.UnitOfWork).GetInfo(variable.EntityDefID.Value);
                entitydefid = variable.EntityDefID.Value;
                entityId = new EntityDefEngine(base.EngineSharedModel, base.UnitOfWork).GetEntityID(entityDef, variable, base.EngineSharedModel.BaseQueryModel);
            }

            Dictionary<Guid, HttpPostedFileBase> files = fileUploadHtml.Value == null ? new Dictionary<Guid, HttpPostedFileBase>() : (((Dictionary<Guid, object>)fileUploadHtml.Value).ToDictionary(c => c.Key.ToGuidObj(), c => c.Value == null ? null : c.Value is HttpPostedFileBase ? (HttpPostedFileBase)c.Value : new HttpPostedFileWrapper((HttpPostedFile)c.Value)));
            foreach (var item in files)
            {
                if (resultOperation.IsSuccess)
                    resultOperation = this.SaveFile(item.Value?.InputStream, item.Value?.FileName, entityId, entitydefid, item.Key, captionOf, !fileUploadHtml.Multiple);
            }

            return resultOperation;
        }

        public ResultOperation SaveFile(System.IO.Stream inputStream, string fileName, Guid? entityID, Guid? entityDefID, Guid documentDefID, string captionOF, bool replace)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (inputStream == null || inputStream.Length <= 0) return resultOperation;
            sysBpmsDocument PostDocument = null;
            sysBpmsDocumentDef documentDef = new DocumentDefService(base.UnitOfWork).GetInfo(documentDefID);

            try
            {
                Guid Guid = System.Guid.NewGuid();
                string fe = System.IO.Path.GetExtension(fileName).Trim('.').ToLower();
                if (!string.IsNullOrWhiteSpace(documentDef.ValidExtentions) &&
                    !documentDef.ValidExtentions.ToStringObj().Trim().ToLower().Split(',').Contains(fe))
                {
                    resultOperation.AddError(string.Format(LangUtility.Get("FileNotValid.Text", "Engine"), documentDef.DisplayName));
                    return resultOperation;
                }
                if (documentDef.MaxSize > 0 && documentDef.MaxSize * 1024 < inputStream.Length)
                {
                    resultOperation.AddError(string.Format(LangUtility.Get("FileSizeError.Text", "Engine"), documentDef.DisplayName));
                    return resultOperation;
                }
                if (!System.IO.Directory.Exists(BPMSResources.FilesRoot.Trim('\\')))
                {
                    System.IO.DirectoryInfo DirectoryInfoObject = System.IO.Directory.CreateDirectory(BPMSResources.FilesRoot.Trim('\\'));
                }

                using (System.IO.FileStream saveStream = System.IO.File.Create(BPMSResources.FilesRoot.Trim('\\') + "\\" + Guid))
                {
                    byte[] bytes = new byte[1024];
                    int lenght = 0;
                    while ((lenght = inputStream.Read(bytes, 0, bytes.Length)) > 0)
                    {
                        saveStream.Write(bytes, 0, lenght);
                    }
                }

                PostDocument = new sysBpmsDocument()
                {
                    IsDeleted = false,
                    AtachDateOf = DateTime.Now,
                    CaptionOf = string.IsNullOrWhiteSpace(captionOF) ? documentDef.DisplayName : captionOF,
                    EntityID = entityID,
                    EntityDefID = entityDefID,
                    DocumentDefID = documentDefID,
                    FileExtention = fe,
                    GUID = Guid,
                    ThreadID = base.EngineSharedModel.CurrentThreadID,
                };

                if (replace)
                {
                    var _Document = this.DocumentService.GetList(documentDefID, entityDefID, entityID, "", false, null, null).FirstOrDefault();
                    if (_Document != null)
                    {
                        _Document.IsDeleted = true;
                        this.DocumentService.Update(_Document);
                    }
                }
                this.DocumentService.Add(PostDocument);
            }
            catch
            {
                resultOperation.AddError(LangUtility.Get("FileSaveError.Text", "Engine"));
            }
            resultOperation.CurrentObject = PostDocument;
            return resultOperation;
        }

        public List<sysBpmsDocument> GetList(Guid? documentDefId, Guid? VariableId, Guid? documentFolderId)
        {
            if (VariableId.HasValue)
            {
                sysBpmsVariable variable = new VariableService(base.UnitOfWork).GetInfo(VariableId.Value);
                sysBpmsEntityDef entityDef = new EntityDefService(base.UnitOfWork).GetInfo(variable.EntityDefID.Value);
                Guid? entityId = new EntityDefEngine(base.EngineSharedModel, base.UnitOfWork).GetEntityID(entityDef, variable, base.EngineSharedModel.BaseQueryModel);
                if (entityId.HasValue)
                    return new DocumentService(base.UnitOfWork).GetList(documentDefId, variable.EntityDefID, entityId, "", false, documentFolderId, null);
                else return new List<sysBpmsDocument>();
            }
            else
            {
                if (documentDefId.HasValue || documentFolderId.HasValue)
                    return new DocumentService(base.UnitOfWork).GetList(documentDefId, null, null, "", false, documentFolderId, base.EngineSharedModel.CurrentThreadID);
                else return new List<sysBpmsDocument>();
            }
        }
    }
}
