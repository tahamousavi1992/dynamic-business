using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class MessageTypeDTO
    {
        public MessageTypeDTO() { }
        public MessageTypeDTO(sysBpmsMessageType sysBpmsMessageType)
        {
            if (sysBpmsMessageType != null)
            {
                this.ID = sysBpmsMessageType.ID;
                this.Name = sysBpmsMessageType.Name;
                this.ParamsXML = sysBpmsMessageType.ParamsXML;
                this.IsActive = sysBpmsMessageType.IsActive;
            }
        }
        [DataMember]
        public System.Guid ID { get; set; }
        [Required] 
        [DataMember]
        public string Name { get; set; }
         
        [DataMember]
        public string ParamsXML { get; set; }
         
        [DataMember]
        public bool IsActive { get; set; }
         
        [DataMember]
        public List<MessageTypeParamsModel> ListParameter
        {
            get => this.ParamsXML?.ParseXML<List<MessageTypeParamsModel>>() ?? new List<MessageTypeParamsModel>();
            set => this.ParamsXML = value.BuildXml() ?? "";
        }
    }
}