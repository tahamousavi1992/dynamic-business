using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IUserRepository
    {
        void Add(sysBpmsUser user);
        void Update(sysBpmsUser user);
        void Delete(Guid UserId);
        sysBpmsUser GetInfo(Guid ID);
        sysBpmsUser GetInfo(string Username);
        List<sysBpmsUser> GetList(string name, PagingProperties currentPaging);
        List<sysBpmsUser> GetList(string name, int? roleCode, Guid? departmentId, PagingProperties currentPaging);
    }
}
