using DynamicBusiness.BPMS.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;

namespace DynamicBusiness.BPMS.Controllers
{
    public class BpmsVariableManagerController : BpmsAdminApiControlBase
    {
        // GET: BpmsVariableManager
        public object GetList(Guid? ProcessId = null, Guid? ApplicationPageId = null)
        {
            using (VariableService variableService = new VariableService())
                return new { GetList = variableService.GetList(ProcessId, ApplicationPageId, null, "", null, true).Select(c => new VariableDTO(c)).ToList() };
        }

        [HttpGet]
        public object GetAddEdit(Guid? ID = null, string VariableName = "", Guid? ProcessId = null, Guid? ApplicationPageId = null)
        {
            using (VariableService variableService = new VariableService())
            {
                VariableDTO variable = null;
                if (ID.ToGuidObj() != Guid.Empty)
                    variable = new VariableDTO(variableService.GetInfo(ID.Value));
                else
                    if (!string.IsNullOrWhiteSpace(VariableName))
                    variable = new VariableDTO(variableService.GetInfo(ProcessId, ApplicationPageId, VariableName));
                if (variable == null)
                    variable = new VariableDTO(new sysBpmsVariable() { ProcessID = ProcessId, ApplicationPageID = ApplicationPageId });

                using (EntityDefService entityDefService = new EntityDefService())
                {
                    List<EntityPropertyModel> Properties = new List<EntityPropertyModel>();
                    var Entities = entityDefService.GetList(string.Empty, true);
                    if (variable != null && variable.EntityDefID.HasValue)
                        Properties = entityDefService.GetInfo(variable.EntityDefID.Value).AllProperties;
                    else
                        Properties = new List<EntityPropertyModel>();

                    variable.ListVariableDependencyDTO?.ForEach((item) =>
                    {
                        if (item.ToVariableID.HasValue)
                        {
                            sysBpmsVariable getVar = variableService.GetInfo(item.ToVariableID.Value);
                            if (getVar.EntityDefID.HasValue)
                            {
                                item.GetToVariableProperties = entityDefService.GetInfo(getVar.EntityDefID.Value).AllProperties;
                            }
                        }
                        else item.GetToVariableProperties = new List<EntityPropertyModel>();
                    });

                    using (DBConnectionService dbConnectionService = new DBConnectionService())
                        return new
                        {
                            Model = variable,
                            ListConnection = dbConnectionService.GetList("").Select(c => new DBConnectionDTO(c)).ToList(),
                            ListTypes = EnumObjHelper.GetEnumList<sysBpmsVariable.e_VarTypeLU>().Select(c => new QueryModel(c.Key.ToString(), c.Value)),
                            ListRelations = EnumObjHelper.GetEnumList<sysBpmsVariable.e_RelationTypeLU>().Where(c => variable.ProcessID.HasValue || c.Key != (int)sysBpmsVariable.e_RelationTypeLU.Local).Select(c => new QueryModel(c.Key.ToString(), c.Value)),
                            ListEntities = Entities.Select(c => new { c.ID, Name = c.Name + $"({c.DisplayName})" }).ToList(),
                            ListFilters = EnumObjHelper.GetEnumList<sysBpmsVariable.e_FilterTypeLU>().Select(c => new QueryModel(c.Key.ToString(), c.Value)),
                            ListProperties = Properties,
                            DependencyToVariables = variableService.GetList(base.ProcessId, base.ApplicationPageId, null, "", null, true).Where(c => c.ID != variable.ID).Select(c => new VariableDTO(c)).ToList()
                        };
                }
            }
        }

