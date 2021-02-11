
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsEntityDef
    {
        
        public sysBpmsEntityDef()
        {
            this.Documents = new HashSet<sysBpmsDocument>();
            this.Variables = new HashSet<sysBpmsVariable>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string DisplayName { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = true)]
        public string DesignXML { get; set; }
        public bool IsActive { get; set; }
    
        
        public virtual ICollection<sysBpmsDocument> Documents { get; set; }
        
        public virtual ICollection<sysBpmsVariable> Variables { get; set; }
    }
}
