using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IThreadTaskRepository
    {
        void Add(sysBpmsThreadTask ThreadTask);
        void Update(sysBpmsThreadTask ThreadTask);
        sysBpmsThreadTask GetInfo(Guid ID, string[] Includes);
        bool HasAny(Guid processId, Guid taskId);
        void Delete(Guid ID);
        List<sysBpmsThreadTask> GetList(Guid ThreadID, int? TaskType, Guid? taskId, int? statusLU, string[] Includes);
        List<sysBpmsThreadTask> GetListRunning(Guid ThreadID);
        List<sysBpmsThreadTask> GetListKartable(Guid UserID, int[] statusLU, PagingProperties currentPaging);

    }
}
