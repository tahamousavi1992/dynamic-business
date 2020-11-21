using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class EntityCodeHelper : IEntityCodeHelper
    {
        public EngineSharedModel EngineSharedModel { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }
        public IQueryCodeHelper QueryHelper { get; set; }
        public EntityCodeHelper(EngineSharedModel engineSharedModel, IQueryCodeHelper queryHelper, IUnitOfWork unitOfWork)
        {
            this.EngineSharedModel = engineSharedModel;
            this.UnitOfWork = unitOfWork;
            this.QueryHelper = queryHelper;
        }
        public VariableModel GetById(string entityName, Guid id)
        {
            DataTable dataTable = this.QueryHelper.Get($"select * from Bpms_{entityName.Trim()} where ID='{id}' ");
            dataTable.TableName = "Bpms_" + entityName;
            return dataTable;
        }

        public void DeleteById(string entityName, Guid id)
        {
            this.QueryHelper.Execute($"delete from Bpms_{entityName.Trim()} where ID='{id}' ");
        }

        public void Save(VariableModel variableModel)
        {
            new EntityDefEngine(this.EngineSharedModel, this.UnitOfWork).SaveIntoDataBase(variableModel);
        }

        public void Save<T>(T entity)
        {
            new EntityDefEngine(this.EngineSharedModel, this.UnitOfWork).SaveIntoDataBase(VariableModel.ConvertFrom<T>(entity));
        }
    }
}
