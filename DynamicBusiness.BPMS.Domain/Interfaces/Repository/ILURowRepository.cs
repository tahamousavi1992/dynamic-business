using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface ILURowRepository
    {
        int MaxOrderByLUTableID(Guid LUTableID);
        int MaxCodeOfByLUTableID(Guid LUTableID);
        List<sysBpmsLURow> GetList(Guid LUTableID, string NameOf, bool? IsActive);
        List<sysBpmsLURow> GetList(Guid LUTableID, string NameOf, bool? IsActive, PagingProperties currentPaging);
        List<sysBpmsLURow> GetList(string Alias);
        sysBpmsLURow GetInfo(Guid ID);
        sysBpmsLURow GetInfo(string Alias, string CodeOf);
        sysBpmsLURow GetInfoByName(string Alias, string NameOf);
        sysBpmsLURow GetInfo(Guid ID, Guid LUTableID, string CodeOf);
        string GetNameOfByAlias(string Alias, string CodeOf);
        void Update(sysBpmsLURow LURow);
        void Add(sysBpmsLURow LURow);
    }
}
