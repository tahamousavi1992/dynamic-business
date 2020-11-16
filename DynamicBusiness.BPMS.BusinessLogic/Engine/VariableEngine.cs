using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using System.Web.Script.Serialization;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class VariableEngine : BaseEngine
    {
        public VariableEngine(EngineSharedModel engineSharedModel, IUnitOfWork unitOfWork = null) : base(engineSharedModel, unitOfWork) { }

        //this method make where clause for filtering Entity
        public static string GenerateWhereClause(sysBpmsVariable variable)
        {
            string whereClause;
            if (string.IsNullOrWhiteSpace(variable.WhereClause))
            {
                whereClause = variable.FilterTypeLU == (int)sysBpmsVariable.e_FilterTypeLU.AllEntities ?
                  "" : " where ThreadID=@ThreadID ";
            }

            else
                whereClause = $" where ({variable.WhereClause})" +
                    (variable.FilterTypeLU == (int)sysBpmsVariable.e_FilterTypeLU.AllEntities ? "" : " and ThreadID=@ThreadID");
            return whereClause;
        }

        //this method make order clause for filtering Entity
        public static string GenerateOrderClause(sysBpmsVariable variable)
        {
            return !string.IsNullOrWhiteSpace(variable.OrderByClause) ? $" order by {variable.OrderByClause}" : "";
        }
    }
}
