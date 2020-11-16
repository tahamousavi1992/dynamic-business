using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public abstract class VariableTypeEngineBase : BaseEngine
    {
        protected sysBpmsVariable Variable { get; set; }
        protected Guid? ProcessID { get; set; }
        protected Guid? ThreadID { get; set; }
        protected List<QueryModel> AdditionalParams { get; set; }
        public VariableTypeEngineBase(EngineSharedModel engineSharedModel, sysBpmsVariable variable, Guid? ProcessID, Guid? threadID, List<QueryModel> additionalParams, IUnitOfWork unitOfWork) : base(engineSharedModel, unitOfWork)
        {
            this.Variable = variable;
            this.ProcessID = ProcessID;
            this.ThreadID = threadID;
            this.AdditionalParams = additionalParams;
        }
        /// <param name="containerQuery">It is generally used in combosearch which add a parent query that filter table's rows according to query parameter and text field</param>
        public abstract List<DataModel> GetResult(PagingProperties currentPaging, string containerQuery = null);
        /// <summary>
        /// save variable into data base.
        /// </summary>
        /// <param name="allSavedEntities">all Entities thats other entity variables need them because they are dependent.</param>
        public abstract ResultOperation SaveValues(DataModel _DataModel, Dictionary<string, DataModel> allSavedEntities = null);
    }
}
