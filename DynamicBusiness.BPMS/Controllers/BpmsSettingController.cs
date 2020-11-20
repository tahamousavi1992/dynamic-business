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
    public class BpmsSettingController : BpmsAdminApiControlBase
    {
        [HttpGet]
        public object GetAddEdit()
        {
            //base.SetMenuIndex(AdminMenuIndex.SettingIndex);
            SettingDTO settingDTO = new SettingDTO();
            settingDTO.LoadData();
            return settingDTO;
        }

        [HttpPost]
        public object PostAddEdit(SettingDTO settingDTO)
        {
            ResultOperation resultOperation = new ResultOperation();
            using (SettingValueService settingValueService = new SettingValueService())
            {
                using (SettingDefService settingDefService = new SettingDefService())
                {
                    List<sysBpmsSettingDef> listDef = settingDefService.GetList(null, "", null);
                    List<sysBpmsSettingValue> listValues = settingValueService.GetList(null, "", "");

                    //DefaultReportFontFamily
                    sysBpmsSettingDef setDef = listDef.FirstOrDefault(c => c.Name == sysBpmsSettingDef.e_NameType.DefaultReportFontFamily.ToString());
                    sysBpmsSettingValue setValue = listValues.FirstOrDefault(c => c.SettingDefID == setDef.ID);
                    setValue = this.FillObject(setValue, setDef.ID, settingDTO.DefaultReportFontFamily);
                    if (setValue.ID == Guid.Empty)
                        resultOperation = settingValueService.Add(setValue);
                    else
                        resultOperation = settingValueService.Update(setValue);

                    //ProcessFormatSerlialNumber
                    setDef = listDef.FirstOrDefault(c => c.Name == sysBpmsSettingDef.e_NameType.ProcessFormatSerlialNumber.ToString());
                    setValue = listValues.FirstOrDefault(c => c.SettingDefID == setDef.ID);
                    setValue = this.FillObject(setValue, setDef.ID, settingDTO.ProcessFormatSerlialNumber);
                    if (setValue.ID == Guid.Empty)
                        resultOperation = settingValueService.Add(setValue);
                    else
                        resultOperation = settingValueService.Update(setValue);

                    //ProcessStartPointSerlialNumber
                    setDef = listDef.FirstOrDefault(c => c.Name == sysBpmsSettingDef.e_NameType.ProcessStartPointSerlialNumber.ToString());
                    setValue = listValues.FirstOrDefault(c => c.SettingDefID == setDef.ID);
                    setValue = this.FillObject(setValue, setDef.ID, settingDTO.ProcessStartPointSerlialNumber.ToStringObj());
                    if (setValue.ID == Guid.Empty)
                        resultOperation = settingValueService.Add(setValue);
                    else
                        resultOperation = settingValueService.Update(setValue);

                    //ThreadFormatSerlialNumber
                    setDef = listDef.FirstOrDefault(c => c.Name == sysBpmsSettingDef.e_NameType.ThreadFormatSerlialNumber.ToString());
                    setValue = listValues.FirstOrDefault(c => c.SettingDefID == setDef.ID);
                    setValue = this.FillObject(setValue, setDef.ID, settingDTO.ThreadFormatSerlialNumber);
                    if (setValue.ID == Guid.Empty)
                        resultOperation = settingValueService.Add(setValue);
                    else
                        resultOperation = settingValueService.Update(setValue);

                    //ThreadStartPointSerlialNumber
                    setDef = listDef.FirstOrDefault(c => c.Name == sysBpmsSettingDef.e_NameType.ThreadStartPointSerlialNumber.ToString());
                    setValue = listValues.FirstOrDefault(c => c.SettingDefID == setDef.ID);
                    setValue = this.FillObject(setValue, setDef.ID, settingDTO.ThreadStartPointSerlialNumber.ToStringObj());
                    if (setValue.ID == Guid.Empty)
                        resultOperation = settingValueService.Add(setValue);
                    else
                        resultOperation = settingValueService.Update(setValue);


                    //AddUserAutomatically
                    setDef = listDef.FirstOrDefault(c => c.Name == sysBpmsSettingDef.e_NameType.AddUserAutomatically.ToString());
                    setValue = listValues.FirstOrDefault(c => c.SettingDefID == setDef.ID);
                    setValue = this.FillObject(setValue, setDef.ID, settingDTO.AddUserAutomatically.ToStringObj());
                    if (setValue.ID == Guid.Empty)
                        resultOperation = settingValueService.Add(setValue);
                    else
                        resultOperation = settingValueService.Update(setValue);

                    //ShowUserPanelWithNoSkin
                    bool showNoSkin = settingDTO.ShowUserPanelWithNoSkin.ToStringObj().ToLower() == "true";
                    setDef = listDef.FirstOrDefault(c => c.Name == sysBpmsSettingDef.e_NameType.ShowUserPanelWithNoSkin.ToString());
                    setValue = listValues.FirstOrDefault(c => c.SettingDefID == setDef.ID);
                    setValue = this.FillObject(setValue, setDef.ID, settingDTO.ShowUserPanelWithNoSkin.ToStringObj());
                    if (setValue.ID == Guid.Empty)
                        resultOperation = settingValueService.Add(setValue);
                    else
                        resultOperation = settingValueService.Update(setValue);

                    //LoadUserPanelJquery
                    setDef = listDef.FirstOrDefault(c => c.Name == sysBpmsSettingDef.e_NameType.LoadUserPanelJquery.ToString());
                    setValue = listValues.FirstOrDefault(c => c.SettingDefID == setDef.ID);
                    setValue = this.FillObject(setValue, setDef.ID, (showNoSkin ? "true" : settingDTO.LoadUserPanelJquery.ToStringObj()));
                    if (setValue.ID == Guid.Empty)
                        resultOperation = settingValueService.Add(setValue);
                    else
                        resultOperation = settingValueService.Update(setValue);

                    //LoadUserPanelBootstrap
                    setDef = listDef.FirstOrDefault(c => c.Name == sysBpmsSettingDef.e_NameType.LoadUserPanelBootstrap.ToString());
                    setValue = listValues.FirstOrDefault(c => c.SettingDefID == setDef.ID);
                    setValue = this.FillObject(setValue, setDef.ID, (showNoSkin ? "true" : settingDTO.LoadUserPanelBootstrap.ToStringObj()));
                    if (setValue.ID == Guid.Empty)
                        resultOperation = settingValueService.Add(setValue);
                    else
                        resultOperation = settingValueService.Update(setValue);

                    TimerThreadEventScheduler.CheckScheduler();
                    TimerStartEventScheduler.CheckScheduler();

                }
            }

            if (resultOperation.IsSuccess)
                return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
            else
                return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);

        }

        public sysBpmsSettingValue FillObject(sysBpmsSettingValue settingValue, Guid settingDefID, string value)
        {
            if (settingValue == null)
            {
                settingValue = new sysBpmsSettingValue();
                settingValue.SettingDefID = settingDefID;
                settingValue.SetDate = DateTime.Now;
                settingValue.UserName = base.UserInfo?.Username.ToStringObj() ?? "";
            }
            else
            {
                if (settingValue.Value != value)
                {
                    settingValue.SetDate = DateTime.Now;
                    settingValue.UserName = base.UserInfo?.Username.ToStringObj() ?? "";
                }
            }
            settingValue.Value = value;

            return settingValue;
        }

    }
}