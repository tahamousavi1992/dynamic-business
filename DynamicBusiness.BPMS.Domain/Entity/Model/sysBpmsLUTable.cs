
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsLUTable
    {
        
        public sysBpmsLUTable()
        {
            this.LURows = new HashSet<sysBpmsLURow>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string NameOf { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string Alias { get; set; }
        public bool IsSystemic { get; set; }
        public bool IsActive { get; set; }
    
        
        public virtual ICollection<sysBpmsLURow> LURows { get; set; }
    }
}
