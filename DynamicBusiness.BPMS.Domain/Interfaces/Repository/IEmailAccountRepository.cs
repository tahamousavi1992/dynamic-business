using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IEmailAccountRepository
    {
        void Add(sysBpmsEmailAccount emailAccount);
        void Update(sysBpmsEmailAccount emailAccount);
        void Delete(Guid emailAccountId);
        sysBpmsEmailAccount GetInfo(Guid ID);
        List<sysBpmsEmailAccount> GetList(int? ObjectTypeLU, Guid? ObjectID, PagingProperties currentPaging);
    }
}
