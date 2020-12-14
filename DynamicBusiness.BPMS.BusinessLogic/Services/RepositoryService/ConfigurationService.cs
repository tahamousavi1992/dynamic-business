using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ConfigurationService : ServiceBase
    {
        public ConfigurationService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsConfiguration settingValue)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                settingValue.Value = settingValue.Value.ToStringObj().Trim();
                this.UnitOfWork.Repository<IConfigurationRepository>().Add(settingValue);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsConfiguration settingValue)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                settingValue.Value = settingValue.Value.ToStringObj().Trim();
                this.UnitOfWork.Repository<IConfigurationRepository>().Update(settingValue);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }
 
        public List<sysBpmsConfiguration> GetList(string value, string name)
        {
            return this.UnitOfWork.Repository<IConfigurationRepository>().GetList(value, name);
        }

        public string GetValue(string NameDef)
        {
            try
            {
                return this.UnitOfWork.Repository<IConfigurationRepository>().GetValue(NameDef);
            }
            catch
            {
                return "";
            }           
        }
 
    }
}
