using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public class EngineSharedModel
    {
        public EngineSharedModel(sysBpmsThread currentThread, Guid? currentProcessID, List<QueryModel> baseQueryModel, string currentUserName, string apiSessionId)
        {
            this.CurrentThread = currentThread;
            this.CurrentProcessID = currentProcessID;
            this.BaseQueryModel = baseQueryModel;
            this.CurrentThreadID = currentThread?.ID ?? Guid.Empty;
            this.CurrentUserName = currentUserName;
            this.ApiSessionID = apiSessionId;
        }
        public EngineSharedModel(Guid? currentThreadID, Guid? currentProcessID, List<QueryModel> baseQueryModel, string currentUserName, string apiSessionId)
        {
            this.CurrentThreadID = currentThreadID;
            this.CurrentProcessID = currentProcessID;
            this.BaseQueryModel = baseQueryModel;
            this.CurrentUserName = currentUserName;
            this.ApiSessionID = apiSessionId;
        }
        public EngineSharedModel(Guid applicationPageID, List<QueryModel> baseQueryModel, string currentUserName, string apiSessionId)
        {
            this.CurrentApplicationPageID = applicationPageID;
            this.BaseQueryModel = baseQueryModel;
            this.CurrentUserName = currentUserName;
            this.ApiSessionID = apiSessionId;
        }

        public EngineSharedModel(List<QueryModel> baseQueryModel, string currentUserName, string apiSessionId)
        {
            this.BaseQueryModel = baseQueryModel;
            this.CurrentUserName = currentUserName;
            this.ApiSessionID = apiSessionId;
        }
 
        /// <summary>
        /// it is set by request parameter and form and also it can be used throught process of flow.
        /// it is the result of QueryModel.GetList(request).ToList()
        /// </summary>
        public List<QueryModel> BaseQueryModel { get; set; }
        public sysBpmsThread CurrentThread { get; set; }
        public Guid? CurrentProcessID { get; set; }
        public Guid? CurrentThreadID { get; set; }
        public string CurrentUserName { get; set; }
        public Guid? CurrentApplicationPageID { get; set; }
        public string ApiSessionID { get; set; }
    }
}
