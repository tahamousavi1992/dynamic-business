using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface ITaskRepository
    {
        void Add(sysBpmsTask Task);
        void Update(sysBpmsTask Task);
        void Delete(Guid id);
        sysBpmsTask GetInfo(Guid id);
        sysBpmsTask GetInfo(string elementId, Guid processId);
        List<sysBpmsTask> GetList(int? typeLU, Guid? processID);
    }
}
