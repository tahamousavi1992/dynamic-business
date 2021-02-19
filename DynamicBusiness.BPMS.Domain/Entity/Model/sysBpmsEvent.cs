
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsEvent
    {
        
        public sysBpmsEvent()
        {
            this.ThreadEvents = new HashSet<sysBpmsThreadEvent>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(100)]
        public string ElementID { get; set; }
        public int TypeLU { get; set; }
        public string ConfigurationXML { get; set; }
        public Nullable<int> SubType { get; set; }
        [MaxLength(100)]
        public string RefElementID { get; set; }
        public Nullable<bool> CancelActivity { get; set; }
        [ForeignKey(nameof(Process))]
        public System.Guid ProcessID { get; set; }
        [ForeignKey(nameof(MessageType))]
        public Nullable<System.Guid> MessageTypeID { get; set; }
    
        public virtual sysBpmsElement Element { get; set; }
        public virtual sysBpmsMessageType MessageType { get; set; }
        public virtual sysBpmsProcess Process { get; set; }
        
        public virtual ICollection<sysBpmsThreadEvent> ThreadEvents { get; set; }
    }
}
