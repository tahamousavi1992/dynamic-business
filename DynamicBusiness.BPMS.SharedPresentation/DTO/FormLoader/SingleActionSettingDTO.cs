using DotNetNuke.Collections;
using DotNetNuke.Entities.Portals;
using DynamicBusiness.BPMS.BusinessLogic;
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
        public SingleActionSettingDTO()
        {

        }
        /// <summary>
        /// It only fills WebApiAddress and WebServicePass properties.
        /// </summary>
        /// <param name="portalId"></param>
        public SingleActionSettingDTO(HttpRequestBase request, int portalId)
        {
            this.WebApiAddress = PortalController.GetPortalSetting(SingleActionSettingDTO.e_SettingType.SingleAction_WebApiAddress.ToString(), portalId, string.Empty);
            this.WebServicePass = PortalController.GetPortalSetting(SingleActionSettingDTO.e_SettingType.SingleAction_WebServicePass.ToString(), portalId, string.Empty);
            if (string.IsNullOrWhiteSpace(this.WebApiAddress))
            {
                this.WebApiAddress = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/');
            }
        }
        /// <summary>
        /// It fills all properties.
        /// </summary>
        public SingleActionSettingDTO(HttpRequestBase request, int portalId, System.Collections.Hashtable moduleSettings)
        {
            this.WebApiAddress = PortalController.GetPortalSetting(SingleActionSettingDTO.e_SettingType.SingleAction_WebApiAddress.ToString(), portalId, string.Empty);
            this.WebServicePass = PortalController.GetPortalSetting(SingleActionSettingDTO.e_SettingType.SingleAction_WebServicePass.ToString(), portalId, string.Empty);
            this.ApplicationPageID = moduleSettings.GetValueOrDefault(SingleActionSettingDTO.e_SettingType.SingleAction_ApplicationPageID.ToString(), string.Empty).ToGuidObjNull();
            this.AppPageSubmitMessage = moduleSettings.GetValueOrDefault(SingleActionSettingDTO.e_SettingType.SingleAction_AppPageSubmitMessage.ToString(), string.Empty);
            this.ProcessID = moduleSettings.GetValueOrDefault(SingleActionSettingDTO.e_SettingType.SingleAction_ProcessID.ToString(), string.Empty).ToGuidObjNull();
            this.ShowCardBody = moduleSettings.GetValueOrDefault(SingleActionSettingDTO.e_SettingType.SingleAction_ShowCardBody.ToString(), string.Empty).ToStringObj().ToLower() == "true";
            this.ProcessEndFormID = moduleSettings.GetValueOrDefault(SingleActionSettingDTO.e_SettingType.SingleAction_ProcessEndFormID.ToString(), string.Empty).ToGuidObjNull();
            this.LoadjQuery = moduleSettings.GetValueOrDefault(SingleActionSettingDTO.e_SettingType.SingleAction_Jquery.ToString(), string.Empty).ToStringObj().ToLower() == "true"; ;
            this.LoadBootstrap = moduleSettings.GetValueOrDefault(SingleActionSettingDTO.e_SettingType.SingleAction_Bootstrap.ToString(), string.Empty).ToStringObj().ToLower() == "true";
            if (string.IsNullOrWhiteSpace(this.WebApiAddress) && request != null)
            {
                this.WebApiAddress = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/');
            }
        }
        [DataMember]
        public string WebApiAddress { get; set; }
        [DataMember]
        public Guid? ApplicationPageID { get; set; }
        /// <summary>
        /// This message is shown after submitting a form.
        /// </summary>
        [DataMember]
        public string AppPageSubmitMessage { get; set; }
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
        [DataMember]
        public bool LoadjQuery { get; set; }
        [DataMember]
        public bool LoadBootstrap { get; set; }
        public enum e_SettingType
        {
            SingleAction_WebApiAddress,
            SingleAction_ApplicationPageID,
            SingleAction_ProcessID,
            SingleAction_WebServicePass,
            SingleAction_ShowCardBody,
            SingleAction_ProcessEndFormID,
            SingleAction_Jquery,
            SingleAction_Bootstrap,
            SingleAction_AppPageSubmitMessage
        }
       
    }
}