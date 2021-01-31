using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    public class HtmlElementHelperModel
    {
        public HtmlElementHelperModel()
        {

        }
        public enum e_FormAction
        {
            Onload = 1,//load from database
            OnPost = 2,//dont load from database
            Preview = 3,
            FillMode = 4,//dont load from database
        }
        public ResultOperation ResultOperation { get; set; }
        [DataMember]
        public bool IsEncrypted { get; set; }
        public string ApiSessionId { get; set; }
        public IApplicationPageEngine applicationPageEngine { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }
        public IDataManageEngine DataManageHelper { get; set; }
        public IDynamicCodeEngine DynamicCodeEngine { get; set; }
        public IDocumentEngine DocumentEngine { get; set; }
        /// <summary>
        /// All the parameters that can used in variable retrieve query are in this list.
        /// </summary>
        public List<QueryModel> ListFormQueryModel { get; set; }
        [DataMember]
        public string Script { get; private set; }
        [DataMember]
        public string StyleSheet { get; private set; }
        /// <summary>
        /// add all control scripts to end of the page
        /// </summary>
        public void AddScript(string script)
        {
            this.Script = this.Script ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(script))
                this.Script += Environment.NewLine + script;
        }
        public void AddStyleSheet(string styleSheet)
        {
            this.StyleSheet = this.StyleSheet ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(styleSheet))
                this.StyleSheet += Environment.NewLine + styleSheet;
        }
        public e_FormAction FormAction { get; set; }

        public HtmlElementHelperModel(IDataManageEngine dataManageHelper, IUnitOfWork unitOfWork, IApplicationPageEngine applicationPageEngine,
            e_FormAction formAction, IDynamicCodeEngine dynamicCodeEngine, IDocumentEngine documentEngine,
            List<QueryModel> listFormQueryModel, string apiSessionID, bool isEncrypted)
        {
            this.ResultOperation = new ResultOperation();
            this.FormAction = formAction;
            this.DataManageHelper = dataManageHelper;
            this.UnitOfWork = unitOfWork;
            this.applicationPageEngine = applicationPageEngine;
            this.DynamicCodeEngine = dynamicCodeEngine;
            this.DocumentEngine = documentEngine;
            this.ListFormQueryModel = listFormQueryModel;
            this.ApiSessionId = apiSessionID;
            this.IsEncrypted = isEncrypted;
        }
    }
}
