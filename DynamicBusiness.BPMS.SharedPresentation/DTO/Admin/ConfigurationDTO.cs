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
    public class ConfigurationDTO
    {
        public ConfigurationDTO() { }
        public void LoadData()
        {
            ConfigurationService configurationService = new ConfigurationService();
            this.DefaultReportFontFamily = configurationService.GetValue(sysBpmsConfiguration.e_NameType.DefaultReportFontFamily.ToString());

            this.ProcessStartPointSerlialNumber = configurationService.GetValue(sysBpmsConfiguration.e_NameType.ProcessStartPointSerlialNumber.ToString()).ToIntObj();
            this.ProcessFormatSerlialNumber = configurationService.GetValue(sysBpmsConfiguration.e_NameType.ProcessFormatSerlialNumber.ToString());
            this.ThreadStartPointSerlialNumber = configurationService.GetValue(sysBpmsConfiguration.e_NameType.ThreadStartPointSerlialNumber.ToString()).ToIntObj();
            this.ThreadFormatSerlialNumber = configurationService.GetValue(sysBpmsConfiguration.e_NameType.ThreadFormatSerlialNumber.ToString());
            this.NoSkinPath = configurationService.GetValue(sysBpmsConfiguration.e_NameType.NoSkinPath.ToString());
            this.AddUserAutomatically = configurationService.GetValue(sysBpmsConfiguration.e_NameType.AddUserAutomatically.ToString()).ToLower() == "true";
            this.ShowUserPanelWithNoSkin = configurationService.GetValue(sysBpmsConfiguration.e_NameType.ShowUserPanelWithNoSkin.ToString()).ToLower() == "true";
            this.LoadUserPanelJquery = configurationService.GetValue(sysBpmsConfiguration.e_NameType.LoadUserPanelJquery.ToString()).ToLower() == "true";
            this.LoadUserPanelBootstrap = configurationService.GetValue(sysBpmsConfiguration.e_NameType.LoadUserPanelBootstrap.ToString()).ToLower() == "true";
        }

        [DataMember]
        public string DefaultReportFontFamily { get; set; }

        [DataMember]
        public int ProcessStartPointSerlialNumber { get; set; }

        [DataMember]
        public string ProcessFormatSerlialNumber { get; set; }

        [DataMember]
        public int ThreadStartPointSerlialNumber { get; set; }

        [DataMember]
        public string ThreadFormatSerlialNumber { get; set; }

        [DataMember]
        public string NoSkinPath { get; set; }

        [DataMember]
        public bool AddUserAutomatically { get; set; }

        [DataMember]
        public bool ShowUserPanelWithNoSkin { get; set; }

        [DataMember]
        public bool LoadUserPanelJquery { get; set; }

        [DataMember]
        public bool LoadUserPanelBootstrap { get; set; }
    }
}