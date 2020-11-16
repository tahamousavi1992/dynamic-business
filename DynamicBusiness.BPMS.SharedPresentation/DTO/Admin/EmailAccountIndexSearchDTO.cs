using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    /// <summary>
    ///Controller -> EmailAccount  Action -> Index
    /// </summary>
    [KnownType(typeof(EmailAccountIndexSearchDTO))]
    [DataContract]
    public class EmailAccountIndexSearchDTO : BaseSearchDTO<EmailAccountDTO>
    {
        public EmailAccountIndexSearchDTO() : base() { }
        [DataMember]
        public string Name { get; set; }
    }
}