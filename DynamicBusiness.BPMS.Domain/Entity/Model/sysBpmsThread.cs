
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsThread
    {

        public sysBpmsThread()
        {
            this.Documents = new HashSet<sysBpmsDocument>();
            this.ThreadEvents = new HashSet<sysBpmsThreadEvent>();
            this.ThreadTasks = new HashSet<sysBpmsThreadTask>();
            this.ThreadVariables = new HashSet<sysBpmsThreadVariable>();
        }
        
        public System.Guid ID { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public int StatusLU { get; set; }
        public string GatewayStatusXml { get; set; }
        [ForeignKey(nameof(Process))]
        public System.Guid ProcessID { get; set; }
        [ForeignKey(nameof(User))]
        public Nullable<System.Guid> UserID { get; set; }
        [MaxLength(50)]
        public string FormattedNumber { get; set; }
        public Nullable<int> Number { get; set; }


        public virtual ICollection<sysBpmsDocument> Documents { get; set; }
        public virtual sysBpmsProcess Process { get; set; }
        public virtual sysBpmsUser User { get; set; }

        public virtual ICollection<sysBpmsThreadEvent> ThreadEvents { get; set; }
        public virtual ICollection<sysBpmsThreadTask> ThreadTasks { get; set; }
        public virtual ICollection<sysBpmsThreadVariable> ThreadVariables { get; set; }

    }
}
