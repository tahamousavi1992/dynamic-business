using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface ISettingValueRepository
    {
        void Add(sysBpmsSettingValue SettingValue);
        void Update(sysBpmsSettingValue SettingValue);
        void Delete(Guid ID);
        sysBpmsSettingValue GetInfo(Guid ID);
        List<sysBpmsSettingValue> GetList(DateTime? SetDate, string Value, Guid? SettingDefID);
        List<sysBpmsSettingValue> GetList(DateTime? SetDate, string Value, string NameDef);
        string GetValue(string NameDef);
    }
}
