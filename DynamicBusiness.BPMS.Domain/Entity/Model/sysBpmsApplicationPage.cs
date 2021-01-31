 
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class sysBpmsApplicationPage
    {
        public sysBpmsApplicationPage()
        {
            this.sysBpmsApplicationPageAccesses = new HashSet<sysBpmsApplicationPageAccess>();
            this.DynamicForms = new HashSet<sysBpmsDynamicForm>();
            this.Variables = new HashSet<sysBpmsVariable>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        public int GroupLU { get; set; }
        public string Description { get; set; }
        public bool ShowInMenu { get; set; }
    
        
        public virtual ICollection<sysBpmsApplicationPageAccess> sysBpmsApplicationPageAccesses { get; set; }
        
        public virtual ICollection<sysBpmsDynamicForm> DynamicForms { get; set; }
        
        public virtual ICollection<sysBpmsVariable> Variables { get; set; }
    }
}
