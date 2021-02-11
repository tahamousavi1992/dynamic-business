
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsDocumentFolder
    {
        
        public sysBpmsDocumentFolder()
        {
            this.DocumentDefs = new HashSet<sysBpmsDocumentDef>();
            this.ChildrenFolders = new HashSet<sysBpmsDocumentFolder>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string NameOf { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string DisplayName { get; set; }
        [ForeignKey(nameof(ParentFolder))]
        public Nullable<System.Guid> DocumentFolderID { get; set; }
        public bool IsActive { get; set; }
 
        public virtual ICollection<sysBpmsDocumentDef> DocumentDefs { get; set; }
        
        public virtual ICollection<sysBpmsDocumentFolder> ChildrenFolders { get; set; }
        public virtual sysBpmsDocumentFolder ParentFolder { get; set; }
    }
}
