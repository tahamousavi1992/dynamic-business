using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IAPIAccessRepository
    {
        void Add(sysBpmsAPIAccess apiAccess);
        void Update(sysBpmsAPIAccess apiAccess);
        sysBpmsAPIAccess GetInfo(Guid ID);
        sysBpmsAPIAccess GetInfo(string IPAddress, string AccessKey);
        List<sysBpmsAPIAccess> GetList(string Name, string IPAddress, string AccessKey, bool? IsActive, PagingProperties currentPaging);

    }
}
