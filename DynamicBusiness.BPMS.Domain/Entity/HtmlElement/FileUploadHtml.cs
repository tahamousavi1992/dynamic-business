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
    [KnownType(typeof(FileUploadHtml))]
    public class FileUploadHtml : BindingElementBase
    {
        public FileUploadHtml() { }
        public FileUploadHtml(JObject obj, HtmlElementHelperModel _helper, Guid dynamicFormId, bool isFormReadOnly) : base(obj, _helper, dynamicFormId, isFormReadOnly)
        {
            this.VariableId = DomainUtility.toString(obj["entityVariableId"]);
            this.DocumentdefId = DomainUtility.toString(obj["documentDefId"]);
            this.DocumentFolderId = obj["documentFolderId"].ToStringObj();
            this.Multiple = !string.IsNullOrWhiteSpace(DomainUtility.toString(obj["multiple"])) && Convert.ToBoolean(obj["multiple"]);
            this.DeleteClass = DomainUtility.toString(obj["deleteClass"]);
            this.DownloadClass = DomainUtility.toString(obj["downloadClass"]);
            this.DeleteCaption = DomainUtility.toString(obj["deleteCaption"]);
            this.DownloadCaption = DomainUtility.toString(obj["downloadCaption"]);
            this.ListDocument = new List<QueryModel>();
            this.FillValue();
        }
        [DataMember]
        public Guid? ThreadID { get { return base.Helper?.DataManageHelper?.ThreadID; } private set { } }
        [DataMember]
        public string VariableId { get; set; }
        [DataMember]
        public string DocumentdefId { get; set; }
        [DataMember]
        public string DocumentFolderId { get; set; }
        [DataMember]
        public bool Multiple { get; set; }
        [DataMember]
        public string DeleteClass { get; set; }
        [DataMember]
        public string DownloadClass { get; set; }
        [DataMember]
        public string DeleteCaption { get; set; }
        [DataMember]
        public string DownloadCaption { get; set; }
        [DataMember]
        public List<QueryModel> ListDocument { get; set; }
        [DataMember]
        public List<DocumentDefinitionModel> ListDocumentDef { get; set; }
        private void FillValue()
        {
            if (base.Helper.FormAction == HtmlElementHelperModel.e_FormAction.Onload)
            {
                if (!string.IsNullOrWhiteSpace(this.DocumentFolderId))
                {
                    this.ListDocument = base.Helper?.DocumentEngine?.GetList(null, this.VariableId.ToGuidObjNull(), this.DocumentFolderId.ToGuidObjNull()).Select(c => new QueryModel(c.GUID.ToString(), c.DocumentDefID)).ToList();
                    this.ListDocumentDef = base.Helper?.UnitOfWork?.Repository<IDocumentDefRepository>()?.GetList(this.DocumentFolderId.ToGuidObj(), "", "", true, null, null)?.Select(c => new DocumentDefinitionModel(c)).ToList();
                }
                else
                    this.ListDocument = base.Helper?.DocumentEngine?.GetList(this.DocumentdefId.ToGuidObjNull(), this.VariableId.ToGuidObjNull(), this.DocumentFolderId.ToGuidObjNull()).Select(c => new QueryModel(c.GUID.ToString(), c.DocumentDefID)).ToList();
            }
            else
            if (this.Helper?.FormAction == HtmlElementHelperModel.e_FormAction.OnPost)
            {
                if (!string.IsNullOrWhiteSpace(this.DocumentFolderId))
                {
                    this.Value = this.Helper.ListFormQueryModel.Where(c => c.Key.Contains(this.Id + "_")).ToDictionary(c => c.Key.Split('_').LastOrDefault().Replace("[]", "").ToGuidObj(), c => c.Value.ToStringObj().Contains("undefined") ? null : (object)c.Value);
                }
                else
                    this.Value = this.Helper.ListFormQueryModel.Where(c => c.Key == this.Id + "[]" && !c.Value.ToStringObj().Contains("undefined")).ToDictionary(c => this.DocumentdefId.ToGuidObj(), c => (object)c.Value);
            }
        }

        public override void SetValue(object value)
        {
            this.Value = value;
        }
    }

}
