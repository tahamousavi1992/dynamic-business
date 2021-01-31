using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using System.Web.Script.Serialization;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class VariableService : ServiceBase
    {
        public VariableService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsVariable variable, List<sysBpmsVariableDependency> listVariables)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                this.BeginTransaction();
                if (resultOperation.IsSuccess)
                {
                    if (variable.RelationTypeLU != (int)sysBpmsVariable.e_RelationTypeLU.Entity)
                    {
                        variable.FilterTypeLU = variable.ApplicationPageID.HasValue ? (int)sysBpmsVariable.e_FilterTypeLU.AllEntities : variable.FilterTypeLU;
                        variable.EntityDefID = null;
                        variable.FieldName = string.Empty;
                    }

                    if (variable.RelationTypeLU != (int)sysBpmsVariable.e_RelationTypeLU.SqlQuery)
                    {
                        variable.Query = string.Empty;
                        variable.DBConnectionID = null;
                    }
                    sysBpmsVariable preVariable = this.GetInfo(variable.ProcessID, variable.ApplicationPageID, variable.Name);
                    if (preVariable != null)
                        resultOperation.AddError(LangUtility.Get("SameName.Error", nameof(sysBpmsVariable)));

                    if (resultOperation.IsSuccess)
                    {
                        this.UnitOfWork.Repository<IVariableRepository>().Add(variable);
                        this.UnitOfWork.Save();

                        if (variable.RelationTypeLU == (int)sysBpmsVariable.e_RelationTypeLU.Entity)
                        {
                            listVariables = listVariables ?? new List<sysBpmsVariableDependency>();
                            foreach (var item in listVariables)
                            {
                                item.DependentVariableID = variable.ID;
                                if (resultOperation.IsSuccess)
                                    resultOperation = new VariableDependencyService(base.UnitOfWork).Add(item);
                            }
                        }
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

        public ResultOperation Update(sysBpmsVariable variable, List<sysBpmsVariableDependency> listVariables)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                this.BeginTransaction();
                if (resultOperation.IsSuccess)
                {
                    if (variable.RelationTypeLU != (int)sysBpmsVariable.e_RelationTypeLU.Entity)
                    {
                        variable.FilterTypeLU = variable.ApplicationPageID.HasValue ? (int)sysBpmsVariable.e_FilterTypeLU.AllEntities : variable.FilterTypeLU;
                        variable.EntityDefID = null;
                        variable.FieldName = string.Empty;
                    }

                    if (variable.RelationTypeLU != (int)sysBpmsVariable.e_RelationTypeLU.SqlQuery)
                    {
                        variable.Query = string.Empty;
                        variable.DBConnectionID = null;
                    }
                    sysBpmsVariable preVariable = this.GetInfo(variable.ProcessID, variable.ApplicationPageID, variable.Name);
                    if (preVariable != null && preVariable.ID != variable.ID)
                        resultOperation.AddError(LangUtility.Get("SameName.Error", nameof(sysBpmsVariable)));

                    if (resultOperation.IsSuccess)
                    {
                        this.UnitOfWork.Repository<IVariableRepository>().Update(variable);
                        this.UnitOfWork.Save();

                        //if variable is entity type.
                        if (variable.RelationTypeLU == (int)sysBpmsVariable.e_RelationTypeLU.Entity)
                        {
                            listVariables = listVariables ?? new List<sysBpmsVariableDependency>();
                            //VariableDependency
                            List<sysBpmsVariableDependency> Dependencies = new VariableDependencyService(base.UnitOfWork).GetList(variable.ID, null);

                            //delete VariableDependency if is not exist in listVariables
                            Dependencies.ForEach(c =>
                            {
                                if (!listVariables.Any(d => d.ID == c.ID))
                                    new VariableDependencyService(base.UnitOfWork).Delete(c.ID);
                            });

                            //add or update VariableDependency
                            foreach (var item in listVariables)
                            {
                                item.DependentVariableID = variable.ID;
                                if (resultOperation.IsSuccess)
                                {
                                    if (item.ID == Guid.Empty)
                                        resultOperation = new VariableDependencyService(base.UnitOfWork).Add(item);
                                    else
                                        resultOperation = new VariableDependencyService(base.UnitOfWork).Update(item);
                                }
                            }
                        }
                        else
                            new VariableDependencyService(base.UnitOfWork).GetList(variable.ID, null).ForEach(c =>
                            {
                                if (resultOperation.IsSuccess)
                                    resultOperation = new VariableDependencyService(base.UnitOfWork).Delete(c.ID);
                            });
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

        public ResultOperation Delete(Guid variableId)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                this.BeginTransaction();
                if (resultOperation.IsSuccess)
                {
                    List<sysBpmsVariableDependency> list = new VariableDependencyService(base.UnitOfWork).GetList(variableId, null);
                    foreach (sysBpmsVariableDependency item in list)
                    {
                        if (resultOperation.IsSuccess)
                            resultOperation = new VariableDependencyService(base.UnitOfWork).Delete(item.ID);
                    }

                    list = new VariableDependencyService(base.UnitOfWork).GetList(null, variableId);
                    foreach (sysBpmsVariableDependency item in list)
                    {
                        if (resultOperation.IsSuccess)
                            resultOperation = new VariableDependencyService(base.UnitOfWork).Delete(item.ID);
                    }
                    if (resultOperation.IsSuccess)
                    {
                        this.UnitOfWork.Repository<IVariableRepository>().Delete(variableId);
                        this.UnitOfWork.Save();
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

        public sysBpmsVariable GetInfo(Guid ID, string[] includes = null)
        {
            return this.UnitOfWork.Repository<IVariableRepository>().GetInfo(ID, includes);
        }

        public sysBpmsVariable GetInfo(Guid? ProcessID, Guid? ApplicationPageID, string Name, string[] includes = null)
        {
            return this.UnitOfWork.Repository<IVariableRepository>().GetInfo(ProcessID, ApplicationPageID, Name, includes);
        }
        /// <summary>
        /// .OrderBy(c => c.Name)
        /// </summary>
        public List<sysBpmsVariable> GetList(Guid? ProcessID, Guid? ApplicationPageID, int? VarTypeLU, string Name, Guid? EntityDefID, bool? EntityIsActive, string[] includes = null)
        {
            return this.UnitOfWork.Repository<IVariableRepository>().GetList(ProcessID, ApplicationPageID, VarTypeLU, Name, EntityDefID, EntityIsActive, includes);
        }
        public List<sysBpmsVariable> GetList(Guid dbConnectionID)
        {
            return this.UnitOfWork.Repository<IVariableRepository>().GetList(dbConnectionID);
        }
        /// <summary>
        /// this method make a list of variables name and Entity.PropertyName from variables which are object.
        /// </summary>
        public List<ComboTreeModel> GetVariableAsComboTree(Guid? ProcessID, Guid? ApplicationPageID, sysBpmsVariable.e_RelationTypeLU? notRelationTypeLU, string template = "")
        {
            List<ComboTreeModel> Items = new List<ComboTreeModel>();
            List<sysBpmsVariable> Variables = new VariableService(this.UnitOfWork).GetList(ProcessID, ApplicationPageID, null, "", null, true, new string[] { nameof(sysBpmsVariable.EntityDef) }).Where(c => (!notRelationTypeLU.HasValue || c.RelationTypeLU != (int)notRelationTypeLU) && c.VarTypeLU != (int)sysBpmsVariable.e_VarTypeLU.List).ToList();

            foreach (sysBpmsVariable item in Variables)
            {
                if (item.VarTypeLU == (int)sysBpmsVariable.e_VarTypeLU.Object)
                {
                    sysBpmsEntityDef entityDef = new EntityDefService(this.UnitOfWork).GetInfo(item.EntityDefID.Value);
                    Items.Add(new ComboTreeModel() { title = item.Name, id = item.Name, state = "closed", });
                    foreach (EntityPropertyModel Property in entityDef.Properties.Where(c => c.IsActive))
                    {
                        Items.LastOrDefault().subs.Add(new ComboTreeModel()
                        {
                            title = item.Name + "." + Property.Name,
                            id = !string.IsNullOrWhiteSpace(template) ? string.Format(template, (item.Name + "." + Property.Name)) :
                             (item.Name + "." + Property.Name),
                        });
                    }
                    Items.LastOrDefault().subs.Add(new ComboTreeModel()
                    {
                        title = item.Name + ".ID",
                        id = !string.IsNullOrWhiteSpace(template) ? string.Format(template, (item.Name + ".ID")) :
                             (item.Name + ".ID"),
                    });
                }
                else
                    Items.Add(new ComboTreeModel()
                    {
                        title = item.Name,
                        id = !string.IsNullOrWhiteSpace(template) ? string.Format(template, (item.Name)) : item.Name
                    });
            }
            return Items;
        }
    }
}
