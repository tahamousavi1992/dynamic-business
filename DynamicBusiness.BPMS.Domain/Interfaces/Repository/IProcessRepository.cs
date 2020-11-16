using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IProcessRepository
    {
        void Add(sysBpmsProcess Process);
        void Update(sysBpmsProcess Process);
        void Delete(Guid ID);
        sysBpmsProcess GetInfo(Guid ID);
        List<sysBpmsProcess> GetList(DateTime? StartDate, DateTime? EndDate, Guid? processGroupId, PagingProperties currentPaging);
        List<sysBpmsProcess> GetList(int? statusLU, Guid? parentProcessId);
        sysBpmsProcess GetLastActive(Guid parentProcessId);
        List<string> GetListBeginTaskElementID(Guid ID);
        /// <summary>
        /// include element.process
        /// </summary>
        List<sysBpmsTask> GetAvailableProccess(Guid UserID);
        int MaxNumber();
    }
}