        /// <summary>
        /// update variable.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object PostAddEdit(VariableDTO variableDTO)
        {
            int RelationTypeLU = base.MyRequest.Form["RelationTypeLU"].ToIntObj();
            variableDTO.ListDependencies = variableDTO.ListDependencies ?? new List<VariableDependencyDTO>();
            if (variableDTO.ProcessID.HasValue || variableDTO.ApplicationPageID.HasValue)
            {
                if (RelationTypeLU == (int)sysBpmsVariable.e_RelationTypeLU.Local && variableDTO.VarTypeLU == (int)sysBpmsVariable.e_VarTypeLU.List)
                    variableDTO.Collection = variableDTO.ListItems.BuildXml();
                //set ViewBags
                using (VariableService variableService = new VariableService())
                {
                    using (ProcessService processService = new ProcessService())
                    {
                        sysBpmsVariable variable = variableDTO.ID != Guid.Empty ? variableService.GetInfo(variableDTO.ID) : new sysBpmsVariable();
                        if (!base.ProcessId.HasValue || processService.GetInfo(base.ProcessId.Value).AllowEdit())
                        {
                            ResultOperation resultOperation = variable.Update(variableDTO.ProcessID, variableDTO.ApplicationPageID, variableDTO.Name, variableDTO.VarTypeLU, variableDTO.EntityDefID, variableDTO.FieldName, variableDTO.Query, variableDTO.FilterTypeLU, variableDTO.Collection, variableDTO.DBConnectionID, variableDTO.DefaultValue, variableDTO.WhereClause, variableDTO.OrderByClause);
                            if (!resultOperation.IsSuccess)
                                return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);

                            List<sysBpmsVariableDependency> VariableDependencies = new List<sysBpmsVariableDependency>();
                            if (variableDTO.ID != Guid.Empty)
                            {
                                foreach (var item in variableDTO.ListDependencies)
                                {
                                    sysBpmsVariableDependency variableDependency = new sysBpmsVariableDependency();
                                    resultOperation = variableDependency.Update(item.ID, variableDTO.ID, item.DependentPropertyName, item.ToVariableID, item.ToPropertyName, item.Description);
                                    if (!resultOperation.IsSuccess)
                                        return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                                    VariableDependencies.Add(variableDependency);
                                }
                                resultOperation = variableService.Update(variable, VariableDependencies);
                            }
                            else
                            {
                                foreach (var item in variableDTO.ListDependencies)
                                {
                                    sysBpmsVariableDependency variableDependency = new sysBpmsVariableDependency();
                                    resultOperation = variableDependency.Update(item.ID, item.DependentVariableID, item.DependentPropertyName, item.ToVariableID, item.ToPropertyName, item.Description);
                                    if (!resultOperation.IsSuccess)
                                        return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                                    VariableDependencies.Add(variableDependency);
                                }
                                resultOperation = variableService.Add(variable, VariableDependencies);
                            }
                            if (resultOperation.IsSuccess)
                            {
                                return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success, new
                                {
                                    name = variable.Name,
                                    id = variable.ID,
                                    comboTree = variableService.GetVariableAsComboTree(base.ProcessId, base.ApplicationPageId, null).AsJson(),
                                    entityVariables = variableService.GetList(base.ProcessId, base.ApplicationPageId, (int)sysBpmsVariable.e_VarTypeLU.Object, "", null, true).Select(c => new { text = c.Name, value = c.ID }).ToList(),
                                    listVariables = variableService.GetList(base.ProcessId, base.ApplicationPageId, (int)sysBpmsVariable.e_VarTypeLU.List, "", null, true).Select(c => new { text = c.Name, value = c.Name }).ToList()
                                });

                            }
                            else
                                return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                        }
                        else
                            return new PostMethodMessage(LangUtility.Get("NotAllowEdit.Text", nameof(sysBpmsProcess)), DisplayMessageType.error);
                    }
                }
            }
            return new PostMethodMessage(SharedLang.Get("NotFound.Text"), DisplayMessageType.error);
        }

        [HttpDelete]
        public object Delete(Guid ID)
        {
            using (VariableService variableService = new VariableService())
            {
                ResultOperation resultOperation = variableService.Delete(ID);
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }

        [HttpGet]
        public object GetEntityProperty(Guid? EntityID = null, Guid? VariableID = null)
        {
            using (EntityDefService entityDefService = new EntityDefService())
            {
                if (EntityID.HasValue)
                    return entityDefService.GetInfo(EntityID.Value).AllProperties;
                else
                    using (VariableService variableService = new VariableService())
                        return entityDefService.GetInfo(variableService.GetInfo(VariableID.Value)?.EntityDefID ?? Guid.Empty)?.AllProperties ?? new List<EntityPropertyModel>();
            }
        }

        [HttpGet]
        public object GetSelectVariable(string selectedVariable = "", bool isListVariable = false, string searchVariable = "")
        {
            selectedVariable = selectedVariable.ToStringObj().Split('.').FirstOrDefault();
            searchVariable = searchVariable.ToStringObj().Split('.').FirstOrDefault();
            using (VariableService variableService = new VariableService())
            {
                var list = variableService.GetList(base.ProcessId, base.ApplicationPageId, null, searchVariable, null, null).Where(c =>
                  (isListVariable && c.VarTypeLU == (int)sysBpmsVariable.e_VarTypeLU.List) ||
                  (!isListVariable && c.VarTypeLU != (int)sysBpmsVariable.e_VarTypeLU.List)).Select(c => new SelectVariableDTO(c)).ToList();
                if (!string.IsNullOrWhiteSpace(selectedVariable))
                {
                    var sV = list.FirstOrDefault(c => c.Name == selectedVariable);
                    list.Remove(sV);
                    list.Insert(0, sV);
                }
                return list;
            }
        }

        /// <summary>
        /// it is called from _FormGenerator for filling all variable combotrees
        /// </summary>
        [HttpGet]
        public object GetVariableData(Guid? ProcessId = null, Guid? ApplicationPageId = null)
        {
            return new VariableService().GetVariableAsComboTree(ProcessId, ApplicationPageId, null);
        }
    }
}