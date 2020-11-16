using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class PostMethodMessage
    {
        public PostMethodMessage() { }
        public PostMethodMessage(string message, DisplayMessageType messageType, object data = null)
        {
            this.Message = message;
            this.ResultType = messageType.ToString();
            this.Data = data;
        }
        [DataMember]
        public string ResultType { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public object Data { get; set; }
    }
}