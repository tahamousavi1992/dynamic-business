
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class sysBpmsLURow
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(LUTable))]
        public System.Guid LUTableID { get; set; }
        public string NameOf { get; set; }
        public string CodeOf { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsSystemic { get; set; }
        public bool IsActive { get; set; }

        public virtual sysBpmsLUTable LUTable { get; set; }
    }
}
