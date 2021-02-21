
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsDocumentDef
    {
        
        public sysBpmsDocumentDef()
        {
            this.Documents = new HashSet<sysBpmsDocument>();
        }
        
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(DocumentFolder))]
        public System.Guid DocumentFolderID { get; set; }
        [Required(AllowEmptyStrings = true)]
        public string NameOf { get; set; }
        [Required]
        public string DisplayName { get; set; }
        public Nullable<int> MaxSize { get; set; }
        [Required(AllowEmptyStrings = true)]
        public string ValidExtentions { get; set; }
        public bool IsMandatory { get; set; }
        public string Description { get; set; }
        public bool IsSystemic { get; set; }
        public bool IsActive { get; set; }
    
        
        public virtual ICollection<sysBpmsDocument> Documents { get; set; }
        public virtual sysBpmsDocumentFolder DocumentFolder { get; set; }
    }
}
