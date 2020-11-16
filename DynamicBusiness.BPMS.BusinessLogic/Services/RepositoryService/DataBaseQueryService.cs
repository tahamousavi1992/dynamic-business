using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DataBaseQueryService : ServiceBase
    {
        public DataBaseQueryService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public List<TableSchemaModel> GetTableSchema(string TabelName)
        {
            return this.UnitOfWork.Repository<IDataBaseQueryRepository>().GetTableSchema(TabelName);
        }

        public DataTable GetBySqlQuery(string sqlQuery, string Connectionstr, bool dbNullParams, PagingProperties currentPaging, params SqlParameter[] _params)
        {
            return this.UnitOfWork.Repository<IDataBaseQueryRepository>().GetBySqlQuery(sqlQuery, Connectionstr, dbNullParams, currentPaging, _params);
        }

        public DataTable GetBySqlQuery(string sqlQuery, bool dbNullParams, PagingProperties currentPaging, params SqlParameter[] _params)
        {
            return this.UnitOfWork.Repository<IDataBaseQueryRepository>().GetBySqlQuery(sqlQuery, dbNullParams, currentPaging, _params);
        }

        public int ExecuteBySqlQuery(string sqlQuery, string Connectionstr, bool dbNullParams, params SqlParameter[] _params)
        {
            return this.UnitOfWork.Repository<IDataBaseQueryRepository>().ExecuteBySqlQuery(sqlQuery, Connectionstr, dbNullParams, _params);
        }

        public int ExecuteBySqlQuery(string sqlQuery, bool dbNullParams, params SqlParameter[] _params)
        {
            return this.UnitOfWork.Repository<IDataBaseQueryRepository>().ExecuteBySqlQuery(sqlQuery, dbNullParams, _params);
        }

        public T ExecuteScalar<T>(string sqlQuery, bool dbNullParams, params SqlParameter[] _params)
        {
            return this.UnitOfWork.Repository<IDataBaseQueryRepository>().ExecuteScalar<T>(sqlQuery, dbNullParams, _params);
        }

        public string GetEntityConnection()
        {
            return this.UnitOfWork.Repository<IDataBaseQueryRepository>().GetEntityConnection();
        }
 
    }
}
