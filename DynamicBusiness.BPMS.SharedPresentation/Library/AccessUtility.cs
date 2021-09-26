using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    public static class AccessUtility
    {
        /// <summary>
        /// check request comes from single application on same server.
        /// </summary>
        /// <returns></returns>
        public static bool CalledByLocalSA(HttpRequest request)
        {
            return DomainUtility.IsTestEnvironment || (request.IsLocal && request.Headers.AllKeys.Contains("clientId"));
        }
    }
}