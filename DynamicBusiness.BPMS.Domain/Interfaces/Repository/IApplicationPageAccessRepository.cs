using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IApplicationPageAccessRepository
    {
        void Add(sysBpmsApplicationPageAccess applicationPageAccess);
        void Update(sysBpmsApplicationPageAccess applicationPageAccess);
        void Delete(Guid id);
        sysBpmsApplicationPageAccess GetInfo(Guid id);
        List<sysBpmsApplicationPageAccess> GetList(Guid? applicationPageID, PagingProperties currentPaging);
        List<sysBpmsApplicationPageAccess> GetList(Guid departmentId);
        bool GetUserAccess(Guid? userId, Guid applicationPageID, ElementBase.e_AccessType e_AccessType); 
    }
}
