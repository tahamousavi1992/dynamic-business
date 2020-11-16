using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class OperationCodeHelper
    {
        public EngineSharedModel EngineSharedModel { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }
        public OperationCodeHelper(EngineSharedModel engineSharedModel, IUnitOfWork unitOfWork)
        {
            this.EngineSharedModel = engineSharedModel;
            this.UnitOfWork = unitOfWork;
        }
        /// <summary>
        /// this method execute a query's operation and return a object.
        /// </summary>
        /// <returns>if it is a ExecuteNonQuery,it will return 1 or 0</returns>
        public object RunQuery(Guid operationId, params QueryModel[] queryModels)
        {
            sysBpmsOperation operation = new OperationService(this.UnitOfWork).GetInfo(operationId);
            if (operation != null)
            {
                return new OperationEngine(this.EngineSharedModel, this.UnitOfWork).RunQuery(operation, queryModels);
            }
            else return null;
        }

    }
}
