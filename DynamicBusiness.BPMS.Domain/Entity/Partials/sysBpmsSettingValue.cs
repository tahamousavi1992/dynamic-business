using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsSettingValue
    {
        public sysBpmsSettingValue()
        {
        }

        public sysBpmsSettingValue(Guid settingDefID, DateTime setDate, string userName, string value)
        {
            this.SettingDefID = settingDefID;
            this.SetDate = setDate;
            this.UserName = userName;
            this.Value = value;
        }

        public void Load(sysBpmsSettingValue settingValue)
        {
            this.ID = settingValue.ID;
            this.SettingDefID = settingValue.SettingDefID;
            this.SetDate = settingValue.SetDate;
            this.UserName = settingValue.UserName;
            this.Value = settingValue.Value;
        }
    }
}
