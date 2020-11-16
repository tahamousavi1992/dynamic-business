using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class PostSubTypeEmailDTO
    {
        public PostSubTypeEmailDTO() { }
        public PostSubTypeEmailDTO(sysBpmsEvent _event)
        {
            if (_event != null)
            {
                this.ID = _event.ID;
                this.ElementID = _event.ElementID;
                this.ProcessID = _event.ProcessID;
                this.TypeLU = _event.TypeLU;
                this.ConfigurationXML = _event.ConfigurationXML;
                this.SubType = _event.SubType;
                this.CancelActivity = _event.CancelActivity;
                this.RefElementID = _event.RefElementID;
                this.MessageTypeID = _event.MessageTypeID;
            }
        }
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public Guid ProcessID { get; set; }
        [DataMember]
        public string ElementID { get; set; }
        [DataMember]
        public int TypeLU { get; set; }
        [DataMember]
        public string ConfigurationXML { get; set; }
        [DataMember]
        public int? SubType { get; set; }
        [DataMember]
        public Nullable<bool> CancelActivity { get; set; }
        [DataMember]
        public string RefElementID { get; set; }
        [DataMember] 
        public Guid? MessageTypeID { get; set; }
        [DataMember]
        private SubTypeMessageEventModel subTypeMessageEventModel { get; set; }
        [DataMember]
        public SubTypeMessageEventModel SubTypeMessageEventModel
        {
            get
            {
                if (this.subTypeMessageEventModel == null)
                {
                    if (!string.IsNullOrWhiteSpace(this.ConfigurationXML))
                        this.subTypeMessageEventModel = this.ConfigurationXML.ParseXML<SubTypeMessageEventModel>();
                    if (this.subTypeMessageEventModel == null)
                        this.subTypeMessageEventModel = new SubTypeMessageEventModel()
                        {
                            Email = new SubTypeEmailEventModel() { ToType = (int)SubTypeEmailEventModel.e_ToType.Static },
                            MessageParams = new List<SubTypeMessageParamEventModel>(),
                            Type = (int)SubTypeMessageEventModel.e_Type.Message,
                            KeyType = (int)SubTypeMessageEventModel.e_KeyType.Static,
                        };
                }
                subTypeMessageEventModel.Email = subTypeMessageEventModel.Email ?? new SubTypeEmailEventModel() { ToType = (int)SubTypeEmailEventModel.e_ToType.Static };
                subTypeMessageEventModel.MessageParams = subTypeMessageEventModel.MessageParams ?? new List<SubTypeMessageParamEventModel>();
                return subTypeMessageEventModel;
            }
            set
            {
                subTypeMessageEventModel = value;
            }
        }
        [DataMember]
        private SubTypeTimerEventModel subTypeTimerEventModel { get; set; }
        [DataMember]
        public SubTypeTimerEventModel SubTypeTimerEventModel
        {
            get
            {
                if (this.subTypeTimerEventModel == null)
                    this.subTypeTimerEventModel = this.ConfigurationXML.ParseXML<SubTypeTimerEventModel>() ?? new SubTypeTimerEventModel();
                return subTypeTimerEventModel;
            }
            set
            {
                subTypeTimerEventModel = value;
            }
        }
        [DataMember]
        public string ToStatic { get; set; }
        [DataMember]
        public string ToVariable { get; set; }
        [DataMember]
        public string ToSystemic { get; set; }
    }

}