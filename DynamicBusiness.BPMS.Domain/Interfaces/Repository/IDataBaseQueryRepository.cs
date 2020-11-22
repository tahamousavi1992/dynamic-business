using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IDataBaseQueryRepository
    { 
        DataTable GetBySqlQuery(string sqlQuery, string Connectionstr, bool dbNullParams, PagingProperties currentPaging, params SqlParameter[] _params);
        DataTable GetBySqlQuery(string sqlQuery, bool dbNullParams, PagingProperties currentPaging, params SqlParameter[] _params);
        int ExecuteBySqlQuery(string sqlQuery, string Connectionstr, bool dbNullParams, params SqlParameter[] _params);
        int ExecuteBySqlQuery(string sqlQuery, bool dbNullParams, params SqlParameter[] _params);
        T ExecuteScalar<T>(string sqlQuery, bool dbNullParams, params SqlParameter[] _params);
        string GetEntityConnection(); 
    }
}
