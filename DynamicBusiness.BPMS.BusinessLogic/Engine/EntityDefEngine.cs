using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class EntityDefEngine : BaseEngine
    {
        public EntityDefEngine(EngineSharedModel engineSharedModel, IUnitOfWork unitOfWork = null) : base(engineSharedModel, unitOfWork) { }

        /// <summary>
        /// this method save entityDef after form post action.
        /// </summary>
        public Tuple<ResultOperation, Guid?> SaveIntoDataBase(sysBpmsEntityDef entityDef, sysBpmsVariable variable, DataModel dataModel, List<QueryModel> additionalParams, Dictionary<string, DataModel> allSavedEntities)
        {
            ResultOperation resultOperation = new ResultOperation();
            List<SqlParameter> columnParams = this.GetColumsAsParams(entityDef, dataModel.ToList(), variable);
            string whereClause = VariableEngine.GenerateWhereClause(variable);
            string orderClause = VariableEngine.GenerateOrderClause(variable);
            //get sql query parameter from additionalParams
            List<SqlParameter> queryParams = QueryModel.GetSqlParameter(base.GetAllQueryModels(additionalParams) ?? new List<QueryModel>()).ToList();
            //variable.VariableDependencies is null retrieve it from database
            variable.sysBpmsVariableDependencies = variable.sysBpmsVariableDependencies ?? new VariableDependencyService(base.UnitOfWork).GetList(variable.ID, null);

            //add variable dependies to whereclause which have relation with current variable.
            this.AddDependencyClause(variable, queryParams, columnParams, ref whereClause, additionalParams, allSavedEntities);

            string sqlQueryIsExist = $@"(select top(1) {entityDef.FormattedTableName}.ID from {entityDef.FormattedTableName} {whereClause} {orderClause})";

            string sqlInsertQuery = $@"INSERT INTO {entityDef.FormattedTableName}
                                          ({(base.EngineSharedModel.CurrentThreadID.HasValue ? "ThreadID," : "")}{string.Join(",", columnParams.Select(c => c.ParameterName.TrimStart('@')))})
                                    OUTPUT inserted.ID
                                    VALUES
                                          ({(base.EngineSharedModel.CurrentThreadID.HasValue ? ("'" + base.EngineSharedModel.CurrentThreadID.Value + "',") : "")}{string.Join(",", columnParams.Select(c => c.ParameterName))})";

            this.AddDefaultParams((sqlInsertQuery + sqlQueryIsExist), queryParams);
            this.AddVariableParameters(ref sqlInsertQuery, queryParams);
            this.AddVariableParameters(ref sqlQueryIsExist, queryParams);

            queryParams = columnParams.Union(queryParams).Where(c => (sqlInsertQuery + sqlQueryIsExist).Contains(c.ParameterName)).GroupBy(c => c.ParameterName).Select(c => c.FirstOrDefault()).ToList();

            DataTable dataTableIsExist = new DataBaseQueryService(base.UnitOfWork).GetBySqlQuery(sqlQueryIsExist, true, null, queryParams.ToArray());
            Guid? entityIsExistId = dataTableIsExist != null && dataTableIsExist.Rows.Count > 0 ? dataTableIsExist.Rows[0][0].ToGuidObj() : (Guid?)null;

            if (entityIsExistId.HasValue)
            {
                string sqlUpdateQuery = $@"update {entityDef.FormattedTableName} set {string.Join(",", columnParams.Select(c => c.ParameterName.TrimStart('@') + "=" + c.ParameterName))} where ID='{entityIsExistId.ToStringObj()}'";
                //update entity
                new DataBaseQueryService(base.UnitOfWork).ExecuteBySqlQuery(sqlUpdateQuery, true, queryParams.ToArray());
            }
            else
            {
                //insert entity
                this.CheckMandatory(resultOperation, entityDef, columnParams);
                if (resultOperation.IsSuccess)
                    entityIsExistId = new DataBaseQueryService(base.UnitOfWork).ExecuteScalar<Guid>(sqlInsertQuery, true, queryParams.ToArray()).ToGuidObjNull();
            }
            return new Tuple<ResultOperation, Guid?>(resultOperation, entityIsExistId);
        }

        /// <summary>
        /// this method save entityDef using dynamic code.
        /// </summary>
        public Tuple<ResultOperation, Guid?> SaveIntoDataBase(VariableModel variableModel)
        {
            ResultOperation resultOperation = new ResultOperation();
            sysBpmsEntityDef entityDef = new EntityDefService(base.UnitOfWork).GetInfo(variableModel.Name);
            List<SqlParameter> columnParams = this.GetColumsAsParams(entityDef, variableModel.Items.FirstOrDefault().ToList(), (base.EngineSharedModel != null ? new VariableService(base.UnitOfWork).GetInfo(base.EngineSharedModel.CurrentProcessID, base.EngineSharedModel.CurrentApplicationPageID, variableModel.Name) : null));
            //get sql query parameter from additionalParams
            List<SqlParameter> queryParams = QueryModel.GetSqlParameter(base.GetAllQueryModels(null) ?? new List<QueryModel>()).ToList();

            string sqlQueryIsExist = $@"(select top(1) {entityDef.FormattedTableName}.ID from {entityDef.FormattedTableName} where ID='{variableModel["ID"]}' )";

            string sqlInsertQuery = $@"INSERT INTO {entityDef.FormattedTableName}
                                          ({string.Join(",", columnParams.Select(c => c.ParameterName.TrimStart('@')))})
                                    OUTPUT inserted.ID
                                    VALUES
                                          ({string.Join(",", columnParams.Select(c => c.ParameterName))})";
            this.AddDefaultParams((sqlInsertQuery + sqlQueryIsExist), queryParams);
            this.AddVariableParameters(ref sqlInsertQuery, queryParams);
            this.AddVariableParameters(ref sqlQueryIsExist, queryParams);
            queryParams = columnParams.Union(queryParams).Where(c => (sqlInsertQuery + sqlQueryIsExist).Contains(c.ParameterName)).GroupBy(c => c.ParameterName).Select(c => c.FirstOrDefault()).ToList();

            DataTable dataTableIsExist = new DataBaseQueryService(base.UnitOfWork).GetBySqlQuery(sqlQueryIsExist, true, null, queryParams.ToArray());
            Guid? entityIsExistId = dataTableIsExist != null && dataTableIsExist.Rows.Count > 0 ? dataTableIsExist.Rows[0][0].ToGuidObj() : (Guid?)null;

            if (entityIsExistId.HasValue)
            {
                string sqlUpdateQuery = $@"update {entityDef.FormattedTableName} set {string.Join(",", columnParams.Select(c => c.ParameterName.TrimStart('@') + "=" + c.ParameterName))} where ID='{entityIsExistId.ToStringObj()}' ";
                //update entity
                new DataBaseQueryService(base.UnitOfWork).ExecuteBySqlQuery(sqlUpdateQuery, true, queryParams.ToArray());
            }
            else
            {
                //insert entity
                this.CheckMandatory(resultOperation, entityDef, columnParams);
                if (resultOperation.IsSuccess)
                    entityIsExistId = new DataBaseQueryService(base.UnitOfWork).ExecuteScalar<Guid>(sqlInsertQuery, true, queryParams.ToArray()).ToGuidObjNull();
            }

            return new Tuple<ResultOperation, Guid?>(resultOperation, entityIsExistId);
        }
         
        /// <param name="containerQuery">It is generally used in combosearch which add a parent query that filter table's rows according to query parameter and text field</param>
        public DataTable GetEntity(sysBpmsEntityDef entityDef, sysBpmsVariable variable, List<QueryModel> additionalParams, PagingProperties currentPaging, string containerQuery = null)
        {
            string columnName = string.Empty;
            switch ((sysBpmsVariable.e_VarTypeLU)variable.VarTypeLU)
            {
                case sysBpmsVariable.e_VarTypeLU.Integer:
                case sysBpmsVariable.e_VarTypeLU.String:
                case sysBpmsVariable.e_VarTypeLU.Uniqueidentifier:
                case sysBpmsVariable.e_VarTypeLU.Decimal:
                case sysBpmsVariable.e_VarTypeLU.DateTime:
                case sysBpmsVariable.e_VarTypeLU.Boolean:
                    columnName = variable.FieldName;
                    break;
            }
            columnName = string.IsNullOrWhiteSpace(columnName) ? "*" : columnName;
            string whereClause = VariableEngine.GenerateWhereClause(variable);
            string orderClause = VariableEngine.GenerateOrderClause(variable);

            //get sql query parameter from additionalParams
            List<SqlParameter> queryParams = QueryModel.GetSqlParameter(base.GetAllQueryModels(additionalParams) ?? new List<QueryModel>()).ToList();

            //add variable dependies to whereclause which have relation with current variable.
            this.AddDependencyClause(variable, queryParams, null, ref whereClause, additionalParams);

            string sqlQuery = $" select {entityDef.FormattedTableName}.{columnName} from {entityDef.FormattedTableName} {whereClause} {orderClause} ";

            //If containerQuery  is not null 
            if (!string.IsNullOrWhiteSpace(containerQuery))
            {
                sqlQuery = (sqlQuery.Contains("order") && (!sqlQuery.ToLower().Contains("top") && !sqlQuery.ToLower().Contains("offset"))) ? $"{sqlQuery} OFFSET 0 ROWS " : sqlQuery;
                sqlQuery = string.Format(containerQuery, sqlQuery);
            }

            this.AddDefaultParams(sqlQuery, queryParams);
            this.AddVariableParameters(ref sqlQuery, queryParams);
            queryParams = queryParams.Where(c => sqlQuery.Contains(c.ParameterName)).ToList();

            return new DataBaseQueryService(this.UnitOfWork).GetBySqlQuery(sqlQuery, true, currentPaging, queryParams.ToArray());
        }

        public Guid? GetEntityID(sysBpmsEntityDef entityDef, sysBpmsVariable variable, List<QueryModel> additionalParams)
        {

            string whereClause = VariableEngine.GenerateWhereClause(variable);
            string orderClause = VariableEngine.GenerateOrderClause(variable);

            //get sql query parameter from additionalParams
            List<SqlParameter> queryParams = QueryModel.GetSqlParameter(base.GetAllQueryModels(additionalParams) ?? new List<QueryModel>()).ToList();


            //add variable dependies to whereclause which have relation with current variable.
            this.AddDependencyClause(variable, queryParams, null, ref whereClause, additionalParams);

            string sqlQuery = $" select {entityDef.FormattedTableName}.ID from {entityDef.FormattedTableName} {whereClause} {orderClause} ";

            this.AddDefaultParams(sqlQuery, queryParams);
            this.AddVariableParameters(ref sqlQuery, queryParams);
            queryParams = queryParams.Where(c => sqlQuery.Contains(c.ParameterName)).ToList();

            DataTable dataTable = new DataBaseQueryService(this.UnitOfWork).GetBySqlQuery(sqlQuery, true, null, queryParams.ToArray());
            if (dataTable.Rows.Count > 0)
                return Guid.Parse(dataTable.Rows[0][0].ToString());
            return null;
        }

        /// <summary>
        /// this method add variable dependies to whereclause which have relation with current variable.
        /// </summary>
        /// <param name="queryParams">it would update this parameter.</param>
        private void AddDependencyClause(sysBpmsVariable variable, List<SqlParameter> queryParams, List<SqlParameter> insertColumnParams, ref string whereClause, List<QueryModel> additionalParams, Dictionary<string, DataModel> allSavedEntities = null)
        {
            //add variable dependies where clause which have relation with current variable
            if (variable.sysBpmsVariableDependencies.Any())
            {
                foreach (sysBpmsVariableDependency item in variable.sysBpmsVariableDependencies)
                {
                    object value = null;
                    var toVariable = new VariableService(base.UnitOfWork).GetInfo(item.ToVariableID.Value);
                    //set BindTrace to find a field of a entity variable if the variable is entity.
                    string BindTrace = $"{toVariable.Name}{(!string.IsNullOrWhiteSpace(item.ToPropertyName) ? ("." + item.ToPropertyName) : "")}";
                    whereClause += $"{(string.IsNullOrWhiteSpace(whereClause) ? "" : " and ")}{item.DependentPropertyName}=@{BindTrace.Replace(".", "_")}";

                    if (allSavedEntities?.ContainsKey(toVariable.Name) == true && allSavedEntities[toVariable.Name][BindTrace.Split('.').LastOrDefault()] != null)
                        value = allSavedEntities[toVariable.Name][BindTrace.Split('.').LastOrDefault()];
                    else
                        value = new DataManageEngine(base.EngineSharedModel, base.UnitOfWork).GetValueByBinding(BindTrace, base.GetAllQueryModels(additionalParams));

                    queryParams.Add(new SqlParameter($"@{BindTrace.Replace(".", "_")}", value ?? DBNull.Value));

                    if (insertColumnParams != null)
                    {
                        if (!insertColumnParams.Any(c => c.ParameterName.TrimStart('@') == item.DependentPropertyName))
                        {
                            insertColumnParams.Add(new SqlParameter("@" + item.DependentPropertyName, value ?? DBNull.Value));
                        }
                    }
                }
            }
            whereClause = string.IsNullOrWhiteSpace(whereClause) ? "" : (whereClause.Trim().StartsWith("where") ? whereClause : (" where " + whereClause));
        }

        private void AddDefaultParams(string sqlQuery, List<SqlParameter> queryParams)
        {
            if (sqlQuery.Contains("@ThreadID") && !queryParams.Any(c => c.ParameterName == "@ThreadID"))
                queryParams.Add(new SqlParameter("@ThreadID", base.EngineSharedModel?.CurrentThreadID));
            if (sqlQuery.Contains("@ProcessID") && !queryParams.Any(c => c.ParameterName == "@ProcessID"))
                queryParams.Add(new SqlParameter("@ProcessID", base.EngineSharedModel?.CurrentProcessID));
            if (sqlQuery.Contains("@UserID") && !queryParams.Any(c => c.ParameterName == "@UserID"))
                queryParams.Add(new SqlParameter("@UserID", new UserService(base.UnitOfWork).GetInfo(base.EngineSharedModel?.CurrentUserName)?.ID));
            if (sqlQuery.Contains("@UserName") && !queryParams.Any(c => c.ParameterName == "@UserName"))
                queryParams.Add(new SqlParameter("@UserName", base.EngineSharedModel?.CurrentUserName));
        }

        private List<SqlParameter> GetColumsAsParams(sysBpmsEntityDef sysBpmsEntityDef, Dictionary<string, object> dataList, sysBpmsVariable sysBpmsVariable)
        {
            List<EntityPropertyModel> Properties = sysBpmsEntityDef.Properties;
            List<SqlParameter> columnParams = new List<SqlParameter>();

            for (int index = 0; index < dataList.Count; index++)
            {
                var item = dataList.ToList()[index];
                string propertyName = (sysBpmsVariable == null || sysBpmsVariable.VarTypeLU == (int)sysBpmsVariable.e_VarTypeLU.Object) ? item.Key : sysBpmsVariable.FieldName;
                EntityPropertyModel _Property = Properties.FirstOrDefault(c => c.Name == propertyName);
                if (Properties.Any(c => c.Name == propertyName))
                {
                    switch (_Property.DbType)
                    {
                        case EntityPropertyModel.e_dbType.Decimal:
                            if (_Property.Required)
                                dataList[item.Key] = BPMSUtility.toDecimal(item.Value);
                            else
                                dataList[item.Key] = item.Value != null ? (decimal?)BPMSUtility.toDecimal(item.Value) : (decimal?)null;
                            break;
                        case EntityPropertyModel.e_dbType.Integer:
                            if (_Property.Required)
                                dataList[item.Key] = BPMSUtility.toInt(item.Value);
                            else
                                dataList[item.Key] = item.Value != null ? BPMSUtility.toInt(item.Value) : (int?)null;
                            break;
                        case EntityPropertyModel.e_dbType.Long:
                            if (_Property.Required)
                                dataList[item.Key] = Convert.ToInt64(item.Value);
                            else
                                dataList[item.Key] = item.Value != null ? Convert.ToInt64(item.Value) : (long?)null;
                            break;
                        case EntityPropertyModel.e_dbType.String:
                            dataList[item.Key] = BPMSUtility.toString(item.Value);
                            break;
                        case EntityPropertyModel.e_dbType.DateTime:
                            if (string.IsNullOrWhiteSpace(item.Value.ToStringObj()))
                                dataList[item.Key] = DBNull.Value;
                            else
                                dataList[item.Key] = Convert.ToDateTime(dataList[item.Key]);
                            break;
                        case EntityPropertyModel.e_dbType.Uniqueidentifier:
                            if (string.IsNullOrWhiteSpace(item.Value.ToStringObj()))
                                dataList[item.Key] = DBNull.Value;
                            break;
                    }
                    columnParams.Add(new SqlParameter("@" + propertyName, dataList[item.Key]));
                }
            }
            return columnParams;
        }

        private void CheckMandatory(ResultOperation resultOperation, sysBpmsEntityDef entityDef, List<SqlParameter> columnParams)
        {
            if (entityDef.Properties.Any(c => c.Required && !columnParams.Any(d => d.ParameterName.TrimStart('@') == c.Name)))
            {
                string errorMsg = string.Join("<br/>", entityDef.Properties.Where(c => c.Required && !columnParams.Any(d => d.ParameterName.TrimStart('@') == c.Name)).
                    Select(c => string.Format(SharedLang.Get("FormatRequired.Text"), c.Name)));
                resultOperation.AddError(errorMsg);
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

    }
}
