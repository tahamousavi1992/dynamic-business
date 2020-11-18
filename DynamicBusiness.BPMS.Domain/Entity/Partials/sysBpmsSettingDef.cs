using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsSettingDef
    {
        public enum e_ValueType
        {
            Text = 1,
            File = 2,
            Date = 3,
        }
        public enum e_NameType
        { 
            DefaultReportFontFamily,
            ProcessFormatSerlialNumber,
            ProcessStartPointSerlialNumber,
            ThreadFormatSerlialNumber,
            ThreadStartPointSerlialNumber,
            NoSkinPath,
            WebServicePass,
            AddUserAutomatically,
        }
        public sysBpmsSettingDef(string name, string title, int valueTypeLU, string defaultValue)
        {
            this.Name = name;
            this.Title = title;
            this.ValueTypeLU = valueTypeLU;
            this.DefaultValue = defaultValue;
        }

        public void Load(sysBpmsSettingDef settingDef)
        {
            this.ID = settingDef.ID;
            this.Name = settingDef.Name;
            this.Title = settingDef.Title;
            this.ValueTypeLU = settingDef.ValueTypeLU;
            this.DefaultValue = settingDef.DefaultValue;
        }
    }
}
