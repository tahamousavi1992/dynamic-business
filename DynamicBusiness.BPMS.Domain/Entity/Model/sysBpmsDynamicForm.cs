
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsDynamicForm
    {
        
        public sysBpmsDynamicForm()
        {
            this.Steps = new HashSet<sysBpmsStep>();
        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(Process))]
        public Nullable<System.Guid> ProcessId { get; set; }
        [ForeignKey(nameof(ApplicationPage))]
        public Nullable<System.Guid> ApplicationPageID { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string Name { get; set; }
        public string DesignJson { get; set; }
        public string OnExitFormCode { get; set; }
        public string OnEntryFormCode { get; set; }
        public Nullable<int> Version { get; set; }
        public string ConfigXML { get; set; }
        public Nullable<bool> ShowInOverview { get; set; }
        public string SourceCode { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string UpdatedBy { get; set; }
        public System.DateTime UpdatedDate { get; set; }
    
        public virtual sysBpmsApplicationPage ApplicationPage { get; set; }
        public virtual sysBpmsProcess Process { get; set; }
        
        public virtual ICollection<sysBpmsStep> Steps { get; set; }
    }
}
