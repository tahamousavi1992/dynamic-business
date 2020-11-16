using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ApplicationPageService : ServiceBase
    {
        public ApplicationPageService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsApplicationPage applicationPage)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IApplicationPageRepository>().Add(applicationPage);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsApplicationPage applicationPage)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IApplicationPageRepository>().Update(applicationPage);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(Guid applicationPageId)
        {
            ResultOperation resultOperation = null;
            try
            {
                resultOperation = new ResultOperation();
                if (resultOperation.IsSuccess)
                {
                    base.BeginTransaction();
                    List<sysBpmsApplicationPageAccess> list = new ApplicationPageAccessService(this.UnitOfWork).GetList(applicationPageId, null);
                    foreach (var item in list)
                    {
                        resultOperation = new ApplicationPageAccessService(this.UnitOfWork).Delete(item.ID);
                        if (!resultOperation.IsSuccess)
                            break;
                    }
                    if (resultOperation.IsSuccess)
                    {
                        this.UnitOfWork.Repository<IApplicationPageRepository>().Delete(applicationPageId);
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

        public sysBpmsApplicationPage GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<IApplicationPageRepository>().GetInfo(ID);
        }

        public List<sysBpmsApplicationPage> GetList(Guid? dynamicFormID, int? groupLU)
        {
            return this.UnitOfWork.Repository<IApplicationPageRepository>().GetList(dynamicFormID, groupLU);
        }
    }
}
