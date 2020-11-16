using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DocumentDefService : ServiceBase
    {
        public DocumentDefService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsDocumentDef documentDef)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IDocumentDefRepository>().Add(documentDef);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsDocumentDef documentDef)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IDocumentDefRepository>().Update(documentDef);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(Guid DocumentDefId)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                //delete documents
                DocumentService documentService = new DocumentService(base.UnitOfWork);
                List<sysBpmsDocument> list = documentService.GetList(DocumentDefId, null, null, "", null, null, null);
                foreach (sysBpmsDocument item in list)
                {
                    if (resultOperation.IsSuccess)
                        resultOperation = documentService.Delete(item.GUID);
                }

                this.UnitOfWork.Repository<IDocumentDefRepository>().Delete(DocumentDefId);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public sysBpmsDocumentDef GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<IDocumentDefRepository>().GetInfo(ID);
        }

        public List<sysBpmsDocumentDef> GetList(Guid? DocumentFolderID, string NameOf, string DisplayName, bool? IsActive, bool? IsSystemic, PagingProperties currentPaging = null)
        {
            return this.UnitOfWork.Repository<IDocumentDefRepository>().GetList(DocumentFolderID, NameOf, DisplayName, IsActive, IsSystemic, currentPaging);
        }

        /// <summary>
        /// this method make a list of DocumentDefs name and Entity.PropertyName from DocumentDef which are object.
        /// </summary>
        public List<ComboTreeModel> HelperGetTreeList(bool forCodding = false)
        {
            List<ComboTreeModel> Items = new List<ComboTreeModel>();
            List<sysBpmsDocumentFolder> folders = new DocumentFolderService(this.UnitOfWork).GetList(null, "", "", true);
            List<sysBpmsDocumentDef> Defs = new DocumentDefService(base.UnitOfWork).GetList(null, "", "", true, null);
            foreach (sysBpmsDocumentFolder item in folders)
            {
                Items.Add(new ComboTreeModel() { title = item.DisplayName, id = item.ID.ToString(), state = "closed", });
                foreach (sysBpmsDocumentDef documentDef in Defs.Where(c => c.DocumentFolderID == item.ID))
                {
                    Items.LastOrDefault().subs.Add(new ComboTreeModel()
                    {
                        title = documentDef.DisplayName,
                        id = documentDef.ID.ToString(),
                    });
                }
            }

            return Items;
        }

    }
}
