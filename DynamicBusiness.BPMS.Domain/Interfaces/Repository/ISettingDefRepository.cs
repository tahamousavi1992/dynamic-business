using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface ISettingDefRepository
    {
        void Add(sysBpmsSettingDef SettingDef);

        void Update(sysBpmsSettingDef SettingDef);

        sysBpmsSettingDef GetInfo(string Name);

        sysBpmsSettingDef GetInfo(Guid ID);

        List<sysBpmsSettingDef> GetList(string Title, string Name, int? ValueTypeLU);
    }
}
