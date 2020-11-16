using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IOperationRepository
    {
        void Add(sysBpmsOperation operation);
        void Update(sysBpmsOperation operation);
        void Delete(Guid id);
        sysBpmsOperation GetInfo(Guid id);
        List<sysBpmsOperation> GetList(int? GroupLU, PagingProperties currentPaging);
    }
}
