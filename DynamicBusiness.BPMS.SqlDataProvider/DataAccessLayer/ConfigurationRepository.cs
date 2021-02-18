using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private Db_BPMSEntities Context;
        public ConfigurationRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsConfiguration SettingValue)
        {
            SettingValue.ID = Guid.NewGuid();
            SettingValue.Value = SettingValue.Value.ToStringObj();
            this.Context.sysBpmsConfigurations.Add(SettingValue);
        }

        public List<sysBpmsConfiguration> GetList(string value, string name)
        {
            value = value.ToStringObj().Trim();
            name = name.ToStringObj().Trim();
            List<sysBpmsConfiguration> retVal = null;

            retVal = (from P in this.Context.sysBpmsConfigurations
                      where
                        (name == string.Empty || P.Name.Trim() == name) &&
                        (value == string.Empty || P.Value.Trim() == value)
                      select P).ToList();

            return retVal;
        }

        public string GetValue(string name)
        {
            name = name.ToStringObj().Trim();
            string retVal = "";
            var setdef = (from P in this.Context.sysBpmsConfigurations
                          where
                            (P.Name.Trim() == name)
                          select P).FirstOrDefault();
            if (setdef != null)
            {
                retVal = string.IsNullOrWhiteSpace(setdef.Value) ? setdef.DefaultValue : setdef.Value;
            }
            return retVal;
        }

        public void Update(sysBpmsConfiguration SettingValue)
        {
            this.Context.Entry(SettingValue).State = EntityState.Modified;
        }

    }
}
