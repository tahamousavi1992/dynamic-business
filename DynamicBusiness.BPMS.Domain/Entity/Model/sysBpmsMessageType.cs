
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsMessageType
    {
        
        public sysBpmsMessageType()
        {
            this.Events = new HashSet<sysBpmsEvent>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
        [Required]
        public string ParamsXML { get; set; }
        public bool IsActive { get; set; }
    
        
        public virtual ICollection<sysBpmsEvent> Events { get; set; }
    }
}
