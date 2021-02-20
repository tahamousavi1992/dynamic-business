using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.BusinessLogic;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Specialized;
using DynamicBusiness.BPMS.Domain;
using System.Web;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DataManageEngine : BaseEngine, IDataManageEngine
    {
        #region .:: Public Properties ::.
        public EngineSharedModel GetSharedModel() => base.EngineSharedModel;
        /// <summary>
        /// It is used for Forms inside forms to change appPageID of parent form for retrieving variables.
        /// </summary>
        public void SetApplicationPageID(Guid appPageID) => base.EngineSharedModel.CurrentApplicationPageID = appPageID;
        public Guid? ThreadID { get; set; }
        public Dictionary<string, object> FormControlValues { get; set; }
        public string ThreadTaskDescription { get; set; }

        #endregion

        #region .:: Private Properties ::.

        /// <summary>
        /// key equal variable name
        /// </summary>
        private List<VariableModel> GetDataList { get; set; }
        /// <summary>
        /// key equal variable name
        /// </summary>
        private Dictionary<string, DataModel> SetDataList { get; set; }

        private List<QueryModel> AdditionalParams { get; set; }
        #endregion

        #region .:: constructor ::.
        public DataManageEngine(EngineSharedModel engineSharedModel, IUnitOfWork unitOfWork = null) : base(engineSharedModel, unitOfWork)
        {
            this.AdditionalParams = new List<QueryModel>();
            this.ThreadID = base.EngineSharedModel?.CurrentThreadID;
            this.GetDataList = new List<VariableModel>();
            this.SetDataList = new Dictionary<string, DataModel>();
        }

        #endregion

        #region .:: public method ::.

        public VariableModel GetEntityByBinding(string BindTrace, List<QueryModel> listFormQueryModel = null, PagingProperties currentPaging = null, string containerQuery = null, string[] includes = null)
        {
            if (!string.IsNullOrWhiteSpace(BindTrace))
                return this.GetEntitiesByName(BindTrace.Split('.')[0], listFormQueryModel, currentPaging, containerQuery, includes);
            else return null;
        }

        public object GetValueByBinding(string BindTrace, List<QueryModel> listFormQueryModel = null)
        {
            VariableModel variableModel = this.GetEntityByBinding(BindTrace, listFormQueryModel);
            if (variableModel != null)
                return variableModel[BindTrace.Split('.').LastOrDefault()];
            else
                return null;
        }

        public T GetValueByBinding<T>(string BindTrace, List<QueryModel> listFormQueryModel = null)
        {
            VariableModel variableModel = this.GetEntityByBinding(BindTrace, listFormQueryModel);
            if (variableModel != null)
                return variableModel.GetValue<T>(BindTrace.Split('.').LastOrDefault());
            else
                return default(T);
        }

        public ResultOperation SaveIntoDataBase(ContentHtml contentHtml, sysBpmsThreadTask threadTask, List<VariableModel> setExternalVariable)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                base.BeginTransaction();

                //First set variable according to elements.
                resultOperation = this.SetValuesToElements(contentHtml);

                //Then set external variable 
                if (setExternalVariable != null)
                {
                    foreach (var item in setExternalVariable)
                        foreach (var dataModel in item.Items)
                            foreach (var val in dataModel.ToList())
                                this.SetValueByBinding(val.Key, val.Value);
                }

                if (resultOperation.IsSuccess)
                    resultOperation = this.SaveInto();

                if (resultOperation.IsSuccess)
                {
                    DocumentEngine documentEngine = new DocumentEngine(base.EngineSharedModel, base.UnitOfWork);
                    foreach (FileUploadHtml item in contentHtml.Rows.SelectMany(r => (r is RowHtml ? ((RowHtml)r).Columns : ((AccordionHtml)r).GetListColumn()).SelectMany(d => d.children.Where(f => f is FileUploadHtml).ToList())))
                    {
                        if (resultOperation.IsSuccess)
                        {
                            sysBpmsVariable variable = item.VariableId.ToGuidObjNull() == null ? null :
                                new VariableService(base.UnitOfWork).GetInfo(item.VariableId.ToGuidObj());

                            Guid? entityId = (variable != null && this.SetDataList.ContainsKey(variable.Name)) ? this.SetDataList[variable?.Name].GetValue<Guid>("ID") : (Guid?)null;
                            Guid? entityDefId = variable?.EntityDefID;
                            resultOperation = documentEngine.IsValid(item, variable, entityId, entityDefId, base.EngineSharedModel.CurrentUserName);
                            if (resultOperation.IsSuccess)
                                resultOperation = documentEngine.SaveFile(item, variable, entityId, entityDefId, "", base.EngineSharedModel.CurrentUserName);

                        }
                    }
                }
                if (resultOperation.IsSuccess)
                {
                    if (!string.IsNullOrWhiteSpace(this.ThreadTaskDescription))
                    {
                        threadTask.Update(this.ThreadTaskDescription);
                        new ThreadTaskService(base.UnitOfWork).Update(threadTask);
                    }
                }
            }
            catch (Exception ex)
            {
                return base.ExceptionHandler(ex);
            }
            base.FinalizeService(resultOperation);
            return resultOperation;
        }

        public void SaveIntoDataBase(VariableModel variableModel, List<QueryModel> additionalParams)
        {
            if (additionalParams != null)
                this.AdditionalParams = additionalParams;
            foreach (var item in variableModel.Items)
            {
                foreach (var dataModel in item.ToList())
                    this.SetValueByBinding(dataModel.Key, dataModel.Value);
            }
            ResultOperation resultOperation = this.SaveInto();
            if (!resultOperation.IsSuccess)
            {
                throw new Exception(resultOperation.GetErrors());
            }
        }

        public void ClearVariable(string varName)
        {
            if (this.GetDataList.Any(c => c.Name == varName))
            {
                this.GetDataList.Remove(this.GetDataList.FirstOrDefault(c => c.Name == varName));
            }
        }

        #endregion

        #region .:: Private Method ::.

        private ResultOperation SaveInto()
        {
            VariableService variableService = new VariableService(base.UnitOfWork);
            ResultOperation resultOperation = new ResultOperation();
            //Resort this.SetDataList because of variable dependencies.
            var listSetDataList = this.SetDataList.ToList();
            for (int i = 0; i < listSetDataList.Count; i++)
            {
                var item = listSetDataList[i];
                sysBpmsVariable variable = variableService.GetInfo(base.EngineSharedModel?.CurrentProcessID, base.EngineSharedModel?.CurrentApplicationPageID, item.Key, new string[] { $"{nameof(sysBpmsVariable.DependentVariableDependencies)}.{nameof(sysBpmsVariableDependency.ToVariable)}" });
                foreach (sysBpmsVariableDependency vDependency in variable.DependentVariableDependencies)
                {
                    var parentVariable = listSetDataList.Select((c, index) => new { item = c, index }).FirstOrDefault(c => c.item.Key != item.Key && c.item.Key == vDependency.ToVariable.Name && i < c.index);
                    //if variable has a dependency which has higher index ,this code block replace both of them.
                    if (parentVariable != null)
                    {
                        listSetDataList[i] = parentVariable.item;
                        listSetDataList[parentVariable.index] = item;
                        i--;
                    }
                }
            }
            this.SetDataList = listSetDataList.ToDictionary(c => c.Key, c => c.Value);

            //save variables.
            foreach (var item in this.SetDataList)
            {
                if (resultOperation.IsSuccess)
                {
                    sysBpmsVariable variable = variableService.GetInfo(base.EngineSharedModel?.CurrentProcessID, base.EngineSharedModel?.CurrentApplicationPageID, item.Key, new string[] { nameof(sysBpmsVariable.DependentVariableDependencies) });
                    if (variable != null)
                    {
                        switch ((sysBpmsVariable.e_RelationTypeLU)variable.RelationTypeLU)
                        {
                            case sysBpmsVariable.e_RelationTypeLU.Local:
                                resultOperation = new SystemicVariableTypeEngine(base.EngineSharedModel, variable, base.EngineSharedModel?.CurrentProcessID, this.ThreadID, this.AdditionalParams, this.UnitOfWork).SaveValues(item.Value);
                                break;
                            case sysBpmsVariable.e_RelationTypeLU.Entity:
                                resultOperation = new EntityVariableTypeEngine(base.EngineSharedModel, variable, base.EngineSharedModel?.CurrentProcessID, this.ThreadID, this.AdditionalParams, this.UnitOfWork).SaveValues(item.Value, this.SetDataList);
                                item.Value.SetValue("ID", resultOperation.CurrentObject);
                                break;
                            case sysBpmsVariable.e_RelationTypeLU.SqlQuery:
                                resultOperation = new SqlQueryVariableTypeEngine(base.EngineSharedModel, variable, base.EngineSharedModel?.CurrentProcessID, this.ThreadID, this.AdditionalParams, this.UnitOfWork).SaveValues(item.Value);
                                break;
                        }
                    }
                }
            }
            return resultOperation;
        }

        /// <summary>
        /// this set form post back element to RowHtmls elements like textbox and dropdown
        /// </summary>
        private ResultOperation SetValuesToElements(ContentHtml contentHtml)
        {
            ResultOperation resultOperation = new ResultOperation();
            foreach (RowHtml row in contentHtml.GetRowHtmls)
            {
                foreach (ColumnHtml column in row.Columns)
                {
                    foreach (var item in column.children)
                    {
                        if (item is FormHtml)
                        {
                            this.SetValuesToElements(((FormHtml)item).ContentHtml);
                        }
                        else
                        {
                            if (item is BindingElementBase)
                            {
                                BindingElementBase bindingItem = ((BindingElementBase)item);
                                if (bindingItem.IsInForm(base.GetAllQueryModels(this.AdditionalParams)))
                                {
                                    bindingItem.IsValid(resultOperation);
                                    if (!resultOperation.IsSuccess)
                                        return resultOperation;
                                    if (item is TextBoxHtml && ((TextBoxHtml)item).SubType == TextBoxHtml.e_TextBoxType.threadTaskDescription.ToString())
                                    {
                                        this.ThreadTaskDescription = bindingItem.Value.ToStringObjNull();
                                    }
                                    else
                                    {
                                        if (bindingItem.Type != "FILEUPLOAD")
                                            this.SetValueByBinding(bindingItem.Map, bindingItem.Value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return resultOperation;
        }

        /// <summary>
        /// this set values of properties according to variable name , storing each variable data into this.SetDataList.
        /// </summary>
        private void SetValueByBinding(string BindTrace, object value)
        {
            if (!string.IsNullOrWhiteSpace(BindTrace))
            {
                string VariableName = BindTrace.Split('.').FirstOrDefault();
                string PropertyName = BindTrace.Split('.').LastOrDefault();
                if (this.SetDataList.ContainsKey(VariableName))
                {
                    this.SetDataList[VariableName][PropertyName] = value;
                }
                else
                {
                    DataModel _DataModel = new DataModel();
                    _DataModel[PropertyName] = value;
                    this.SetDataList[VariableName] = _DataModel;
                }
            }
        }

        private VariableModel GetEntitiesByName(string VarName, List<QueryModel> additionalParams = null, PagingProperties currentPaging = null, string containerQuery = null, string[] includes = null)
        {
            if (additionalParams != null)
                this.AdditionalParams = additionalParams;

            if (!this.GetDataList.Any(c => c.Name == VarName))
            {
                sysBpmsVariable _Variable = new VariableService(base.UnitOfWork).GetInfo(base.EngineSharedModel?.CurrentProcessID, base.EngineSharedModel?.CurrentApplicationPageID, VarName, new string[] { nameof(sysBpmsVariable.DependentVariableDependencies) });
                List<DataModel> _DataModel = new List<DataModel>();
                if (_Variable != null)
                {
                    switch ((sysBpmsVariable.e_RelationTypeLU)_Variable.RelationTypeLU)
                    {
                        case sysBpmsVariable.e_RelationTypeLU.SqlQuery:
                            //this set each DataModel's key corresponding to returned columns by query and then set value into it.  
                            _DataModel = new SqlQueryVariableTypeEngine(base.EngineSharedModel, _Variable, base.EngineSharedModel?.CurrentProcessID, this.ThreadID, this.AdditionalParams, base.UnitOfWork).GetResult(currentPaging, containerQuery);
                            break;
                        case sysBpmsVariable.e_RelationTypeLU.Local:
                            //this set each DataModel's key corresponding to variable name and then set value into it.  
                            _DataModel = new SystemicVariableTypeEngine(base.EngineSharedModel, _Variable, base.EngineSharedModel?.CurrentProcessID, this.ThreadID, this.AdditionalParams, base.UnitOfWork).GetResult();
                            break;
                        case sysBpmsVariable.e_RelationTypeLU.Entity:
                            //this set each DataModel's key corresponding to returned columns by query and then set value into it.  
                            //this method return one DataModel if the _Variable FilterTypeLU is Filtered otherwise it will return list of DataModel.
                            _DataModel = new EntityVariableTypeEngine(base.EngineSharedModel, _Variable, base.EngineSharedModel?.CurrentProcessID, this.ThreadID, this.AdditionalParams, base.UnitOfWork).GetResult(currentPaging, containerQuery, includes);
                            break;
                    }
                    this.GetDataList.Add(new VariableModel(VarName, _DataModel));
                }
            }
            return this.GetDataList.FirstOrDefault(c => c.Name == VarName);
        }

        /// <summary>
        /// it is used by Combo search to get name of entity using text/value field 
        /// </summary>
        public VariableModel GetEntityWithKeyValue(string variableName, Dictionary<string, object> dictionary)
        {
            string whereClause = string.Empty;
            sysBpmsVariable variable = new VariableService(base.UnitOfWork).GetInfo(base.EngineSharedModel?.CurrentProcessID, base.EngineSharedModel?.CurrentApplicationPageID, variableName, new string[] { nameof(sysBpmsVariable.DependentVariableDependencies), nameof(sysBpmsVariable.EntityDef) });
            VariableModel variableModel = null;
            if (variable != null)
            {
                switch ((sysBpmsVariable.e_RelationTypeLU)variable.RelationTypeLU)
                {
                    case sysBpmsVariable.e_RelationTypeLU.SqlQuery:
                        variable.WhereClause = string.Join(" and ", dictionary.Select(c => $"{c.Key}='{c.Value}'"));
                        //this set each DataModel's key corresponding to returned columns by query and then set value into it.  
                        variableModel = new VariableModel(variableName, new SqlQueryVariableTypeEngine(base.EngineSharedModel, variable, base.EngineSharedModel?.CurrentProcessID, this.ThreadID, this.AdditionalParams, base.UnitOfWork).GetResult(null));
                        break;
                    case sysBpmsVariable.e_RelationTypeLU.Entity:
                        //this set each DataModel's key corresponding to returned columns by query and then set value into it.  
                        //this method return one DataModel if the _Variable FilterTypeLU is Filtered otherwise it will return list of DataModel.
                        variable.WhereClause = string.Join(" and ", dictionary.Select(c => $"{variable.EntityDef.FormattedTableName}.{c.Key}='{c.Value}'"));
                        variableModel = new VariableModel(variableName, new EntityVariableTypeEngine(base.EngineSharedModel, variable, base.EngineSharedModel?.CurrentProcessID, this.ThreadID, this.AdditionalParams, base.UnitOfWork).GetResult(null));
                        break;
                }
            }
            return variableModel;
        }

        #endregion
    }
}
