using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IElementRepository
    {
        void Add(sysBpmsElement Element);
        void Update(sysBpmsElement Element);
        void Delete(string ElementId, Guid processId);
        sysBpmsElement GetInfo(string ID, Guid processId);
        List<sysBpmsElement> GetList(Guid ProcessID, int? TypeLU, string Name);
        List<sysBpmsElement> GetList(string[] ElementID, Guid processId);
    }
}
