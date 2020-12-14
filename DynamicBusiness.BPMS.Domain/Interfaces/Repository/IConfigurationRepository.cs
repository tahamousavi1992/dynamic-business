using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IConfigurationRepository
    {
        void Add(sysBpmsConfiguration sysBpmsConfiguration1);
        void Update(sysBpmsConfiguration sysBpmsConfiguration);  
        List<sysBpmsConfiguration> GetList(string value, string name);
        string GetValue(string name);
    }
}
