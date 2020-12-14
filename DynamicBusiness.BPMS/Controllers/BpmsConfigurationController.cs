using DynamicBusiness.BPMS.BusinessLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;

namespace DynamicBusiness.BPMS.Controllers
{
    public class BpmsConfigurationController : BpmsAdminApiControlBase
    {
        [HttpGet]
        public object GetAddEdit()
        {
            //base.SetMenuIndex(AdminMenuIndex.SettingIndex);
            ConfigurationDTO settingDTO = new ConfigurationDTO();
            settingDTO.LoadData();
            return settingDTO;
        }

        [HttpPost]
        public object PostAddEdit(ConfigurationDTO settingDTO)
        {
            ResultOperation resultOperation = new ResultOperation();
            using (ConfigurationService configurationService = new ConfigurationService())
            {

                List<sysBpmsConfiguration> listValues = configurationService.GetList("", "");

                //DefaultReportFontFamily
                sysBpmsConfiguration config = listValues.FirstOrDefault(c => c.Name == sysBpmsConfiguration.e_NameType.DefaultReportFontFamily.ToString());
                config = this.FillObject(config, settingDTO.DefaultReportFontFamily);
                resultOperation = configurationService.Update(config);

                //ProcessFormatSerlialNumber
                config = listValues.FirstOrDefault(c => c.Name == sysBpmsConfiguration.e_NameType.ProcessFormatSerlialNumber.ToString());
                config = this.FillObject(config, settingDTO.ProcessFormatSerlialNumber);
                resultOperation = configurationService.Update(config);

                //ProcessStartPointSerlialNumber
                config = listValues.FirstOrDefault(c => c.Name == sysBpmsConfiguration.e_NameType.ProcessStartPointSerlialNumber.ToString());
                config = this.FillObject(config, settingDTO.ProcessStartPointSerlialNumber.ToStringObj());

                resultOperation = configurationService.Update(config);

                //ThreadFormatSerlialNumber
                config = listValues.FirstOrDefault(c => c.Name == sysBpmsConfiguration.e_NameType.ThreadFormatSerlialNumber.ToString());
                config = this.FillObject(config, settingDTO.ThreadFormatSerlialNumber);
                resultOperation = configurationService.Update(config);

                //ThreadStartPointSerlialNumber
                config = listValues.FirstOrDefault(c => c.Name == sysBpmsConfiguration.e_NameType.ThreadStartPointSerlialNumber.ToString());
                config = this.FillObject(config, settingDTO.ThreadStartPointSerlialNumber.ToStringObj());
                resultOperation = configurationService.Update(config);


                //AddUserAutomatically
                config = listValues.FirstOrDefault(c => c.Name == sysBpmsConfiguration.e_NameType.AddUserAutomatically.ToString());
                config = this.FillObject(config, settingDTO.AddUserAutomatically.ToStringObj());
                resultOperation = configurationService.Update(config);

                //ShowUserPanelWithNoSkin
                bool showNoSkin = settingDTO.ShowUserPanelWithNoSkin.ToStringObj().ToLower() == "true";
                config = listValues.FirstOrDefault(c => c.Name == sysBpmsConfiguration.e_NameType.ShowUserPanelWithNoSkin.ToString());
                config = this.FillObject(config, settingDTO.ShowUserPanelWithNoSkin.ToStringObj());
                resultOperation = configurationService.Update(config);

                //LoadUserPanelJquery
                config = listValues.FirstOrDefault(c => c.Name == sysBpmsConfiguration.e_NameType.LoadUserPanelJquery.ToString());
                config = this.FillObject(config, (showNoSkin ? "true" : settingDTO.LoadUserPanelJquery.ToStringObj()));
                resultOperation = configurationService.Update(config);

                //LoadUserPanelBootstrap
                config = listValues.FirstOrDefault(c => c.Name == sysBpmsConfiguration.e_NameType.LoadUserPanelBootstrap.ToString());
                config = this.FillObject(config, (showNoSkin ? "true" : settingDTO.LoadUserPanelBootstrap.ToStringObj()));
                resultOperation = configurationService.Update(config);

                TimerThreadEventScheduler.CheckScheduler();
                TimerStartEventScheduler.CheckScheduler();

            }

            if (resultOperation.IsSuccess)
                return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
            else
                return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);

        }

        private sysBpmsConfiguration FillObject(sysBpmsConfiguration settingValue, string value)
        {
            if (settingValue == null)
            {
                settingValue = new sysBpmsConfiguration();
                settingValue.LastUpdateOn = DateTime.Now;
            }
            else
            {
                if (settingValue.Value != value)
                {
                    settingValue.LastUpdateOn = DateTime.Now;
                }
            }
            settingValue.Value = value;

            return settingValue;
        }

    }
}