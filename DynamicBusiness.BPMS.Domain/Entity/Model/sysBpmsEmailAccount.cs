
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class sysBpmsEmailAccount
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        public int ObjectTypeLU { get; set; }
        public Nullable<System.Guid> ObjectID { get; set; }
        public string SMTP { get; set; }
        public string Port { get; set; }
        public string MailUserName { get; set; }
        public string MailPassword { get; set; }
        public string Email { get; set; }
    }
}
