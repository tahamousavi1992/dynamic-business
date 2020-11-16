using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ProcessGroupService : ServiceBase
    {
        public ProcessGroupService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsProcessGroup ProcessGroup)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                this.BeginTransaction();
                if (resultOperation.IsSuccess)
                {
                    this.UnitOfWork.Repository<IProcessGroupRepository>().Add(ProcessGroup);
                    this.UnitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
                return base.ExceptionHandler(ex);
            }
            base.FinalizeService(resultOperation);
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsProcessGroup ProcessGroup)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                this.BeginTransaction();
                if (resultOperation.IsSuccess)
                {
                    this.UnitOfWork.Repository<IProcessGroupRepository>().Update(ProcessGroup);
                    this.UnitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
                return base.ExceptionHandler(ex);
            }
            base.FinalizeService(resultOperation);
            return resultOperation;
        }

        public ResultOperation Delete(Guid ProcessGroupId)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                if (new ProcessService(base.UnitOfWork).GetList(null, null, ProcessGroupId, null).Any())
                    resultOperation.AddError(LangUtility.Get("DeleteError.Text", nameof(sysBpmsProcessGroup)));
                if (resultOperation.IsSuccess)
                {
                    this.UnitOfWork.Repository<IProcessGroupRepository>().Delete(ProcessGroupId);
                    this.UnitOfWork.Save();
                }
            }
            return resultOperation;
        }

        public sysBpmsProcessGroup GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<IProcessGroupRepository>().GetInfo(ID);
        }

        public List<sysBpmsProcessGroup> GetList(string Name, Guid? ParentProcessGroupID)
        {
            return this.UnitOfWork.Repository<IProcessGroupRepository>().GetList(Name, ParentProcessGroupID);
        }

        public TreeViewModel HelperGetTreeList(Guid? SelectedID)
        {
            TreeViewModel Item = new TreeViewModel() { id = "-1", text = "Groups", icon = "fa fa-folder text-primary", state = new TreeNodeStateModel() { opened = true, } };
            List<sysBpmsProcessGroup> ProcessGroups = this.GetList("", null);

            foreach (sysBpmsProcessGroup item in ProcessGroups.Where(d => !d.ProcessGroupID.HasValue))
            {
                this.FillTree(item, Item.children, ProcessGroups, SelectedID);
            }
            return Item;
        }

        private void FillTree(sysBpmsProcessGroup ProcessGroup, List<TreeViewModel> Items, List<sysBpmsProcessGroup> ProcessGroups, Guid? SelectedID)
        {
            Items.Add(new TreeViewModel()
            {
                id = ProcessGroup.ID.ToString(),
                text = ProcessGroup.Name,
                icon = ProcessGroups.Any(d => d.ProcessGroupID == ProcessGroup.ID) ? "fa fa-folder text-primary" : "fa fa-file  text-primary",
                state = SelectedID.HasValue && ProcessGroup.ID == SelectedID ? new TreeNodeStateModel() { selected = true } : null
            });
            foreach (sysBpmsProcessGroup item in ProcessGroups.Where(d => d.ProcessGroupID == ProcessGroup.ID))
            {
                this.FillTree(item, Items.LastOrDefault().children, ProcessGroups, SelectedID);
            }
        }
    }
}
