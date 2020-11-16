using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IVariableDependencyRepository
    {
        void Add(sysBpmsVariableDependency variableDependency);
        void Update(sysBpmsVariableDependency variableDependency);
        void Delete(Guid ID);
        sysBpmsVariableDependency GetInfo(Guid ID);
        List<sysBpmsVariableDependency> GetList(Guid? DependentVariableID, Guid? ToVariableID);
        List<sysBpmsVariableDependency> GetList(Guid processId);
    }
}
