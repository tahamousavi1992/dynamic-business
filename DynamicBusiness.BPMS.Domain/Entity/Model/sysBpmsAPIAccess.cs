 
namespace DynamicBusiness.BPMS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class sysBpmsAPIAccess
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public System.Guid ID { get; set; }
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public string AccessKey { get; set; }
        public bool IsActive { get; set; }
    }
}
