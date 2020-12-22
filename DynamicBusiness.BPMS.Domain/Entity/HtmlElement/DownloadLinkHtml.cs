using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    [KnownType(typeof(DownloadLinkHtml))]
    public class DownloadLinkHtml : BindingElementBase
    {
        public DownloadLinkHtml() { }
        public DownloadLinkHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId, isFormReadOnly)
        {
            this.VariableId = DomainUtility.toString(obj["entityVariableId"]);
            this.DocumentDefId = DomainUtility.toString(obj["documentDefId"]).ToGuidObjNull();
            this.DocumentFolderId = obj["documentFolderId"].ToGuidObjNull();
            this.ListDocument = new List<Guid>();
            if (base.Helper.FormAction == HtmlElementHelperModel.e_FormAction.Onload &&
                (this.DocumentFolderId.HasValue || this.DocumentDefId.HasValue))
                this.ListDocument = base.Helper?.DocumentEngine?.GetList(this.DocumentDefId.ToGuidObjNull(),
                    this.VariableId.ToGuidObjNull(), this.DocumentFolderId).Select(c => new { c.GUID, c.CaptionOf }).ToList();
        }
        [DataMember]
        public Guid? ThreadID { get { return base.Helper?.DataManageHelper?.ThreadID; } private set { } }
        [DataMember]
        public string VariableId { get; set; }
        [DataMember]
        public Guid? DocumentDefId { get; set; }
        [DataMember]
        public Guid? DocumentFolderId { get; set; }
        [DataMember]
        public object ListDocument { get; set; }
    }
}
