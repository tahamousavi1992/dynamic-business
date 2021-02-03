
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class sysBpmsDocument
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public System.Guid GUID { get; set; }
        public bool IsDeleted { get; set; }
        [ForeignKey(nameof(DocumentDef))]

        public System.Guid DocumentDefID { get; set; }

        [ForeignKey(nameof(EntityDef))]
        public Nullable<System.Guid> EntityDefID { get; set; }

        public Nullable<System.Guid> EntityID { get; set; }

        [ForeignKey(nameof(Thread))]
        public Nullable<System.Guid> ThreadID { get; set; }

        public System.DateTime AtachDateOf { get; set; }
        [Required]
        [MaxLength(10)]
        public string FileExtention { get; set; }
        [Required]
        [MaxLength(1000)]
        public string CaptionOf { get; set; }
    
        public virtual sysBpmsDocumentDef DocumentDef { get; set; }
        public virtual sysBpmsEntityDef EntityDef { get; set; }
        public virtual sysBpmsThread Thread { get; set; }
    }
}
