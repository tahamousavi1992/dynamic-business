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
    public class SettingDTO
    {
        public SettingDTO() { }
        public void LoadData()
        {
            SettingValueService settingValueService = new SettingValueService();
            this.DefaultReportFontFamily = settingValueService.GetValue(sysBpmsSettingDef.e_NameType.DefaultReportFontFamily.ToString());

            this.ProcessStartPointSerlialNumber = settingValueService.GetValue(sysBpmsSettingDef.e_NameType.ProcessStartPointSerlialNumber.ToString()).ToIntObj();
            this.ProcessFormatSerlialNumber = settingValueService.GetValue(sysBpmsSettingDef.e_NameType.ProcessFormatSerlialNumber.ToString());
            this.ThreadStartPointSerlialNumber = settingValueService.GetValue(sysBpmsSettingDef.e_NameType.ThreadStartPointSerlialNumber.ToString()).ToIntObj();
            this.ThreadFormatSerlialNumber = settingValueService.GetValue(sysBpmsSettingDef.e_NameType.ThreadFormatSerlialNumber.ToString());
            this.NoSkinPath = settingValueService.GetValue(sysBpmsSettingDef.e_NameType.NoSkinPath.ToString());
            this.NoContainerPath = settingValueService.GetValue(sysBpmsSettingDef.e_NameType.NoContainerPath.ToString());
            this.AddUserAutomatically = settingValueService.GetValue(sysBpmsSettingDef.e_NameType.AddUserAutomatically.ToString()).ToLower() == "true";
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
        public string NoContainerPath { get; set; }

        [DataMember]
        public bool AddUserAutomatically { get; set; }

    }
}