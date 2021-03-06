 
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsApplicationPage
    {
        public sysBpmsApplicationPage()
        {
            this.sysBpmsApplicationPageAccesses = new HashSet<sysBpmsApplicationPageAccess>();
            this.DynamicForms = new HashSet<sysBpmsDynamicForm>();
            this.Variables = new HashSet<sysBpmsVariable>();
        }
        
        public System.Guid ID { get; set; }
        public int GroupLU { get; set; }
        public string Description { get; set; }
        public bool ShowInMenu { get; set; }
    
        
        public virtual ICollection<sysBpmsApplicationPageAccess> sysBpmsApplicationPageAccesses { get; set; }
        
        public virtual ICollection<sysBpmsDynamicForm> DynamicForms { get; set; }
        
        public virtual ICollection<sysBpmsVariable> Variables { get; set; }
    }
}
