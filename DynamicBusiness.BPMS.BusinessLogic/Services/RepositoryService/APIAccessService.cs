using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class APIAccessService : ServiceBase
    {
        public APIAccessService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsAPIAccess APIAccess)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IAPIAccessRepository>().Add(APIAccess);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsAPIAccess APIAccess)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IAPIAccessRepository>().Update(APIAccess);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public sysBpmsAPIAccess GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<IAPIAccessRepository>().GetInfo(ID);
        }

        public sysBpmsAPIAccess GetInfo(string IPAddress, string AccessKey)
        {
            return this.UnitOfWork.Repository<IAPIAccessRepository>().GetInfo(IPAddress, AccessKey);
        }

        public List<sysBpmsAPIAccess> GetList(string Name, string IPAddress, string AccessKey, bool? IsActive, PagingProperties currentPaging = null)
        {
            return this.UnitOfWork.Repository<IAPIAccessRepository>().GetList(Name, IPAddress, AccessKey, IsActive, currentPaging);
        }

    }
}
