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
    public class DocumentDefinitionModel
    {
        public DocumentDefinitionModel() { }
        public DocumentDefinitionModel(sysBpmsDocumentDef documentDef)
        {
            this.ID = documentDef.ID;
            this.DisplayName = documentDef.DisplayName;
            this.MaxSize = documentDef.MaxSize;
            this.IsMandatory = documentDef.IsMandatory;
            this.ValidExtentions = documentDef.ValidExtentions;
        }
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public string DisplayName { get; set; }
        [DataMember]
        public Nullable<int> MaxSize { get; set; }
        [DataMember]
        public string ValidExtentions { get; set; }
        [DataMember]
        public bool IsMandatory { get; set; }
    }
}
