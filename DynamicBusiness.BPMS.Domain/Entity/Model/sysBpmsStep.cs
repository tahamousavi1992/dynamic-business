
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsStep
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [ForeignKey(nameof(Task))]
        public System.Guid TaskID { get; set; }
        public int Position { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string Name { get; set; }
        [ForeignKey(nameof(DynamicForm))]
        public Nullable<System.Guid> DynamicFormID { get; set; }
    
        public virtual sysBpmsDynamicForm DynamicForm { get; set; }
        public virtual sysBpmsTask Task { get; set; }
    }
}
