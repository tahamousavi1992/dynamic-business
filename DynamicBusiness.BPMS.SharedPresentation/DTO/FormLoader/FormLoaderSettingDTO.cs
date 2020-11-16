using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class SingleActionSettingDTO
    {
        [DataMember]
        public string WebApiAddress { get; set; }
        [DataMember]
        public Guid? ApplicationPageID { get; set; }
        [DataMember]
        public Guid? ProcessEndFormID { get; set; }
        [DataMember]
        public Guid? ProcessID { get; set; }
        [DataMember]
        public string WebServicePass { get; set; }
        [DataMember]
        public bool IsProcess { get { return this.ProcessID.HasValue || !this.ApplicationPageID.HasValue; } private set { } }
        [DataMember]
        public string ApplicationName { get; set; }
        [DataMember]
        public string ProcessEndFormName { get; set; }
        [DataMember]
        public string ProcessName { get; set; }
        [DataMember]
        public bool ShowCardBody { get; set; }
        public enum e_SettingType
        {
            SingleAction_WebApiAddress,
            SingleAction_ApplicationPageID,
            SingleAction_ProcessID,
            SingleAction_WebServicePass,
            SingleAction_ShowCardBody,
            SingleAction_ProcessEndFormID,
        }
    }
}