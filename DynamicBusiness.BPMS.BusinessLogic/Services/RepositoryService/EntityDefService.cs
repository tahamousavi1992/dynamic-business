using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using System.Data;
using System.Data.SqlClient;
using Mono.CSharp;
using System.Reflection.Metadata;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class EntityDefService : ServiceBase
    {
        public EntityDefService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsEntityDef entityDef)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                List<sysBpmsEntityDef> listEntity = this.GetList(string.Empty, string.Empty, null);
                this.BeginTransaction();
                if (listEntity.Any(c => c.TableName == entityDef.TableName && c.ID != entityDef.ID))
                {
                    resultOperation.AddError(LangUtility.Get("SameTable.Text", nameof(sysBpmsEntityDef)));
                }
                if (listEntity.Any(c => c.Name == entityDef.Name && c.ID != entityDef.ID))
                {
                    resultOperation.AddError(LangUtility.Get("SameEntity.Text", nameof(sysBpmsEntityDef)));
                }

                if (resultOperation.IsSuccess)
                {
                    this.CreateTable(entityDef);
                    entityDef.DesignXML = new EntityDesignXmlModel()
                    {
                        EntityPropertyModel = entityDef.Properties,
                        EntityRelationModel = entityDef.Relations
                    }.BuildXml();
                    this.UnitOfWork.Repository<IEntityDefRepository>().Add(entityDef);
                    this.UnitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
                return base.ExceptionHandler(ex);
            }
            base.FinalizeService(resultOperation);
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsEntityDef entityDef)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                List<sysBpmsEntityDef> listEntity = this.GetList(string.Empty, string.Empty, null);
                this.BeginTransaction();
                if (listEntity.Any(c => c.TableName == entityDef.TableName && c.ID != entityDef.ID))
                {
                    resultOperation.AddError(LangUtility.Get("SameTable.Text", nameof(sysBpmsEntityDef)));
                }
                if (listEntity.Any(c => c.Name == entityDef.Name && c.ID != entityDef.ID))
                {
                    resultOperation.AddError(LangUtility.Get("SameEntity.Text", nameof(sysBpmsEntityDef)));
                }

                if (resultOperation.IsSuccess)
                {
                    this.UpdateTable(entityDef);
                    entityDef.DesignXML = new EntityDesignXmlModel()
                    {
                        EntityPropertyModel = entityDef.Properties,
                        EntityRelationModel = entityDef.Relations
                    }.BuildXml();
                    this.UnitOfWork.Repository<IEntityDefRepository>().Update(entityDef);
                    this.UnitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
                return base.ExceptionHandler(ex);
            }
            base.FinalizeService(resultOperation);
            return resultOperation;
        }

        public ResultOperation InActive(Guid EntityDefId)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                sysBpmsEntityDef entityDef = this.UnitOfWork.Repository<IEntityDefRepository>().GetInfo(EntityDefId);
                entityDef.IsActive = false;
                this.UnitOfWork.Repository<IEntityDefRepository>().Update(entityDef);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Active(Guid EntityDefId)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                sysBpmsEntityDef entityDef = this.UnitOfWork.Repository<IEntityDefRepository>().GetInfo(EntityDefId);
                entityDef.IsActive = true;
                this.UnitOfWork.Repository<IEntityDefRepository>().Update(entityDef);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(Guid EntityDefId)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                List<sysBpmsDocument> listDoc = new DocumentService().GetList(null, EntityDefId, null, "", null, null, null);
                List<sysBpmsVariable> listVariable = new VariableService().GetList(null, null, null, "", EntityDefId, null);
                this.BeginTransaction();

                if (listVariable.Count > 0)
                    resultOperation.AddError(string.Format(LangUtility.Get("DeleteError.Text", nameof(sysBpmsEntityDef)), string.Join(" ,", listVariable.Select(d => d.Name))));

                DocumentService documentService = new DocumentService(base.UnitOfWork);
                foreach (var item in listDoc)
                {
                    if (resultOperation.IsSuccess)
                        resultOperation = documentService.Delete(item.GUID);
                }

                if (resultOperation.IsSuccess)
                {
                    this.DropTable(this.GetInfo(EntityDefId));
                    this.UnitOfWork.Repository<IEntityDefRepository>().Delete(EntityDefId);
                    this.UnitOfWork.Save();
                }
                listDoc = null;
            }
            catch (Exception ex)
            {
                return base.ExceptionHandler(ex);
            }
            base.FinalizeService(resultOperation);
            return resultOperation;
        }

        public sysBpmsEntityDef GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<IEntityDefRepository>().GetInfo(ID);
        }

        public sysBpmsEntityDef GetInfo(string name)
        {
            return this.UnitOfWork.Repository<IEntityDefRepository>().GetInfo(name);
        }

        /// <param name="TableName">it only search on TableName using like query</param>
        /// <param name="Name">it search on Name,TableName,DisplayName using like query</param>
        /// <returns></returns>
        public List<sysBpmsEntityDef> GetList(string TableName, string Name, bool? IsActive, PagingProperties currentPaging = null)
        {
            return this.UnitOfWork.Repository<IEntityDefRepository>().GetList(TableName, Name, IsActive, currentPaging);
        }

        public List<string> GetList(bool? isActive)
        {
            return this.UnitOfWork.Repository<IEntityDefRepository>().GetList(isActive);
        }

        #region ..:: Update Database ::..
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rModel">current EntityRelationModel which is evaluated.</param>
        /// <returns></returns>
        private void CreateConstraintQuery(EntityRelationModel rModel, sysBpmsEntityDef entityDef, List<string> listQueries, bool dropFirst = false)
        {
            sysBpmsEntityDef foreignEntity = this.GetInfo(rModel.EntityDefID);

            //If It is making a relation for foreign table base on ID of current table.
            if (rModel.EntityDefID != entityDef.ID && rModel.PropertyID == "1")
            {
                if (dropFirst)
                    listQueries.Add($@"ALTER TABLE {foreignEntity.FormattedTableName} DROP CONSTRAINT {rModel.ConstraintName};");

                //add number to end of foreign key name to prevent same foreign key name if this foreign table added already. 
                string numericalIncrement = string.Empty;
                if (foreignEntity.Relations.Count(c => c.EntityDefID == entityDef.ID) > 1)
                    numericalIncrement = (foreignEntity.Relations.Where(c => c.EntityDefID == entityDef.ID).Select((c, i) => new { i, c.ID }).FirstOrDefault(c => c.ID == rModel.ID).i + 1).ToString();
                rModel.ConstraintName = $"FK_{foreignEntity.FormattedTableName}_{entityDef.FormattedTableName + numericalIncrement}";

                //generate foreign key query
                listQueries.Add($@"ALTER TABLE [{foreignEntity.FormattedTableName}] 
WITH CHECK ADD  CONSTRAINT [{rModel.ConstraintName}] FOREIGN KEY([{foreignEntity.AllProperties.FirstOrDefault(c => c.ID == rModel.EntityDefPropertyID).Name}])
REFERENCES [{entityDef.FormattedTableName}] ([{entityDef.AllProperties.FirstOrDefault(c => c.ID == rModel.PropertyID).Name}])");

                listQueries.Add($@"ALTER TABLE  [{foreignEntity.FormattedTableName}] CHECK CONSTRAINT [{rModel.ConstraintName}]");

            }
            else
            {
                if (dropFirst)
                    listQueries.Add($@"ALTER TABLE {entityDef.FormattedTableName} DROP CONSTRAINT {rModel.ConstraintName};");

                //add number to end of foreign key name to prevent same foreign key name if this foreign table added already. 
                string numericalIncrement = string.Empty;
                if (entityDef.Relations.Count(c => c.EntityDefID == foreignEntity.ID) > 1)
                    numericalIncrement = (entityDef.Relations.Where(c => c.EntityDefID == foreignEntity.ID).Select((c, i) => new { i, c.ID }).FirstOrDefault(c => c.ID == rModel.ID).i + 1).ToString();
                rModel.ConstraintName = $"FK_{entityDef.FormattedTableName}_{foreignEntity.FormattedTableName + numericalIncrement}";

                //generate foreign key query
                listQueries.Add($@"ALTER TABLE [{entityDef.FormattedTableName}] 
WITH CHECK ADD  CONSTRAINT [{rModel.ConstraintName}] FOREIGN KEY([{entityDef.AllProperties.FirstOrDefault(c => c.ID == rModel.PropertyID).Name}])
REFERENCES [{foreignEntity.FormattedTableName}] ([{foreignEntity.AllProperties.FirstOrDefault(c => c.ID == rModel.EntityDefPropertyID).Name}])");

                listQueries.Add($@"ALTER TABLE  [{entityDef.FormattedTableName}] CHECK CONSTRAINT [{rModel.ConstraintName}]");

            }

        }

        /// <summary>
        /// publish a specific process by generating table with relations and properties.
        /// </summary>
        public ResultOperation CreateTable(sysBpmsEntityDef entityDef)
        {
            DataBaseQueryService dataBaseQueryService = new DataBaseQueryService(base.UnitOfWork);

            ResultOperation resultOperation = new ResultOperation();
            List<string> executeAlterQueries = new List<string>();

            if (entityDef.IsActive && entityDef.Properties != null && entityDef.Properties.Any())
            {
                string paramsQuery = string.Empty;

                foreach (EntityPropertyModel property in entityDef.Properties)
                {
                    paramsQuery += $"[{property.Name}] {property.SqlTypeName} {(property.Required ? "NOT NULL" : "NULL")} ,";
                }

                //generate table create query.
                string sqlQuery =
$@"CREATE TABLE [{entityDef.FormattedTableName}](
[ID][uniqueidentifier] NOT NULL DEFAULT newid() PRIMARY KEY,
[ThreadID][uniqueidentifier] NULL,
{paramsQuery.TrimEnd(',')}) ";

                foreach (EntityRelationModel rModel in entityDef.Relations)
                {
                    this.CreateConstraintQuery(rModel, entityDef, executeAlterQueries);
                }

                dataBaseQueryService.ExecuteBySqlQuery(sqlQuery, false, null);
                 
            }
            this.UpdateDependentEntity(entityDef, entityDef);
            foreach (EntityPropertyModel property in entityDef.Properties.Where(c => !string.IsNullOrWhiteSpace(c.DefaultValue)))
            {
                executeAlterQueries.Add($@" ALTER TABLE {entityDef.FormattedTableName} ADD CONSTRAINT def_{entityDef.FormattedTableName}_{property.Name} {property.SqlDefaultValue} FOR {property.Name} ;");
            }
            foreach (string query in executeAlterQueries)
            {
                dataBaseQueryService.ExecuteBySqlQuery(query, false, null);
            }
            return resultOperation;
        }

        public ResultOperation UpdateTable(sysBpmsEntityDef newEntityDef)
        {
            DataBaseQueryService dataBaseQueryService = new DataBaseQueryService(base.UnitOfWork);
            sysBpmsEntityDef currentEntityDef = this.GetInfo(newEntityDef.ID);

            ResultOperation resultOperation = new ResultOperation();

            List<string> addQuery = new List<string>();

            if (newEntityDef.IsActive && newEntityDef.Properties != null && newEntityDef.Properties.Any())
            {

                foreach (EntityPropertyModel newProperty in newEntityDef.Properties)
                {
                    //change Property
                    if (currentEntityDef.Properties.Any(c => c.ID == newProperty.ID))
                    {
                        EntityPropertyModel currentProperty = currentEntityDef.Properties.FirstOrDefault(c => c.ID == newProperty.ID);
                        //change property name.
                        if (currentProperty.Name != newProperty.Name)
                        {
                            addQuery.Add($" EXEC sp_rename '{currentEntityDef.FormattedTableName}.{currentProperty.Name}', '{newProperty.Name}', 'COLUMN'; {Environment.NewLine} ");
                        }
                        if (currentProperty.DbType != newProperty.DbType ||
                            currentProperty.SqlTypeName != newProperty.SqlTypeName ||
                            currentProperty.Required != newProperty.Required)
                        {
                            addQuery.Add($@" ALTER TABLE {currentEntityDef.FormattedTableName} ALTER COLUMN {newProperty.Name} {newProperty.SqlTypeName} {newProperty.SqlDefaultValue} {(newProperty.Required ? "NOT NULL" : "NULL")} ; ");
                        }
                        if (currentProperty.DefaultValue != newProperty.DefaultValue)
                        {
                            if (!string.IsNullOrWhiteSpace(currentProperty.DefaultValue))
                                addQuery.Add($@" ALTER TABLE {currentEntityDef.FormattedTableName} DROP CONSTRAINT def_{currentEntityDef.FormattedTableName}_{currentProperty.Name} ; ");
                            addQuery.Add($@" ALTER TABLE {currentEntityDef.FormattedTableName} ADD CONSTRAINT def_{currentEntityDef.FormattedTableName}_{newProperty.Name} {newProperty.SqlDefaultValue} FOR {newProperty.Name} ;");
                        }
                    }
                    else
                    {
                        addQuery.Add($@"ALTER TABLE {currentEntityDef.FormattedTableName} ADD {newProperty.Name} {newProperty.SqlTypeName} {newProperty.SqlDefaultValue} {(newProperty.Required ? "NOT NULL" : "NULL")} ;");
                    }
                }

                //deleted properties
                foreach (EntityPropertyModel currentProperty in currentEntityDef.Properties.Where(c => !newEntityDef.Properties.Any(d => d.ID == c.ID)))
                {
                    //currentProperty.IsActive = false;
                    //newEntityDef.Properties.Add(currentProperty);
                    addQuery.Add($@"ALTER TABLE {currentEntityDef.FormattedTableName} DROP COLUMN {currentProperty.Name};");
                }

                //deleted Relations
                foreach (EntityRelationModel currentRelation in currentEntityDef.Relations.Where(c => !newEntityDef.Relations.Any(d => d.ID == c.ID)))
                {
                    addQuery.Add($@"ALTER TABLE {currentEntityDef.FormattedTableName} DROP CONSTRAINT {currentRelation.ConstraintName};");
                }

                foreach (EntityRelationModel newRelation in newEntityDef.Relations)
                {
                    //change Relation
                    if (currentEntityDef.Relations.Any(c => c.ID == newRelation.ID))
                    {
                        EntityRelationModel currentRelation = currentEntityDef.Relations.FirstOrDefault(c => c.ID == newRelation.ID);
                        if (currentRelation.EntityDefID != newRelation.EntityDefID ||
                            currentRelation.EntityDefPropertyID != newRelation.EntityDefPropertyID ||
                            currentRelation.PropertyID != newRelation.PropertyID)
                        {
                            this.CreateConstraintQuery(newRelation, newEntityDef, addQuery, true);
                        }
                    }
                    else
                    {
                        this.CreateConstraintQuery(newRelation, newEntityDef, addQuery);
                    }
                }
            }
            this.UpdateDependentEntity(currentEntityDef, newEntityDef);
            foreach (string query in addQuery)
            {
                dataBaseQueryService.ExecuteBySqlQuery(query, false, null);
            }
            return resultOperation;
        }

        /// <summary>
        /// publish a specific process by generating table with relations and properties.
        /// </summary>
        public ResultOperation DropTable(sysBpmsEntityDef entityDef)
        {
            DataBaseQueryService dataBaseQueryService = new DataBaseQueryService(base.UnitOfWork);
            ResultOperation resultOperation = new ResultOperation();
            List<string> executeAlterQueries = new List<string>();

            foreach (EntityRelationModel rModel in entityDef.Relations)
            {
                if (rModel.EntityDefID != entityDef.ID && rModel.PropertyID == "1")
                {
                    sysBpmsEntityDef foreignEntity = this.GetInfo(rModel.EntityDefID);
                    executeAlterQueries.Add($@"ALTER TABLE {foreignEntity.FormattedTableName} DROP CONSTRAINT {rModel.ConstraintName}");
                }
                else
                    executeAlterQueries.Add($@"ALTER TABLE {entityDef.FormattedTableName} DROP CONSTRAINT {rModel.ConstraintName}");

            }
            foreach (string query in executeAlterQueries)
            {
                dataBaseQueryService.ExecuteBySqlQuery(query, false, null);
            }
            string sqlQuery = $@"Drop TABLE [{entityDef.FormattedTableName}] ";
            dataBaseQueryService.ExecuteBySqlQuery(sqlQuery, false, null);

            return resultOperation;
        }

        private void UpdateDependentEntity(sysBpmsEntityDef currentEntity, sysBpmsEntityDef newEntity)
        {
            //deleted Relations
            foreach (EntityRelationModel currentRelation in currentEntity.Relations.Where(c => !newEntity.Relations.Any(d => d.ID == c.ID)))
            {
                if (currentRelation.EntityDefID != currentEntity.ID)
                {
                    sysBpmsEntityDef relatedEntity = this.GetInfo(currentRelation.EntityDefID);
                    relatedEntity.DesignXML = new EntityDesignXmlModel()
                    {
                        EntityPropertyModel = relatedEntity.Properties,
                        EntityRelationModel = relatedEntity.Relations.Where(c => c.ID != currentRelation.ID).ToList()
                    }.BuildXml();
                    this.UnitOfWork.Repository<IEntityDefRepository>().Update(relatedEntity);
                    this.UnitOfWork.Save();
                }
            }

            foreach (EntityRelationModel newRelation in newEntity.Relations)
            {
                EntityRelationModel cloneRelation = new EntityRelationModel()
                {
                    ID = newRelation.ID,
                    ConstraintName = newRelation.ConstraintName,
                    Description = newRelation.Description,
                    EntityDefID = currentEntity.ID,
                    EntityDefPropertyID = newRelation.PropertyID,
                    PropertyID = newRelation.EntityDefPropertyID,
                };

                //change Relation
                if (currentEntity.Relations.Any(c => c.ID == newRelation.ID))
                {
                    EntityRelationModel currentRelation = currentEntity.Relations.FirstOrDefault(c => c.ID == newRelation.ID);
                    if (currentRelation.EntityDefID != newRelation.EntityDefID ||
                        currentRelation.EntityDefPropertyID != newRelation.EntityDefPropertyID ||
                        currentRelation.PropertyID != newRelation.PropertyID ||
                        currentRelation.Description != newRelation.Description)
                    {
                        if (newRelation.EntityDefID != currentEntity.ID)
                        {
                            sysBpmsEntityDef relatedEntity = this.GetInfo(newRelation.EntityDefID);
                            relatedEntity.DesignXML = new EntityDesignXmlModel()
                            {
                                EntityPropertyModel = relatedEntity.Properties,
                                EntityRelationModel = relatedEntity.Relations.Where(c => c.ID != cloneRelation.ID).Union(new List<EntityRelationModel>() { cloneRelation }).ToList()
                            }.BuildXml();
                            base.UnitOfWork.Repository<IEntityDefRepository>().Update(relatedEntity);
                            base.UnitOfWork.Save();
                        }
                    }
                }
                else
                {
                    if (newRelation.EntityDefID != currentEntity.ID)
                    {
                        sysBpmsEntityDef relatedEntity = this.GetInfo(newRelation.EntityDefID);
                        relatedEntity.DesignXML = new EntityDesignXmlModel()
                        {
                            EntityPropertyModel = relatedEntity.Properties,
                            EntityRelationModel = relatedEntity.Relations.Union(new List<EntityRelationModel>() { cloneRelation }).ToList()
                        }.BuildXml();
                        base.UnitOfWork.Repository<IEntityDefRepository>().Update(relatedEntity);
                        base.UnitOfWork.Save();
                    }
                }
            }
        }
        #endregion

    }
}
