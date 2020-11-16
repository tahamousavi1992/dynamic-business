using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IVariableRepository
    {
        void Add(sysBpmsVariable Variable);
        void Update(sysBpmsVariable Variable);
        void Delete(Guid VariableId);
        sysBpmsVariable GetInfo(Guid ID, string[] includes);
        sysBpmsVariable GetInfo(Guid? ProcessID, Guid? ApplicationPageID, string Name, string[] includes);
        List<sysBpmsVariable> GetList(Guid? ProcessID, Guid? ApplicationPageID, int? VarTypeLU, string Name, Guid? EntityDefID, bool? EntityIsActive, string[] includes);
        List<sysBpmsVariable> GetList(Guid dbConnectionID);
    }
}
