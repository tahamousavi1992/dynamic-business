using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IThreadVariableRepository
    {
        void Add(sysBpmsThreadVariable ThreadVariable);
        void Update(sysBpmsThreadVariable ThreadVariable);
        void Delete(Guid ID);
        sysBpmsThreadVariable GetInfo(Guid ID);
        sysBpmsThreadVariable GetInfo(Guid ThreadID, Guid VariableID);
        List<sysBpmsThreadVariable> GetList(Guid? ThreadID, Guid? VariableID, string Value);
    }
}
