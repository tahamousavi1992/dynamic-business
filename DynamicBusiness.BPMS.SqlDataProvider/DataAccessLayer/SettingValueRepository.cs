using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class SettingValueRepository : ISettingValueRepository
    {
        private Db_BPMSEntities Context;
        public SettingValueRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsSettingValue SettingValue)
        {
            SettingValue.ID = Guid.NewGuid();
            SettingValue.Value = SettingValue.Value.ToStringObj();
            this.Context.sysBpmsSettingValues.Add(SettingValue);
        }

        public void Delete(Guid ID)
        {
            sysBpmsSettingValue settingValue = this.Context.sysBpmsSettingValues.FirstOrDefault(d => d.ID == ID);
            if (settingValue != null)
            {
                this.Context.sysBpmsSettingValues.Remove(settingValue);
            }
        }

        public sysBpmsSettingValue GetInfo(Guid ID)
        {
            return (from P in this.Context.sysBpmsSettingValues
                    where P.ID == ID
                    select P).AsNoTracking().FirstOrDefault();
        }

        public List<sysBpmsSettingValue> GetList(DateTime? SetDate, string Value, Guid? SettingDefID)
        {
            Value = Value != null ? Value.Trim() : "";
            List<sysBpmsSettingValue> retVal = null;
            retVal = (from P in this.Context.sysBpmsSettingValues
                      where
                      (!SettingDefID.HasValue || P.SettingDefID == SettingDefID) &&
                       (Value == string.Empty || P.Value.Trim() == Value) &&
                       (!SetDate.HasValue || (System.Data.Entity.DbFunctions.TruncateTime(P.SetDate) == System.Data.Entity.DbFunctions.TruncateTime(SetDate.Value)))
                      select P).ToList();
            return retVal;
        }

        public List<sysBpmsSettingValue> GetList(DateTime? SetDate, string Value, string NameDef)
        {
            Value = Value != null ? Value.Trim() : "";
            NameDef = NameDef != null ? NameDef.Trim() : "";
            List<sysBpmsSettingValue> retVal = null;

            retVal = (from P in this.Context.sysBpmsSettingValues
                      join
                          D in this.Context.sysBpmsSettingDefs on P.SettingDefID equals D.ID
                      where
                        (NameDef == string.Empty || D.Name.Trim() == NameDef) &&
                        (Value == string.Empty || P.Value.Trim() == Value) &&
                        (!SetDate.HasValue || (System.Data.Entity.DbFunctions.TruncateTime(P.SetDate) == System.Data.Entity.DbFunctions.TruncateTime(SetDate.Value)))
                      select P).ToList();

            return retVal;
        }

        public string GetValue(string NameDef)
        {
            NameDef = NameDef != null ? NameDef.Trim() : "";
            string retVal = "";
            var setdef = (from P in this.Context.sysBpmsSettingDefs
                          where
                            (P.Name.Trim() == NameDef)
                          select P).Include(c => c.sysBpmsSettingValues).FirstOrDefault();
            if (setdef != null)
            {
                retVal = setdef.sysBpmsSettingValues.Any() ? setdef.sysBpmsSettingValues.LastOrDefault().Value : setdef.DefaultValue;
            }
            return retVal;
        }

        public void Update(sysBpmsSettingValue SettingValue)
        {
            sysBpmsSettingValue retVal = (from p in this.Context.sysBpmsSettingValues
                                      where p.ID == SettingValue.ID
                                   select p).FirstOrDefault();
            retVal.Load(SettingValue);
        }

    }
}
