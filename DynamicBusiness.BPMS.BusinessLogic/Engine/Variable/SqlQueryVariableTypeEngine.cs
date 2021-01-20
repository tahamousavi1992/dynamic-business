using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class SqlQueryVariableTypeEngine : VariableTypeEngineBase
    {
        public SqlQueryVariableTypeEngine(EngineSharedModel engineSharedModel, sysBpmsVariable variable, Guid? processID, Guid? threadID, List<QueryModel> additionalParams, IUnitOfWork unitOfWork = null) : base(engineSharedModel, variable, processID, threadID, additionalParams, unitOfWork)
        {
        }

        /// <param name="containerQuery">It is generally used in combosearch which add a parent query that filter table's rows according to query parameter and text field</param>
        public List<DataModel> GetResult(PagingProperties currentPaging, string containerQuery = null)
        {
            string ColunName = string.Empty;
            string sqlQuery = Variable.Query;
            List<DataModel> dataModel = new List<DataModel>();
            string connection = Variable.DBConnectionID.HasValue ?
                new DBConnectionService(this.UnitOfWork).GetInfo(Variable.DBConnectionID.Value).GetConnection : "";

            //If containerQuery  is not null 
            if (!string.IsNullOrWhiteSpace(containerQuery))
            {
                sqlQuery = (sqlQuery.Contains("order") && (!sqlQuery.ToLower().Contains("top") && !sqlQuery.ToLower().Contains("offset"))) ? $"{sqlQuery} OFFSET 0 ROWS " : sqlQuery;
                sqlQuery = string.Format(containerQuery, sqlQuery);
            }
            //get sql query parameter from listFormQueryModel
            List<SqlParameter> queryParams = QueryModel.GetSqlParameter(base.GetAllQueryModels(base.AdditionalParams), sqlQuery).ToList();
            //add default parameters like username, userId and processId ,...
            this.AddDefaultParams(sqlQuery, queryParams);

            this.AddVariableParameters(ref sqlQuery, queryParams);
            DataTable dataTable = string.IsNullOrWhiteSpace(connection) ?
                new DataBaseQueryService(this.UnitOfWork).GetBySqlQuery(sqlQuery, true, currentPaging, queryParams.ToArray()) :
                new DataBaseQueryService(this.UnitOfWork).GetBySqlQuery(sqlQuery, connection, true, currentPaging, queryParams.ToArray());

            //if variable is bind to one column of query this set ColunName to variable name because when getting data it get data by variable name not column name.
            if (this.Variable.IsBindToOneData)
                ColunName = this.Variable.Name;

            foreach (DataRow _row in dataTable.Rows)
            {
                dataModel.Add(new DataModel(_row, ColunName));
            }
            return dataModel;
        }

        public override ResultOperation SaveValues(DataModel _DataModel, Dictionary<string, DataModel> allSavedEntities = null)
        {
            ResultOperation resultOperation = new ResultOperation();
            List<DataModel> dataModel = new List<DataModel>();
            string connection = new DBConnectionService(this.UnitOfWork).GetInfo(Variable.DBConnectionID.Value).GetConnection;
            string sqlQuery = Variable.Query;
            //get sql query parameter from listFormQueryModel
            List<SqlParameter> queryParams = QueryModel.GetSqlParameter(base.GetAllQueryModels(base.AdditionalParams), sqlQuery).ToList();
            //add default parameters like username, userId and processId ,...
            this.AddDefaultParams(sqlQuery, queryParams);

            this.AddVariableParameters(ref sqlQuery, queryParams);
            if (new DataBaseQueryService(this.UnitOfWork).ExecuteBySqlQuery(sqlQuery, connection, true, queryParams.ToArray()) > 0)
                return new ResultOperation();
            else
            {
                resultOperation.AddError(string.Format("Error running {0} variable script.", Variable.Name));
                return resultOperation;
            }
        }

        private void AddVariableParameters(ref string sqlQuery, List<SqlParameter> queryParams)
        {
            if (!string.IsNullOrWhiteSpace(sqlQuery))
            {
                IDataManageEngine dataManageEngine = new DataManageEngine(base.EngineSharedModel, base.UnitOfWork);
                List<string> extractedParams = DomainUtility.GetParameters(sqlQuery);
                foreach (string parameter in extractedParams)
                {
                    if (!queryParams.Any(c => c.ParameterName == parameter.Replace(".", "_")) && Regex.Matches(sqlQuery, "declare(\\s*?)" + parameter).Count == 0)
                    {
                        var result = dataManageEngine.GetValueByBinding(parameter.TrimStart('@'));
                        if (result != null)
                        {
                            queryParams.Add(new SqlParameter(parameter.Replace(".", "_"), result));
                            sqlQuery = sqlQuery.Replace(parameter, parameter.Replace(".", "_"));
                        }
                    }
                }
            }
        }

        private void AddDefaultParams(string sqlQuery, List<SqlParameter> queryParams)
        {
            if (sqlQuery.Contains("@ThreadID") && !queryParams.Any(c => c.ParameterName == "@ThreadID"))
                queryParams.Add(new SqlParameter("@ThreadID", base.ThreadID));
            if (sqlQuery.Contains("@ProcessID") && !queryParams.Any(c => c.ParameterName == "@ProcessID"))
                queryParams.Add(new SqlParameter("@ProcessID", base.ProcessID));
            if (sqlQuery.Contains("@UserID") && !queryParams.Any(c => c.ParameterName == "@UserID"))
                queryParams.Add(new SqlParameter("@UserID", new UserService(base.UnitOfWork).GetInfo(base.EngineSharedModel?.CurrentUserName)?.ID));
            if (sqlQuery.Contains("@UserName") && !queryParams.Any(c => c.ParameterName == "@UserName"))
                queryParams.Add(new SqlParameter("@UserName", base.EngineSharedModel?.CurrentUserName));
        }

    }
}
