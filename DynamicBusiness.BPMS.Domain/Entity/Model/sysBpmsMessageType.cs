
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class sysBpmsMessageType
    {
        
        public sysBpmsMessageType()
        {
            this.Events = new HashSet<sysBpmsEvent>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        public string Name { get; set; }
        public string ParamsXML { get; set; }
        public bool IsActive { get; set; }
    
        
        public virtual ICollection<sysBpmsEvent> Events { get; set; }
    }
}
