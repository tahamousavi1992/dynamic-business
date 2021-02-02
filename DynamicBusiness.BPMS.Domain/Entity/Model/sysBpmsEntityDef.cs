
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class sysBpmsEntityDef
    {
        
        public sysBpmsEntityDef()
        {
            this.Documents = new HashSet<sysBpmsDocument>();
            this.Variables = new HashSet<sysBpmsVariable>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string DesignXML { get; set; }
        public bool IsActive { get; set; }
    
        
        public virtual ICollection<sysBpmsDocument> Documents { get; set; }
        
        public virtual ICollection<sysBpmsVariable> Variables { get; set; }
    }
}
