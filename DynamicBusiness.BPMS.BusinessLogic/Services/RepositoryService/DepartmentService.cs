using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DepartmentService : ServiceBase
    {
        public DepartmentService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }
        public ResultOperation Add(sysBpmsDepartment department, sysBpmsEmailAccount emailAccount)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                this.BeginTransaction();
                if (resultOperation.IsSuccess)
                {
                    this.UnitOfWork.Repository<IDepartmentRepository>().Add(department);
                    this.UnitOfWork.Save();
                    if (emailAccount != null)
                    {
                        emailAccount.ObjectID = department.ID;
                        resultOperation = new EmailAccountService(base.UnitOfWork).AddOverwrite(emailAccount);
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

        public ResultOperation Update(sysBpmsDepartment Department, sysBpmsEmailAccount emailAccount)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                this.BeginTransaction();
                if (resultOperation.IsSuccess)
                {
                    this.UnitOfWork.Repository<IDepartmentRepository>().Update(Department);
                    this.UnitOfWork.Save();

                    if (emailAccount != null)
                    {
                        emailAccount.ObjectID = Department.ID;
                        resultOperation = new EmailAccountService(base.UnitOfWork).AddOverwrite(emailAccount);
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

        public ResultOperation Delete(Guid DepartmentId)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IDepartmentRepository>().Delete(DepartmentId);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public sysBpmsDepartment GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<IDepartmentRepository>().GetInfo(ID);
        }

        public List<sysBpmsDepartment> GetList(bool? IsActive, string Name, Guid? ParentDepartmentID)
        {
            return this.UnitOfWork.Repository<IDepartmentRepository>().GetList(IsActive, Name, ParentDepartmentID);
        }
         
        public TreeViewModel HelperGetTreeList(Guid? SelectedID)
        {
            TreeViewModel Item = new TreeViewModel() { id = "-1", text = "Organizations", icon = "fa fa-folder text-primary", state = new TreeNodeStateModel() { opened = true, } };
            List<sysBpmsDepartment> departments = this.GetList(true, "", null);

            foreach (sysBpmsDepartment item in departments.Where(d => !d.DepartmentID.HasValue))
            {
                this.FillTree(item, Item.children, departments, SelectedID);
            }
            return Item;
        }

        private void FillTree(sysBpmsDepartment department, List<TreeViewModel> Items, List<sysBpmsDepartment> departments, Guid? SelectedID)
        {
            Items.Add(new TreeViewModel()
            {
                id = department.ID.ToString(),
                text = department.Name,
                icon = departments.Any(d => d.DepartmentID == department.ID) ? "fa fa-folder text-primary" : "fa fa-file  text-primary",
                state = SelectedID.HasValue && department.ID == SelectedID ? new TreeNodeStateModel() { selected = true } : null
            });
            foreach (sysBpmsDepartment item in departments.Where(d => d.DepartmentID == department.ID))
            {
                this.FillTree(item, Items.LastOrDefault().children, departments, SelectedID);
            }
        }
    }
}
