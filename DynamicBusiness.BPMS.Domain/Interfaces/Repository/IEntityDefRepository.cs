using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IEntityDefRepository
    {
        void Add(sysBpmsEntityDef entityDef);
        void Update(sysBpmsEntityDef entityDef);
        void Delete(Guid entityDefId);
        sysBpmsEntityDef GetInfo(Guid ID);
        sysBpmsEntityDef GetInfo(string name);
        List<sysBpmsEntityDef> GetList(string tableName, string name, bool? isActive, PagingProperties currentPaging);
        List<string> GetList(bool? isActive);
    }
}
