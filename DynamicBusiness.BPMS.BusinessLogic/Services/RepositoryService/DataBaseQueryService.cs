using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DataBaseQueryService : ServiceBase
    {
        public DataBaseQueryService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

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

        public ResultOperation UpdatedSqlQuery(List<string> querirs)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                this.BeginTransaction();
                foreach (string query in querirs)
                {
                    if (query.Trim() != "SET ANSI_NULLS ON" && query.Trim() != "SET QUOTED_IDENTIFIER")
                        this.ExecuteBySqlQuery(query, false, null);
                }
                this.UnitOfWork.Save();

            }
            catch (Exception ex)
            {
                return base.ExceptionHandler(ex);
            }
            base.FinalizeService(resultOperation);
            return resultOperation;
        }
    }
}
