using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DocumentCodeHelper
    {
        private Domain.CodeBaseSharedModel CodeBaseShared { get; set; }
        public EngineSharedModel EngineSharedModel { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }
        public DocumentCodeHelper(EngineSharedModel engineSharedModel,
            IUnitOfWork unitOfWork, Domain.CodeBaseSharedModel codeBaseShared)
        {
            this.EngineSharedModel = engineSharedModel;
            this.CodeBaseShared = codeBaseShared;
            this.UnitOfWork = unitOfWork;
        }

        public void Download(string base64, string fileName)
        {
            this.CodeBaseShared.ListDownloadModel.Add(new DownloadModel(base64, fileName));
        }

        public void DownloadByStream(System.IO.MemoryStream streamFile, string fileName)
        {
            byte[] bytes = streamFile.ToArray();
            string base64 = Convert.ToBase64String(bytes);
            this.CodeBaseShared.ListDownloadModel.Add(new DownloadModel(base64, fileName));
        }

        public void DownloadByByte(byte[] bytes, string fileName)
        {
            string base64 = Convert.ToBase64String(bytes);
            this.CodeBaseShared.ListDownloadModel.Add(new DownloadModel(base64, fileName));
        }

        public List<sysBpmsDocument> GetList(Guid? documentDefID, Guid? documentFolderID, Guid? entityID, Guid? threadId)
        {
            return new DocumentService(this.UnitOfWork).GetList(documentDefID, null, entityID, "", false, documentFolderID, threadId);
        }

        public bool CheckMandatory(Guid? documentDefID, Guid? documentFolderID, Guid? entityID, Guid? threadId)
        {
            DocumentService documentService = new DocumentService(this.UnitOfWork);
            List<sysBpmsDocumentDef> listDef = documentDefID.HasValue ? new List<sysBpmsDocumentDef>() { new DocumentDefService(this.UnitOfWork).GetInfo(documentDefID.Value) } :
                new DocumentDefService(this.UnitOfWork).GetList(documentFolderID, "", "", true, null);
            foreach (var def in listDef)
            {
                if (def.IsMandatory && !documentService.GetList(def.ID, null, entityID, "", false, null, threadId).Any())
                    return false;
            }
            return true;
        }
    }
}
