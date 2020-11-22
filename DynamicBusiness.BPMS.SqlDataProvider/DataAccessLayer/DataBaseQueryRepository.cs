using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.BusinessLogic;
using System.Text.RegularExpressions;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DataBaseQueryRepository : IDataBaseQueryRepository
    {
        private Db_BPMSEntities Context;
        public DataBaseQueryRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }
         
        public DataTable GetBySqlQuery(string sqlQuery, string Connectionstr, bool dbNullParams, PagingProperties currentPaging, params SqlParameter[] _params)
        {
            if (dbNullParams)
                _params = this.DbNullNonSetParams(sqlQuery, _params);
            this.NullToDbNull(_params);
            DataTable dataTable = new DataTable();
            SqlDataReader reader = null;
            using (SqlConnection connection = new SqlConnection(Connectionstr))
            {
                if (currentPaging != null)
                {
                    string commandText = string.Empty;
                    SqlCommand command = null;
                    if (currentPaging.HasPaging)
                    {
                        string countQuery = (sqlQuery.Contains("order") && (!sqlQuery.ToLower().Contains("top") && !sqlQuery.ToLower().Contains("offset"))) ? $"{sqlQuery} OFFSET 0 ROWS " : sqlQuery;
                        //get row count
                        command = this.GetCommand($" select count(*) from ({countQuery}) as returntable ", connection, _params);
                        connection.Open();
                        currentPaging.RowsCount = command.ExecuteScalar().ToIntObj();
                        command.Dispose();
                        connection.Close();
                        //get data with paging and sorting
                        commandText = string.Format($@"{sqlQuery} { (!string.IsNullOrWhiteSpace(currentPaging.SortColumn) ? $"order by {currentPaging.SortColumn} {currentPaging.SortType}" : "")} OFFSET {((currentPaging.PageIndex - 1) * currentPaging.PageSize)} ROWS FETCH NEXT {currentPaging.PageSize} ROWS ONLY ");
                    }
                    else
                        //get data with sorting
                        commandText = string.Format($@"{sqlQuery} { (!string.IsNullOrWhiteSpace(currentPaging.SortColumn) ? $"order by {currentPaging.SortColumn} {currentPaging.SortType}" : "")} ");

                    command = this.GetCommand(commandText, connection, _params);
                    connection.Open();
                    reader = command.ExecuteReader();
                    dataTable.Load(reader);
                    reader.Close();
                }
                else
                {
                    SqlCommand command = this.GetCommand(sqlQuery, connection, _params);
                    connection.Open();

                    reader = command.ExecuteReader();
                    dataTable.Load(reader);
                    reader.Close();
                }

            }
            return dataTable;
        }

        public DataTable GetBySqlQuery(string sqlQuery, bool dbNullParams, PagingProperties currentPaging, params SqlParameter[] _params)
        {
            if (dbNullParams)
                _params = this.DbNullNonSetParams(sqlQuery, _params);
            this.NullToDbNull(_params);
            DataTable dataTable = new DataTable();
            System.Data.Common.DbDataReader reader = null;

            if (currentPaging != null)
            {
                System.Data.Common.DbCommand command = null;
                string commandText;
                if (currentPaging.HasPaging)
                {
                    string countQuery = (sqlQuery.Contains("order") && (!sqlQuery.ToLower().Contains("top") && !sqlQuery.ToLower().Contains("offset"))) ? $"{sqlQuery} OFFSET 0 ROWS " : sqlQuery;
                    //get row count
                    command = this.GetDbCommand($"select count(*) from ({countQuery}) as returntable ", _params);
                    currentPaging.RowsCount = command.ExecuteScalar().ToIntObj();
                    command.Dispose();

                    //get data with paging and sorting
                    commandText = string.Format($@"{sqlQuery} { (!string.IsNullOrWhiteSpace(currentPaging.SortColumn) ? $"order by {currentPaging.SortColumn} {currentPaging.SortType}" : "")} OFFSET {((currentPaging.PageIndex - 1) * currentPaging.PageSize)} ROWS FETCH NEXT {currentPaging.PageSize} ROWS ONLY ");
                }
                else
                    //get data with paging and sorting
                    commandText = string.Format($@"{sqlQuery} { (!string.IsNullOrWhiteSpace(currentPaging.SortColumn) ? $"order by {currentPaging.SortColumn} {currentPaging.SortType}" : "")}");

                command = this.GetDbCommand(commandText, _params);
                reader = command.ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }
            else
            {
                System.Data.Common.DbCommand command = this.GetDbCommand(sqlQuery, _params);
                reader = this.GetDbCommand(sqlQuery, _params).ExecuteReader();
                dataTable.Load(reader);
                reader.Close();
            }


            return dataTable;
        }

        public int ExecuteBySqlQuery(string sqlQuery, string Connectionstr, bool dbNullParams, params SqlParameter[] _params)
        {
            if (dbNullParams)
                _params = this.DbNullNonSetParams(sqlQuery, _params);
            this.NullToDbNull(_params);
            System.Data.Common.DbCommand command = this.Context.Database.Connection.CreateCommand();
            command.CommandText = sqlQuery;

            if (this.Context.Database.CurrentTransaction != null)
                command.Transaction = this.Context.Database.CurrentTransaction.UnderlyingTransaction;

            if (_params != null && _params.Any())
                command.Parameters.AddRange(_params);

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            return command.ExecuteNonQuery();
        }

        public int ExecuteBySqlQuery(string sqlQuery, bool dbNullParams, params SqlParameter[] _params)
        {
            if (dbNullParams)
                _params = this.DbNullNonSetParams(sqlQuery, _params);
            this.NullToDbNull(_params);
            if (_params != null && _params.Any())
                return this.Context.Database.ExecuteSqlCommand(sqlQuery, _params);
            else
                return this.Context.Database.ExecuteSqlCommand(sqlQuery);
        }

        public T ExecuteScalar<T>(string sqlQuery, bool dbNullParams, params SqlParameter[] _params)
        {
            if (dbNullParams)
                _params = this.DbNullNonSetParams(sqlQuery, _params);
            this.NullToDbNull(_params);
            if (_params != null && _params.Any())
                return this.Context.Database.SqlQuery<T>(sqlQuery, _params).FirstOrDefault();
            else
                return this.Context.Database.SqlQuery<T>(sqlQuery).FirstOrDefault();
        }

        public string GetEntityConnection()
        {
            return this.Context.Database.Connection.ConnectionString;
        } 
        private SqlParameter[] DbNullNonSetParams(string sqlQuery, SqlParameter[] _params)
        {
            if (!string.IsNullOrWhiteSpace(sqlQuery) && _params != null)
            {
                List<string> queryParams = Regex.Matches(sqlQuery, @"\@\w+").Cast<Match>().Select(m => m.Value).ToList();
                foreach (string parameter in queryParams)
                {
                    if (!_params.Any(c => c.ParameterName == parameter) && Regex.Matches(sqlQuery, "declare(\\s*?)" + parameter).Count == 0)
                        _params = _params.Union(new SqlParameter[] { new SqlParameter(parameter, DBNull.Value) }).ToArray();
                }
            }
            return _params;
        }
        private void NullToDbNull(SqlParameter[] _params)
        {
            if (_params != null)
            {
                foreach (SqlParameter parameter in _params)
                {
                    if (parameter.Value == null)
                        parameter.Value = DBNull.Value;
                }
            }
        }

        private SqlCommand GetCommand(string sqlQuery, SqlConnection connection, params SqlParameter[] _params)
        {
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            //add parameter to command
            if (_params != null && _params.Any())
                command.Parameters.AddRange(_params);
            return command;
        }
        private System.Data.Common.DbCommand GetDbCommand(string sqlQuery, params SqlParameter[] _params)
        {
            System.Data.Common.DbCommand command = this.Context.Database.Connection.CreateCommand();
            command.CommandText = sqlQuery;
            //add parameter to command
            if (_params != null && _params.Any())
                command.Parameters.AddRange(_params.Select(c => new SqlParameter(c.ParameterName, c.Value)).ToArray());

            if (this.Context.Database.CurrentTransaction != null)
                command.Transaction = this.Context.Database.CurrentTransaction.UnderlyingTransaction;

            if (command.Connection.State != ConnectionState.Open)
                command.Connection.Open();

            return command;
        }
    }
}
