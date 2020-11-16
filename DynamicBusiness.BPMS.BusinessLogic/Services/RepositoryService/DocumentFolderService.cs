using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DocumentFolderService : ServiceBase
    {
        public DocumentFolderService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsDocumentFolder documentFolder)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IDocumentFolderRepository>().Add(documentFolder);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsDocumentFolder documentFolder)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IDocumentFolderRepository>().Update(documentFolder);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(Guid documentFolderId)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                if (resultOperation.IsSuccess)
                {
                    //delete all sysBpmsDocumentDef.
                    List<sysBpmsDocumentDef> list = new DocumentDefService(base.UnitOfWork).GetList(documentFolderId, "", "", null, null);
                    foreach (sysBpmsDocumentDef item in list)
                    {
                        if (resultOperation.IsSuccess)
                            resultOperation = new DocumentDefService(base.UnitOfWork).Delete(item.ID);
                    }
                    //delete children
                    List<sysBpmsDocumentFolder> children = this.GetList(documentFolderId, "", "", null);
                    foreach (sysBpmsDocumentFolder item in children)
                    {
                        if (resultOperation.IsSuccess)
                            resultOperation = this.Delete(item.ID);
                    }

                    if (resultOperation.IsSuccess)
                    {
                        this.UnitOfWork.Repository<IDocumentFolderRepository>().Delete(documentFolderId);
                        this.UnitOfWork.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                return base.ExceptionHandler(ex);
            }
            base.FinalizeService(resultOperation);
            return resultOperation;
        }

        public sysBpmsDocumentFolder GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<IDocumentFolderRepository>().GetInfo(ID);
        }

        public List<sysBpmsDocumentFolder> GetList(Guid? ParentDocumentFolderID, string NameOf, string DisplayName, bool? isActive)
        {
            return this.UnitOfWork.Repository<IDocumentFolderRepository>().GetList(ParentDocumentFolderID, NameOf, DisplayName, isActive);
        }

        public TreeViewModel HelperGetTreeList(Guid? SelectedID)
        {
            TreeViewModel Item = new TreeViewModel() { id = "-1", text = "Folders", icon = "fa fa-folder text-primary", state = new TreeNodeStateModel() { opened = true, } };
            List<sysBpmsDocumentFolder> list = this.GetList(null, "", "", true);

            foreach (sysBpmsDocumentFolder item in list.Where(d => !d.DocumentFolderID.HasValue))
            {
                this.FillTree(item, Item.children, list, SelectedID);
            }

            return Item;
        }

        private void FillTree(sysBpmsDocumentFolder documentFolder, List<TreeViewModel> Items, List<sysBpmsDocumentFolder> documentFolders, Guid? SelectedID)
        {
            Items.Add(new TreeViewModel()
            {
                id = documentFolder.ID.ToString(),
                text = documentFolder.DisplayName,
                icon = documentFolders.Any(d => d.DocumentFolderID == documentFolder.ID) ? "fa fa-folder text-primary" : "fa fa-file  text-primary",
                state = SelectedID.HasValue && documentFolder.ID == SelectedID ? new TreeNodeStateModel() { selected = true } : null
            });
            foreach (sysBpmsDocumentFolder item in documentFolders.Where(d => d.DocumentFolderID == documentFolder.ID))
            {
                this.FillTree(item, Items.LastOrDefault().children, documentFolders, SelectedID);
            }
        }
    }
}
