using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IWebServiceCodeHelper
    {
        /// <param name="contentType">is application/json , application/x-www-form-urlencoded</param>
        object Post(string url, string contentType, List<QueryModel> headers, params QueryModel[] parameterModel);

        /// <param name="contentType">is application/json , application/x-www-form-urlencoded</param>
        object Put(string url, string contentType, List<QueryModel> headers, params QueryModel[] parameterModel);

        object Get(string url, List<QueryModel> headers, params QueryModel[] parameterModel);
    }
}
