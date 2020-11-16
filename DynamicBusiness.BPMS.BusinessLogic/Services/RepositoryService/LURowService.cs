using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class LURowService : ServiceBase
    {
        public LURowService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public int MaxOrderByLUTableID(Guid LUTableID)
        {
            return this.UnitOfWork.Repository<ILURowRepository>().MaxOrderByLUTableID(LUTableID);
        }
        public int MaxCodeOfByLUTableID(Guid LUTableID)
        {
            return this.UnitOfWork.Repository<ILURowRepository>().MaxCodeOfByLUTableID(LUTableID);
        }
        public List<sysBpmsLURow> GetList(Guid LUTableID, string NameOf, bool? IsActive)
        {
            return this.UnitOfWork.Repository<ILURowRepository>().GetList(LUTableID, NameOf, IsActive);
        }
        public List<sysBpmsLURow> GetList(Guid LUTableID, string NameOf, bool? IsActive, PagingProperties currentPaging)
        {
            return this.UnitOfWork.Repository<ILURowRepository>().GetList(LUTableID, NameOf, IsActive, currentPaging);
        }
        public List<sysBpmsLURow> GetList(string Alias)
        {
            return this.UnitOfWork.Repository<ILURowRepository>().GetList(Alias);
        }
        public sysBpmsLURow GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<ILURowRepository>().GetInfo(ID);
        }
        public sysBpmsLURow GetInfo(string Alias, string CodeOf)
        {
            return this.UnitOfWork.Repository<ILURowRepository>().GetInfo(Alias, CodeOf);
        }
        public sysBpmsLURow GetInfoByName(string Alias, string NameOf)
        {
            return this.UnitOfWork.Repository<ILURowRepository>().GetInfoByName(Alias, NameOf);
        }
        public sysBpmsLURow GetInfo(Guid ID, Guid LUTableID, string CodeOf)
        {
            return this.UnitOfWork.Repository<ILURowRepository>().GetInfo(ID, LUTableID, CodeOf);
        }
        public string GetNameOfByAlias(string Alias, string CodeOf)
        {
            return this.UnitOfWork.Repository<ILURowRepository>().GetNameOfByAlias(Alias, CodeOf);
        }
 
        public ResultOperation Update(sysBpmsLURow LURow)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<ILURowRepository>().Update(LURow);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }
        public ResultOperation Add(sysBpmsLURow LURow)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<ILURowRepository>().Add(LURow);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }
    }
}
