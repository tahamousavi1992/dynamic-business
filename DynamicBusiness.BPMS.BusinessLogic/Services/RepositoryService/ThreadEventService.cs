using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ThreadEventService : ServiceBase
    {
        public ThreadEventService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsThreadEvent threadEvent)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IThreadEventRepository>().Add(threadEvent);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsThreadEvent threadEvent)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IThreadEventRepository>().Update(threadEvent);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(Guid threadEventId)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IThreadEventRepository>().Delete(threadEventId);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public sysBpmsThreadEvent GetInfo(Guid ID, string[] Includes = null)
        {
            return this.UnitOfWork.Repository<IThreadEventRepository>().GetInfo(ID, Includes);
        }

        public List<sysBpmsThreadEvent> GetTimerActive(string[] Includes = null)
        {
            return this.UnitOfWork.Repository<IThreadEventRepository>().GetTimerActive(Includes);
        }

        public List<sysBpmsThreadEvent> GetMessageActive(Guid notProcessID, Guid messageTypeID, string[] Includes = null)
        {
            return this.UnitOfWork.Repository<IThreadEventRepository>().GetMessageActive(notProcessID, messageTypeID, Includes);
        }

        public List<sysBpmsThreadEvent> GetActive(Guid threadId)
        {
            return this.UnitOfWork.Repository<IThreadEventRepository>().GetActive(threadId);
        }

        public int GetCount(Guid? threadId, Guid eventID, Guid? ProcessID, int? statusLU, string[] Includes = null)
        {
            return this.UnitOfWork.Repository<IThreadEventRepository>().GetCount(threadId, eventID, ProcessID, statusLU, Includes);
        }

        public sysBpmsThreadEvent GetLastExecuted(Guid? threadId, Guid? ProcessID, Guid? eventID, string[] Includes = null)
        {
            return this.UnitOfWork.Repository<IThreadEventRepository>().GetLastExecuted(threadId, ProcessID, eventID, Includes);
        }
        public List<sysBpmsThreadEvent> GetList(Guid threadId)
        {
            return this.UnitOfWork.Repository<IThreadEventRepository>().GetList(threadId);
        }

    }
}
