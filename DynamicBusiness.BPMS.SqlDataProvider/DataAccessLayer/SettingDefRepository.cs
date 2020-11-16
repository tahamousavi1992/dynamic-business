using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class SettingDefRepository : ISettingDefRepository
    {
        private Db_BPMSEntities Context;
        public SettingDefRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsSettingDef settingDef)
        {
            this.Context.sysBpmsSettingDefs.Add(settingDef);
        }

        public void Update(sysBpmsSettingDef settingDef)
        {
            sysBpmsSettingDef retVal = (from p in this.Context.sysBpmsSettingDefs
                                    where p.ID == settingDef.ID
                                 select p).FirstOrDefault();
            retVal.Load(settingDef);
        }

        public sysBpmsSettingDef GetInfo(string name)
        {
            return (from P in this.Context.sysBpmsSettingDefs
                    where P.Name == name
                    select P).AsNoTracking().FirstOrDefault();
        }

        public sysBpmsSettingDef GetInfo(Guid ID)
        {
            return (from P in this.Context.sysBpmsSettingDefs
                    where P.ID == ID
                    select P).AsNoTracking().FirstOrDefault();
        }

        public List<sysBpmsSettingDef> GetList(string Title, string Name, int? ValueTypeLU)
        {
            Title = Title != null ? Title.Trim().ToLower() : "";
            Name = Name != null ? Name.Trim().ToLower() : "";
 
            List<sysBpmsSettingDef> retVal = (from P in this.Context.sysBpmsSettingDefs
                                          where
                                       (!ValueTypeLU.HasValue || P.ValueTypeLU == ValueTypeLU) &&
                                       (Name == string.Empty || P.Name.Trim().ToLower() == Name) &&
                                       (Title == string.Empty || P.Title.Trim().ToLower() == Title)
                                       select P).ToList();

            return retVal;
        }

    }
}
