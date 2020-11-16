using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class SettingDefService : ServiceBase
    {
        public SettingDefService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }
        public ResultOperation Add(sysBpmsSettingDef SettingDef)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<ISettingDefRepository>().Add(SettingDef);
                this.UnitOfWork.Save();
            }
            return resultOperation;

        }

        public ResultOperation Update(sysBpmsSettingDef settingDef)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<ISettingDefRepository>().Update(settingDef);
                this.UnitOfWork.Save();
            }
            return resultOperation;
            
        }

        public sysBpmsSettingDef GetInfo(string Name)
        {
            return this.UnitOfWork.Repository<ISettingDefRepository>().GetInfo(Name);
        }

        public sysBpmsSettingDef GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<ISettingDefRepository>().GetInfo(ID);
        }

        public List<sysBpmsSettingDef> GetList(string Title, string Name, int? ValueTypeLU)
        {
            return this.UnitOfWork.Repository<ISettingDefRepository>().GetList(Title, Name, ValueTypeLU);
        }
    }
}
