using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using System.Data;
using System.Data.SqlClient;


namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class OperationEngine : BaseEngine
    {
        public OperationEngine(EngineSharedModel engineSharedModel, IUnitOfWork unitOfWork = null) : base(engineSharedModel, unitOfWork) { }

        public object RunQuery(sysBpmsOperation operation, params QueryModel[] additionalQM)
        {
            List<DataModel> dataModel = new List<DataModel>();
            Guid processId = base.EngineSharedModel.CurrentProcessID ?? Guid.Empty;

            //get sql query parameter from listFormQueryModel
            List<SqlParameter> queryParams = QueryModel.GetSqlParameter(base.GetAllQueryModels(additionalQM?.ToList()), operation.SqlCommand, base.EngineSharedModel.CurrentThreadID, processId).ToList();

            switch ((sysBpmsOperation.e_TypeLU)operation.TypeLU)
            {
                case sysBpmsOperation.e_TypeLU.Execute:
                    return new DataBaseQueryService(this.UnitOfWork).ExecuteBySqlQuery(operation.SqlCommand, true, queryParams.ToArray());
                case sysBpmsOperation.e_TypeLU.Scalar:
                    return new DataBaseQueryService(this.UnitOfWork).ExecuteScalar<object>(operation.SqlCommand, true, queryParams.ToArray());
                case sysBpmsOperation.e_TypeLU.DataTable:
                    return new DataBaseQueryService(this.UnitOfWork).GetBySqlQuery(operation.SqlCommand, true, null, queryParams.ToArray());

            }
            return null;
        }
    }
}
