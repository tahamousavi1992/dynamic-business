using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class MessageTypeService : ServiceBase
    {
        public MessageTypeService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsMessageType messageType)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IMessageTypeRepository>().Add(messageType);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsMessageType messageType)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IMessageTypeRepository>().Update(messageType);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(Guid messageTypeId)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IMessageTypeRepository>().Delete(messageTypeId);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation InActive(Guid messageTypeId)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                sysBpmsMessageType sysMessage = this.GetInfo(messageTypeId);
                sysMessage.IsActive = false;
                resultOperation = this.Update(sysMessage);
            }
            return resultOperation;
        }

        public ResultOperation Active(Guid messageTypeId)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                sysBpmsMessageType sysMessage = this.GetInfo(messageTypeId);
                sysMessage.IsActive = true;
                resultOperation = this.Update(sysMessage);
            }
            return resultOperation;
        }

        public sysBpmsMessageType GetInfo(Guid id, string[] Includes = null)
        {
            return this.UnitOfWork.Repository<IMessageTypeRepository>().GetInfo(id, Includes);
        }

        public sysBpmsMessageType GetInfo(string name, string[] Includes = null)
        {
            return this.UnitOfWork.Repository<IMessageTypeRepository>().GetInfo(name, Includes);
        }

        /// <summary>
        /// It is sorted by Name.
        /// </summary>
        public List<sysBpmsMessageType> GetList(string name, bool? isActive, PagingProperties currentPaging = null)
        {
            return this.UnitOfWork.Repository<IMessageTypeRepository>().GetList(name, isActive, currentPaging);
        }
    }
}
