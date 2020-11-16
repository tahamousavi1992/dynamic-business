using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class LUTableService : ServiceBase
    {
        public LUTableService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public List<sysBpmsLUTable> GetList()
        {
            return this.UnitOfWork.Repository<ILUTableRepository>().GetList();
        }

        public sysBpmsLUTable GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<ILUTableRepository>().GetInfo(ID);
        }

    }
}
