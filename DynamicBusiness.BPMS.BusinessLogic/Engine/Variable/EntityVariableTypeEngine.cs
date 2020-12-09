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
    public class EntityVariableTypeEngine : VariableTypeEngineBase
    {
        private sysBpmsEntityDef EntityDef { get; set; }

        public EntityVariableTypeEngine(EngineSharedModel engineSharedModel, sysBpmsVariable variable, Guid? processID, Guid? threadID, List<QueryModel> additionalParams, IUnitOfWork unitOfWork = null) : base(engineSharedModel, variable, processID, threadID, additionalParams, unitOfWork)
        {
            this.EntityDef = new EntityDefService(base.UnitOfWork).GetInfo(Variable.EntityDefID.Value);
        }
        /// <param name="containerQuery">It is generally used in combosearch which add a parent query that filter table's rows according to query parameter and text field</param>
        public List<DataModel> GetResult(PagingProperties currentPaging, string containerQuery = null, string[] includes = null)
        {
            List<DataModel> dataModel = new List<DataModel>();
            DataTable dataTable = new EntityDefEngine(base.EngineSharedModel, base.UnitOfWork).GetEntity(EntityDef, this.Variable, base.AdditionalParams, currentPaging, containerQuery, includes);
            foreach (DataRow _row in dataTable.Rows)
            {
                dataModel.Add(new DataModel(_row));
            }
            return dataModel;
        }

        public override ResultOperation SaveValues(DataModel dataModel, Dictionary<string, DataModel> allSavedEntities = null)
        {
            //this kind of Variable are bind to one property and therefore DataModel item is equivalent to variable name.
            var result = new EntityDefEngine(base.EngineSharedModel, base.UnitOfWork).SaveIntoDataBase(this.EntityDef, this.Variable, dataModel, base.AdditionalParams, allSavedEntities);
            result.Item1.CurrentObject = result.Item2;
            return result.Item1;
        }

    }
}
