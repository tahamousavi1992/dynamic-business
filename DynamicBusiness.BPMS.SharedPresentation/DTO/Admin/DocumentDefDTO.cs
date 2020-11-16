using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class DocumentDefDTO
    {
        public DocumentDefDTO() { }
        public DocumentDefDTO(sysBpmsDocumentDef documentDef)
        {
            if (documentDef != null)
            {
                this.ID = documentDef.ID;
                this.DocumentFolderID = documentDef.DocumentFolderID;
                this.NameOf = documentDef.NameOf;
                this.DisplayName = documentDef.DisplayName;
                this.MaxSize = documentDef.MaxSize;
                this.ValidExtentions = documentDef.ValidExtentions;
                this.IsMandatory = documentDef.IsMandatory;
                this.Description = documentDef.Description;
                this.IsSystemic = documentDef.IsSystemic;
                this.IsActive = documentDef.IsActive;
            }
        }
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public Guid DocumentFolderID { get; set; }
        [DataMember]
        public string NameOf { get; set; }
        [DataMember]
        [Required] 
        public string DisplayName { get; set; }
        [DataMember]
        public Nullable<int> MaxSize { get; set; }
        [DataMember]
        public string ValidExtentions { get; set; }
        [DataMember]
        public bool IsMandatory { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public bool IsSystemic { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
    }
}