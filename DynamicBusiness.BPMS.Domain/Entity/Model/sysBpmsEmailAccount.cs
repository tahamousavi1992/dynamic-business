
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    public partial class sysBpmsEmailAccount
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        public int ObjectTypeLU { get; set; }
        public Nullable<System.Guid> ObjectID { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string SMTP { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(10)]
        public string Port { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string MailUserName { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string MailPassword { get; set; }
        [Required(AllowEmptyStrings = true)]
        [MaxLength(500)]
        public string Email { get; set; }
    }
}
