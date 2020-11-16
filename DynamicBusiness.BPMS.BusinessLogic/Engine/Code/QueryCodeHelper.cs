using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class QueryCodeHelper
    {
        public IUnitOfWork UnitOfWork { get; set; }
        public QueryCodeHelper(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        public DataTable Get(string sqlQuery, params QueryModel[] _params)
        {
            return new DataBaseQueryService(this.UnitOfWork).GetBySqlQuery(sqlQuery, true, null, QueryModel.GetSqlParameter(_params?.ToList())?.ToArray());
        }

        public int Execute(string sqlQuery, params QueryModel[] _params)
        {
            return new DataBaseQueryService(this.UnitOfWork).ExecuteBySqlQuery(sqlQuery, true, QueryModel.GetSqlParameter(_params?.ToList())?.ToArray());
        }

        public T ExecuteScalar<T>(string sqlQuery, params QueryModel[] _params)
        {
            return new DataBaseQueryService(this.UnitOfWork).ExecuteScalar<T>(sqlQuery, true, QueryModel.GetSqlParameter(_params?.ToList())?.ToArray());
        }

    }
}
