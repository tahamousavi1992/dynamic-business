
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsAPIAccess
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        [Required]
        [MaxLength(500)]
        public string Name { get; set; }
        [Required]
        [MaxLength(50)]
        public string IPAddress { get; set; }
        [Required]
        [MaxLength(500)]
        public string AccessKey { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
