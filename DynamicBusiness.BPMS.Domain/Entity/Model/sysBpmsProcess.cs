
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsProcess
    {
        
        public sysBpmsProcess()
        {
            this.DynamicForms = new HashSet<sysBpmsDynamicForm>();
            this.Elements = new HashSet<sysBpmsElement>();
            this.Events = new HashSet<sysBpmsEvent>();
            this.Gateways = new HashSet<sysBpmsGateway>();
            this.Lanes = new HashSet<sysBpmsLane>();
            this.ChildrenProcess = new HashSet<sysBpmsProcess>();
            this.SequenceFlows = new HashSet<sysBpmsSequenceFlow>();
            this.Tasks = new HashSet<sysBpmsTask>();
            this.Threads = new HashSet<sysBpmsThread>();
            this.Variables = new HashSet<sysBpmsVariable>();
        }
        
        public System.Guid ID { get; set; }
        public string FormattedNumber { get; set; }
        public Nullable<int> Number { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<int> ProcessVersion { get; set; }
        public Nullable<int> StatusLU { get; set; }
        [MaxLength(500)]
        public string CreatorUsername { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string DiagramXML { get; set; }
        public string WorkflowXML { get; set; }
        public string BeginTasks { get; set; }
        [ForeignKey(nameof(ParentProcess))]
        public Nullable<System.Guid> ParentProcessID { get; set; }
        public Nullable<System.DateTime> PublishDate { get; set; }
        public Nullable<int> ParallelCountPerUser { get; set; }
        public string SourceCode { get; set; }
        [ForeignKey(nameof(ProcessGroup))]
        public System.Guid ProcessGroupID { get; set; }
        public int TypeLU { get; set; }
    
        
        public virtual ICollection<sysBpmsDynamicForm> DynamicForms { get; set; }
        
        public virtual ICollection<sysBpmsElement> Elements { get; set; }
        
        public virtual ICollection<sysBpmsEvent> Events { get; set; }
        
        public virtual ICollection<sysBpmsGateway> Gateways { get; set; }
        
        public virtual ICollection<sysBpmsLane> Lanes { get; set; }
        
        public virtual ICollection<sysBpmsProcess> ChildrenProcess { get; set; }
        public virtual sysBpmsProcess ParentProcess { get; set; }
        public virtual sysBpmsProcessGroup ProcessGroup { get; set; }
        
        public virtual ICollection<sysBpmsSequenceFlow> SequenceFlows { get; set; }
        
        public virtual ICollection<sysBpmsTask> Tasks { get; set; }
        
        public virtual ICollection<sysBpmsThread> Threads { get; set; }
        
        public virtual ICollection<sysBpmsVariable> Variables { get; set; }
    }
}
