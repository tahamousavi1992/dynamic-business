 
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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
        [Required]
        [MaxLength(500)]
        public string DataSource { get; set; }
        [Required]
        [MaxLength(500)]
        public string InitialCatalog { get; set; }
        [Required]
        [MaxLength(500)]
        public string UserID { get; set; }
        [Required]
        [MaxLength(500)]
        public string Password { get; set; }
        public bool IntegratedSecurity { get; set; }
    
        
        public virtual ICollection<sysBpmsVariable> Variables { get; set; }
    }
}