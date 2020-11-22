using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class SettingValueService : ServiceBase
    {
        public SettingValueService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsSettingValue settingValue)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                settingValue.Value = settingValue.Value.ToStringObj().Trim();
                this.UnitOfWork.Repository<ISettingValueRepository>().Add(settingValue);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsSettingValue settingValue)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                settingValue.Value = settingValue.Value.ToStringObj().Trim();
                this.UnitOfWork.Repository<ISettingValueRepository>().Update(settingValue);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(Guid ID)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                if (resultOperation.IsSuccess)
                {
                    this.BeginTransaction();

                    this.UnitOfWork.Repository<ISettingValueRepository>().Delete(ID);
                    this.UnitOfWork.Save();

                }
            }
            catch (Exception ex)
            {
                return base.ExceptionHandler(ex);
            }
            base.FinalizeService(resultOperation);

            return resultOperation;
        }

        public sysBpmsSettingValue GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<ISettingValueRepository>().GetInfo(ID);
        }

        public List<sysBpmsSettingValue> GetList(DateTime? SetDate, string Value, Guid? SettingDefID)
        {
            return this.UnitOfWork.Repository<ISettingValueRepository>().GetList(SetDate, Value, SettingDefID);
        }

        public List<sysBpmsSettingValue> GetList(DateTime? SetDate, string Value, string NameDef)
        {
            return this.UnitOfWork.Repository<ISettingValueRepository>().GetList(SetDate, Value, NameDef);
        }

        public string GetValue(string NameDef)
        {
            try
            {
                return this.UnitOfWork.Repository<ISettingValueRepository>().GetValue(NameDef);
            }
            catch
            {
                return "";
            }           
        }
 
    }
}
