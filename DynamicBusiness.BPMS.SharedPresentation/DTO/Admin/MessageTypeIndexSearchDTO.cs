using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    /// <summary>
    ///Controller -> MessageType  Action -> Index
    /// </summary>
    [KnownType(typeof(MessageTypeIndexSearchDTO))]
    [DataContract]
    public class MessageTypeIndexSearchDTO : BaseSearchDTO<MessageTypeDTO>
    {
        public MessageTypeIndexSearchDTO() : base() { }
        //search region.
        [DataMember]
        public string Name { get; set; }
    }
}