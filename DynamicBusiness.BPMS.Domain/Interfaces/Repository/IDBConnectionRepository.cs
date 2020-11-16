using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DynamicBusiness.BPMS.Domain
{
    public interface IDBConnectionRepository
    {
        void Add(sysBpmsDBConnection DBConnection);
        void Update(sysBpmsDBConnection DBConnection);
        void Delete(Guid DBConnectionId);
        sysBpmsDBConnection GetInfo(Guid ID);
        List<sysBpmsDBConnection> GetList(string Name);
        List<sysBpmsDBConnection> GetList(string Name, string DataSource, string InitialCatalog, PagingProperties currentPaging);
    }
}
