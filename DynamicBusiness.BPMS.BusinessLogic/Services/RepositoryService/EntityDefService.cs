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
                List<sysBpmsEntityDef> listEntity = this.GetList(string.Empty, null);
                this.BeginTransaction();
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
                List<sysBpmsEntityDef> listEntity = this.GetList(string.Empty, null);
                this.BeginTransaction();
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

                List<string> relatedEntity = this.GetList(EntityDefId);
                if (relatedEntity.Any())
                    resultOperation.AddError($"This entity is related to {string.Join(",", relatedEntity)}");

                if (listVariable.Count > 0)
                    resultOperation.AddError(string.Format(LangUtility.Get("DeleteError.Text", nameof(sysBpmsEntityDef)), string.Join(" ,", listVariable.Select(d => d.Name))));

                if (resultOperation.IsSuccess)
                {
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
        public List<sysBpmsEntityDef> GetList(string Name, bool? IsActive, PagingProperties currentPaging = null)
        {
            return this.UnitOfWork.Repository<IEntityDefRepository>().GetList(Name, IsActive, currentPaging);
        }

        public List<string> GetList(Guid relationToEntityId)
        {
            return this.UnitOfWork.Repository<IEntityDefRepository>().GetList(relationToEntityId);
        }

        #region ..:: Update Database ::..
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rModel">current EntityRelationModel which is evaluated.</param>
        /// <returns></returns>
        private void CreateConstraintQuery(EntityPropertyModel rModel, sysBpmsEntityDef entityDef, List<string> listQueries)
        {
            sysBpmsEntityDef foreignEntity = this.GetInfo(rModel.RelationToEntityID.Value);
            string constaintName = this.GetConstraintName(rModel.Name, entityDef.FormattedTableName, foreignEntity.FormattedTableName);
            //generate foreign key query
            listQueries.Add($@"ALTER TABLE [{entityDef.FormattedTableName}] 
WITH CHECK ADD  CONSTRAINT [{constaintName}] FOREIGN KEY([{rModel.Name}])
REFERENCES [{foreignEntity.FormattedTableName}] ([ID])");

            listQueries.Add($@"ALTER TABLE  [{entityDef.FormattedTableName}] CHECK CONSTRAINT [{constaintName}]");
            rModel.RelationConstaintName = constaintName;
        }


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

                foreach (EntityPropertyModel property in entityDef.Properties.Where(c => c.DbType == EntityPropertyModel.e_dbType.Entity))
                {
                    this.CreateConstraintQuery(property, entityDef, executeAlterQueries);
                }

                dataBaseQueryService.ExecuteBySqlQuery(sqlQuery, false, null);

            }
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
                        //if property is no longer a entity type drop relation constraint
                        if (currentProperty.RelationToEntityID.HasValue && !newProperty.RelationToEntityID.HasValue)
                            addQuery.Add($@"ALTER TABLE {currentEntityDef.FormattedTableName} DROP CONSTRAINT {currentProperty.RelationConstaintName};");
                        else
                        {
                            if (currentProperty.RelationToEntityID != newProperty.RelationToEntityID)
                            {     //if it had relation add drop constraint relation query
                                if (currentProperty.RelationToEntityID.HasValue)
                                {
                                    addQuery.Add($@"ALTER TABLE {currentEntityDef.FormattedTableName} DROP CONSTRAINT {currentProperty.RelationConstaintName};");
                                }
                                this.CreateConstraintQuery(newProperty, newEntityDef, addQuery);
                            }
                        }
                    }
                    else
                    {
                        addQuery.Add($@"ALTER TABLE {currentEntityDef.FormattedTableName} ADD {newProperty.Name} {newProperty.SqlTypeName} {newProperty.SqlDefaultValue} {(newProperty.Required ? "NOT NULL" : "NULL")} ;");
                        //Create Constraint for new properties
                        if (newProperty.RelationToEntityID.HasValue)
                            this.CreateConstraintQuery(newProperty, newEntityDef, addQuery);
                    }


                }

                //deleted properties
                foreach (EntityPropertyModel currentProperty in currentEntityDef.Properties.Where(c => !newEntityDef.Properties.Any(d => d.ID == c.ID)))
                {
                    //drop default CONSTRAINT
                    if (!string.IsNullOrWhiteSpace(currentProperty.DefaultValue))
                        addQuery.Add($@" ALTER TABLE {currentEntityDef.FormattedTableName} DROP CONSTRAINT def_{currentEntityDef.FormattedTableName}_{currentProperty.Name} ; ");
                    //if it has relation add drop constraint relation query
                    if (currentProperty.DbType == EntityPropertyModel.e_dbType.Entity)
                    {
                        addQuery.Add($@"ALTER TABLE {currentEntityDef.FormattedTableName} DROP CONSTRAINT {currentProperty.RelationConstaintName};");
                    }
                    //drop property query
                    addQuery.Add($@"ALTER TABLE {currentEntityDef.FormattedTableName} DROP COLUMN {currentProperty.Name};");

                }

            }
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

            foreach (EntityPropertyModel rModel in entityDef.Properties.Where(c => c.DbType == EntityPropertyModel.e_dbType.Entity))
            {
                executeAlterQueries.Add($@"ALTER TABLE {entityDef.FormattedTableName} DROP CONSTRAINT {rModel.RelationConstaintName}");
            }
            foreach (string query in executeAlterQueries)
            {
                dataBaseQueryService.ExecuteBySqlQuery(query, false, null);
            }
            string sqlQuery = $@"Drop TABLE [{entityDef.FormattedTableName}] ";
            dataBaseQueryService.ExecuteBySqlQuery(sqlQuery, false, null);

            return resultOperation;
        }

        private string GetConstraintName(string propertName, string tableName, string foreigntableName)
        {
            return $"FK_{tableName}_{foreigntableName}_{propertName}";
        }
        #endregion

    }
}
