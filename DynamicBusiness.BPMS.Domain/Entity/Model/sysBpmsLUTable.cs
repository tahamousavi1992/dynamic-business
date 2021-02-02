
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class sysBpmsLUTable
    {
        
        public sysBpmsLUTable()
        {
            this.LURows = new HashSet<sysBpmsLURow>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        public string NameOf { get; set; }
        public string Alias { get; set; }
        public bool IsSystemic { get; set; }
        public bool IsActive { get; set; }
    
        
        public virtual ICollection<sysBpmsLURow> LURows { get; set; }
    }
}
