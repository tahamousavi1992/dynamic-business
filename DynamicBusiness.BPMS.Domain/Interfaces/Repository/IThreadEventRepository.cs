using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IThreadEventRepository
    {
        void Add(sysBpmsThreadEvent threadEvent);
        void Update(sysBpmsThreadEvent threadEvent);
        void Delete(Guid threadEventId);
        sysBpmsThreadEvent GetInfo(Guid ID, string[] Includes);
        List<sysBpmsThreadEvent> GetTimerActive(string[] Includes);
        List<sysBpmsThreadEvent> GetMessageActive(Guid notProcessID, Guid messageTypeID, string[] Includes);
        List<sysBpmsThreadEvent> GetActive(Guid threadId);
        List<sysBpmsThreadEvent> GetList(Guid threadId);
        int GetCount(Guid? threadId, Guid eventID, Guid? ProcessID, int? statusLU, string[] Includes);
        sysBpmsThreadEvent GetLastExecuted(Guid? threadId, Guid? ProcessID, Guid? eventID, string[] Includes);
    }
}
