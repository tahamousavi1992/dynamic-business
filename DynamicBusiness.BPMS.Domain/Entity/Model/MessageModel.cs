using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [DataContract]
    /// <summary>
    /// it is generally used for codebase.cs message 
    /// </summary>
    public class MessageModel
    {
        public MessageModel() { }
        public MessageModel(DisplayMessageType displayMessageType, string message)
        {
            this.DisplayMessageType = displayMessageType;
            this.Message = message;
        }
        [DataMember]
        public DisplayMessageType DisplayMessageType { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
}
