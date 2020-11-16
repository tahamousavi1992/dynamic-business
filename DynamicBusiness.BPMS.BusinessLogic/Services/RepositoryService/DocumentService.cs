using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DocumentService : ServiceBase
    {
        public DocumentService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsDocument document)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IDocumentRepository>().Add(document);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsDocument document)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IDocumentRepository>().Update(document);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation InActive(Guid Guid)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                sysBpmsDocument document = this.GetInfo(Guid);
                document.IsDeleted = true;
                this.UnitOfWork.Repository<IDocumentRepository>().Update(document);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(Guid Guid)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IDocumentRepository>().Delete(Guid);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public sysBpmsDocument GetInfo(Guid Guid)
        {
            return this.UnitOfWork.Repository<IDocumentRepository>().GetInfo(Guid);
        }
         
        public List<sysBpmsDocument> GetList(Guid? DocumentDefID, Guid? EntityDefID, Guid? EntityID, string CaptionOf, bool? IsDeleted, Guid? DocumentFolderID, Guid? threadId)
        {
            return this.UnitOfWork.Repository<IDocumentRepository>().GetList(DocumentDefID, EntityDefID, EntityID, CaptionOf, IsDeleted, DocumentFolderID, threadId);
        }

      
    }
}
