
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsLURow
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(LUTable))]
        public System.Guid LUTableID { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string NameOf { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string CodeOf { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsSystemic { get; set; }
        public bool IsActive { get; set; }

        public virtual sysBpmsLUTable LUTable { get; set; }
    }
}
