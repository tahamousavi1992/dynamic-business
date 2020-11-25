using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ApplicationPageAccessService : ServiceBase
    {
        public ApplicationPageAccessService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsApplicationPageAccess applicationPageAccess)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IApplicationPageAccessRepository>().Add(applicationPageAccess);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsApplicationPageAccess applicationPageAccess)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IApplicationPageAccessRepository>().Update(applicationPageAccess);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation AddEdit(Guid ApplicationPageID, List<sysBpmsApplicationPageAccess> newApplicationPageAccess)
        {
            ResultOperation resultOperation = new ResultOperation();

            List<sysBpmsApplicationPageAccess> listCurrent = this.GetList(ApplicationPageID, null);
            foreach (sysBpmsApplicationPageAccess item in listCurrent.Where(c => !newApplicationPageAccess.Any(d => d.ID == c.ID)))
            {
                if (resultOperation.IsSuccess)
                {
                    this.UnitOfWork.Repository<IApplicationPageAccessRepository>().Delete(item.ID);
                }
            }
            foreach (sysBpmsApplicationPageAccess item in newApplicationPageAccess)
            {
                if (resultOperation.IsSuccess)
                {
                    resultOperation = new ResultOperation();
                    if (resultOperation.IsSuccess)
                    {
                        if (item.ID != Guid.Empty)
                            this.UnitOfWork.Repository<IApplicationPageAccessRepository>().Update(item);
                        else
                            this.UnitOfWork.Repository<IApplicationPageAccessRepository>().Add(item);
                    }
                }
            }

            if (resultOperation.IsSuccess)
                this.UnitOfWork.Save();

            return resultOperation;
        }

        public ResultOperation Delete(Guid applicationPageAccessId)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IApplicationPageAccessRepository>().Delete(applicationPageAccessId);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public sysBpmsApplicationPageAccess GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<IApplicationPageAccessRepository>().GetInfo(ID);
        }
        public List<sysBpmsApplicationPageAccess> GetList(Guid departmentId)
        {
            return this.UnitOfWork.Repository<IApplicationPageAccessRepository>().GetList(departmentId);
        }
        public List<sysBpmsApplicationPageAccess> GetList(Guid? applicationPageID, PagingProperties currentPaging)
        {
            return this.UnitOfWork.Repository<IApplicationPageAccessRepository>().GetList(applicationPageID, currentPaging);
        }
    }
}
