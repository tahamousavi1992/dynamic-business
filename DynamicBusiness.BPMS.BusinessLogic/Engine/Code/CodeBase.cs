
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DynamicBusiness.BPMS.CodePanel
{
    /// <summary>
    /// for making dynamic methods and using in dynamic classes as base class .
    /// </summary>
    public abstract class CodeBase
    {
        #region .:: Private Properties::.

        private Guid? ProcessID { get; set; }

        private Guid? ApplicationPageId { get; set; }
        private BusinessLogic.DataManageEngine DataManageHelperService { get; set; }
        private Domain.FormModel formModel { get; set; }

        private BusinessLogic.VariableCodeHelper _variableHelper { get; set; }
        private BusinessLogic.ControlCodeHelper _controlHelper { get; set; }
        private BusinessLogic.AccessCodeHelper _AsccessHelper { get; set; }
        private BusinessLogic.ProcessCodeHelper _ProcessHelper { get; set; }
        private BusinessLogic.MessageCodeHelper _messageHelper { get; set; }
        private BusinessLogic.OperationCodeHelper _operationHelper { get; set; }
        private BusinessLogic.UserCodeHelper _userHelper { get; set; }
        private BusinessLogic.UrlCodeHelper _urlHelper { get; set; }
        private BusinessLogic.DocumentCodeHelper _documentHelper { get; set; }
        private BusinessLogic.QueryCodeHelper _queryHelper { get; set; }
        private BusinessLogic.EntityCodeHelper _entityHelper { get; set; }
        private BusinessLogic.WebServiceCodeHelper _webServiceHelper { get; set; }
        #endregion

        #region .:: protected Properties::.
        protected Domain.EngineSharedModel EngineSharedModel { get; private set; }
        protected Domain.IUnitOfWork UnitOfWork { get; private set; }
        protected BusinessLogic.VariableCodeHelper VariableHelper { get { if (this._variableHelper == null) { this._variableHelper = new BusinessLogic.VariableCodeHelper(this.DataManageHelperService, this.CodeBaseShared); } return this._variableHelper; } }
        protected BusinessLogic.ControlCodeHelper ControlHelper { get { if (this._controlHelper == null) { this._controlHelper = new BusinessLogic.ControlCodeHelper(this.formModel); } return this._controlHelper; } }
        protected BusinessLogic.AccessCodeHelper AccessHelper { get { if (this._AsccessHelper == null) { this._AsccessHelper = new BusinessLogic.AccessCodeHelper(UnitOfWork); } return this._AsccessHelper; } }
        protected BusinessLogic.ProcessCodeHelper ProcessHelper { get { if (this._ProcessHelper == null) { this._ProcessHelper = new BusinessLogic.ProcessCodeHelper(UnitOfWork); } return this._ProcessHelper; } }
        protected BusinessLogic.MessageCodeHelper MessageHelper { get { if (this._messageHelper == null) { this._messageHelper = new BusinessLogic.MessageCodeHelper(this.CodeBaseShared); } return this._messageHelper; } }
        protected BusinessLogic.OperationCodeHelper OperationHelper { get { if (this._operationHelper == null) { this._operationHelper = new BusinessLogic.OperationCodeHelper(this.EngineSharedModel, UnitOfWork); } return this._operationHelper; } }
        protected BusinessLogic.UserCodeHelper UserHelper { get { if (this._userHelper == null) { this._userHelper = new BusinessLogic.UserCodeHelper(this.EngineSharedModel, UnitOfWork); } return this._userHelper; } }
        public BusinessLogic.UrlCodeHelper UrlHelper { get { if (this._urlHelper == null) { this._urlHelper = new BusinessLogic.UrlCodeHelper(this.BaseQueryModel); } return this._urlHelper; } }
        protected BusinessLogic.DocumentCodeHelper DocumentHelper { get { if (this._documentHelper == null) { this._documentHelper = new BusinessLogic.DocumentCodeHelper(this.EngineSharedModel, UnitOfWork, CodeBaseShared); } return this._documentHelper; } }
        protected BusinessLogic.QueryCodeHelper QueryHelper { get { if (this._queryHelper == null) { this._queryHelper = new BusinessLogic.QueryCodeHelper(UnitOfWork); } return this._queryHelper; } }
        protected BusinessLogic.EntityCodeHelper EntityHelper { get { if (this._entityHelper == null) { this._entityHelper = new BusinessLogic.EntityCodeHelper(this.EngineSharedModel, this.QueryHelper, UnitOfWork); } return this._entityHelper; } }
        protected BusinessLogic.WebServiceCodeHelper WebServiceHelper { get { if (this._webServiceHelper == null) { this._webServiceHelper = new BusinessLogic.WebServiceCodeHelper(this.DataManageHelperService, this.CodeBaseShared); } return this._webServiceHelper; } }

        #endregion

        #region .:: Public Properties::.
        public Guid? CurrentUserID { get; private set; }
        public string CurrentUserName { get; private set; }
        public Guid? ThreadUserID
        {
            get
            {
                return this.ThreadID.HasValue ? new BusinessLogic.ThreadService(this.UnitOfWork).GetInfo(this.ThreadID.Value).UserID : default(Guid?);
            }
        }
        public Guid? ThreadID { get; private set; }
        #endregion

        #region .:: internal Properties::.
        internal List<Domain.QueryModel> BaseQueryModel { get; set; }
        internal Domain.CodeBaseSharedModel CodeBaseShared { get; private set; }
        #endregion

        #region .:: abstract ::.

        public abstract object ExecuteCode();

        #endregion

        #region .:: internal Method ::.

        public void InitialProperties(Guid? processID, Guid? threadID, Guid? applicationPageId,
            Domain.IUnitOfWork unitOfWork, List<Domain.QueryModel> baseQueryModel,
            Guid? currentUserID, string currentUserName, Domain.FormModel formModel,
            string apiSessionId, Domain.CodeBaseSharedModel codeBaseShared)
        {
            this.formModel = formModel;
            this.EngineSharedModel = applicationPageId.HasValue ?
                new Domain.EngineSharedModel(applicationPageId.Value, baseQueryModel, currentUserName, apiSessionId) :
                new Domain.EngineSharedModel(threadID, processID, baseQueryModel, currentUserName, apiSessionId);
            this.BaseQueryModel = baseQueryModel;

            this.UnitOfWork = unitOfWork ?? new BusinessLogic.UnitOfWork();
            this.ProcessID = processID;
            this.ThreadID = threadID;
            this.ApplicationPageId = applicationPageId;
            this.CurrentUserID = currentUserID;
            this.CurrentUserName = currentUserName;
            if (applicationPageId.HasValue)
                this.DataManageHelperService = new BusinessLogic.DataManageEngine(this.EngineSharedModel, this.UnitOfWork);
            else
                this.DataManageHelperService = new BusinessLogic.DataManageEngine(new Domain.EngineSharedModel(this.ThreadID, this.ProcessID, baseQueryModel, currentUserName, apiSessionId), this.UnitOfWork);
            this.CodeBaseShared = codeBaseShared ?? new Domain.CodeBaseSharedModel();
        }

        public void InitialProperties(Domain.IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork ?? new BusinessLogic.UnitOfWork();
        }

        #endregion
    }

}

