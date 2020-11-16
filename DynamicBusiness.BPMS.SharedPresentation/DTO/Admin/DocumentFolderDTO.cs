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
    public class DocumentFolderDTO
    {
        public DocumentFolderDTO() { }
        public DocumentFolderDTO(sysBpmsDocumentFolder documentFolder)
        {
            if (documentFolder != null)
            {
                this.ID = documentFolder.ID;
                this.DocumentFolderID = documentFolder.DocumentFolderID;
                this.NameOf = documentFolder.NameOf;
                this.DisplayName = documentFolder.DisplayName;
                this.IsActive = documentFolder.IsActive;
            }
        }
        [DataMember]
        public Guid ID { get; set; } 
        [DataMember]
        public Nullable<Guid> DocumentFolderID { get; set; } 
        [DataMember]
        public string NameOf { get; set; } 
        [DataMember]
        [Required]
        public string DisplayName { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
    }
}