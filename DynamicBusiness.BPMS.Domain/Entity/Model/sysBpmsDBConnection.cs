 
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsDBConnection
    {
        
        public sysBpmsDBConnection()
        {
            this.Variables = new HashSet<sysBpmsVariable>();
        }
        
        public System.Guid ID { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string DataSource { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string InitialCatalog { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string UserID { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string Password { get; set; }
        public bool IntegratedSecurity { get; set; }
    
        
        public virtual ICollection<sysBpmsVariable> Variables { get; set; }
    }
}
