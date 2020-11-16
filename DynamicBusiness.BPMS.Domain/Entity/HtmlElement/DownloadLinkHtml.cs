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
            this.DocumentDefId = DomainUtility.toString(obj["documentDefId"]);
            this.ListDocument = new List<Guid>();
            if (base.Helper.FormAction == HtmlElementHelperModel.e_FormAction.Onload)
                this.ListDocument = base.Helper?.DocumentEngine?.GetList(this.DocumentDefId.ToGuidObjNull(), this.VariableId.ToGuidObjNull(), null).Select(c => c.GUID).ToList();
        }
        [DataMember]
        public Guid? ThreadID { get { return base.Helper?.DataManageHelper?.ThreadID; } private set { } }
        [DataMember]
        public string VariableId { get; set; }
        [DataMember]
        public string DocumentDefId { get; set; }
        [DataMember]
        public List<Guid> ListDocument { get; set; }
    }
}
