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
        public ResultOperation Delete(Guid id)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IAPIAccessRepository>().Delete(id);
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

        public bool HasAccess(string ipAddress, string accessKey)
        {
            return this.UnitOfWork.Repository<IAPIAccessRepository>().HasAccess(ipAddress, accessKey);
        }

        public List<sysBpmsAPIAccess> GetList(string Name, string IPAddress, string AccessKey, bool? IsActive, PagingProperties currentPaging = null)
        {
            return this.UnitOfWork.Repository<IAPIAccessRepository>().GetList(Name, IPAddress, AccessKey, IsActive, currentPaging);
        }

    }
}
