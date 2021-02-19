
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsElement
    {

        public sysBpmsElement()
        {
            this.Events = new HashSet<sysBpmsEvent>();
            //this.Gateways = new HashSet<sysBpmsGateway>();
            this.Lanes = new HashSet<sysBpmsLane>();
            this.SequenceFlows = new HashSet<sysBpmsSequenceFlow>();
            this.Tasks = new HashSet<sysBpmsTask>();
        }
        [Key, Column(Order = 0)]
        [MaxLength(100)]
        public string ID { get; set; }
        [ForeignKey(nameof(Process))]
        [Key, Column(Order = 1)]
        public System.Guid ProcessID { get; set; }

        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string Name { get; set; }

        public int TypeLU { get; set; }


        public virtual sysBpmsProcess Process { get; set; }

        public virtual ICollection<sysBpmsEvent> Events { get; set; }

        public virtual ICollection<sysBpmsGateway> Gateways { get; set; }

        public virtual ICollection<sysBpmsLane> Lanes { get; set; }

        public virtual ICollection<sysBpmsSequenceFlow> SequenceFlows { get; set; }

        public virtual ICollection<sysBpmsTask> Tasks { get; set; }
    }
}
