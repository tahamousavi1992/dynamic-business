using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IThreadRepository
    {
        void Add(sysBpmsThread Thread);
        void Update(sysBpmsThread Thread);
        sysBpmsThread GetInfo(Guid ID, string[] includes = null);
        int GetCountActive(Guid UserId, Guid? ProcessID);
        int GetCount(Guid processID);
        void Delete(Guid ID);
        List<sysBpmsThread> GetArchiveList(Guid? TaskOwnerUserID, Guid? ProcessID, int[] statusLU, Guid? UserID, DateTime? StartFrom, DateTime? StartTo, DateTime? EndFrom, DateTime? EndTo, PagingProperties currentPaging, string[] includes);
        int MaxNumber();
    }
}
