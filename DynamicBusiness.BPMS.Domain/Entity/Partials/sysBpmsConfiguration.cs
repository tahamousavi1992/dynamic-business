using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsConfiguration
    {
        public sysBpmsConfiguration() { }
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
            ShowUserPanelWithNoSkin,
            LoadUserPanelJquery,
            LoadUserPanelBootstrap,
            LastSqlUpdatedVersion,
        }
        public sysBpmsConfiguration(string name, string label, string value, DateTime lastUpdateOn)
        {
            this.Name = name;
            this.Label = label;
            this.Value = value;
            this.LastUpdateOn = lastUpdateOn;
        } 

        public void Load(sysBpmsConfiguration sysBpmsConfiguration)
        {
            this.ID = sysBpmsConfiguration.ID;
            this.Name = sysBpmsConfiguration.Name;
            this.Label = sysBpmsConfiguration.Label;
            this.DefaultValue = sysBpmsConfiguration.DefaultValue;
            this.Value = sysBpmsConfiguration.Value;
            this.LastUpdateOn = sysBpmsConfiguration.LastUpdateOn;
        }
    }
}
