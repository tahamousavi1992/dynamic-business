using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IOperationCodeHelper
    {
        /// <summary>
        /// this method execute a query's operation and return a object.
        /// </summary>
        /// <returns>if it is a ExecuteNonQuery,it will return 1 or 0</returns>
        object RunQuery(Guid operationId, params QueryModel[] queryModels);
    }
}
