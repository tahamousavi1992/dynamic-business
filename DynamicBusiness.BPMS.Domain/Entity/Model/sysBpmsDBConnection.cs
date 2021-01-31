 
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class sysBpmsDBConnection
    {
        
        public sysBpmsDBConnection()
        {
            this.Variables = new HashSet<sysBpmsVariable>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        public string Name { get; set; }
        public string DataSource { get; set; }
        public string InitialCatalog { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public bool IntegratedSecurity { get; set; }
    
        
        public virtual ICollection<sysBpmsVariable> Variables { get; set; }
    }
}
