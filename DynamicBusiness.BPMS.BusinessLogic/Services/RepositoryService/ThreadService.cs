using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using System.Globalization;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ThreadService : ServiceBase
    {
        public ThreadService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsThread thread)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IThreadRepository>().Add(thread);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsThread thread)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                thread.GatewayStatusXml = thread.GatewayStatus.BuildXml();
                this.UnitOfWork.Repository<IThreadRepository>().Update(thread);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        /// <param name="gatewayId">is gateway ID which is Guid</param>
        public ResultOperation AddGatewayStatusXml(sysBpmsThread thread, Guid gatewayId, Guid joinFromSequenceFlowID)
        {
            if (!thread.GatewayStatus.Any(c => c.GatewayID.ToGuidObj() == gatewayId))
                thread.GatewayStatus.Add(new ThreadGatewayStatusXmlModel() { GatewayID = gatewayId, List = new List<GatewayJoinSequencrXmlModel>() });

            thread.GatewayStatus.FirstOrDefault(c => c.GatewayID == gatewayId).List.Add(new GatewayJoinSequencrXmlModel() { Done = true, SequenceFlowID = joinFromSequenceFlowID });

            return this.Update(thread);
        }

        public ResultOperation ClearGatewayStatusXml(sysBpmsThread thread, Guid gatewayId)
        {
            thread.GatewayStatus.FirstOrDefault(c => c.GatewayID == gatewayId)?.List?.Clear();
            return this.Update(thread);
        }

        public sysBpmsThread GetInfo(Guid ID, string[] includes = null)
        {
            return this.UnitOfWork.Repository<IThreadRepository>().GetInfo(ID, includes);
        }

        public int GetCountActive(Guid userId, Guid processID)
        {
            return this.UnitOfWork.Repository<IThreadRepository>().GetCountActive(userId, processID);
        }

        public int GetCount(Guid processID)
        {
            return this.UnitOfWork.Repository<IThreadRepository>().GetCount(processID);
        }
        //get all thread according to user access
        public List<sysBpmsThread> GetArchiveList(Guid? TaskOwnerUserID, Guid? ProcessID, int[] statusLU, Guid? UserID, DateTime? StartFrom, DateTime? StartTo, DateTime? EndFrom, DateTime? EndTo, PagingProperties currentPaging, string[] includes = null)
        {
            return this.UnitOfWork.Repository<IThreadRepository>().GetArchiveList(TaskOwnerUserID, ProcessID, statusLU, UserID, StartFrom, StartTo, EndFrom, EndTo, currentPaging, includes);
        }

        public ResultOperation DoneThread(Guid threadID)
        {
            sysBpmsThread thread = this.GetInfo(threadID);
            thread.EndDate = DateTime.Now;
            thread.StatusLU = (int)sysBpmsThread.Enum_StatusLU.Done;
            ResultOperation resultOperation = this.Update(thread);
            resultOperation.CurrentObject = thread;
            return resultOperation;

        }

        public ResultOperation Delete(Guid threadId)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                this.BeginTransaction();

                //Delete sysBpmsDocument
                DocumentService documentService = new DocumentService(base.UnitOfWork);
                foreach (sysBpmsDocument document in documentService.GetList(null, null, null, "", null, null, threadId))
                {
                    if (resultOperation.IsSuccess)
                        resultOperation = documentService.Delete(document.GUID);
                }

                //Delete sysBpmsThreadVariable
                ThreadVariableService threadVariableService = new ThreadVariableService(base.UnitOfWork);
                foreach (sysBpmsThreadVariable sysThreadVariable in threadVariableService.GetList(threadId, null, ""))
                {
                    if (resultOperation.IsSuccess)
                        resultOperation = threadVariableService.Delete(sysThreadVariable.ID);
                }

                //Delete sysBpmsThreadEvent
                ThreadEventService threadEventService = new ThreadEventService(base.UnitOfWork);
                foreach (sysBpmsThreadEvent sysBpmsThreadEvent in threadEventService.GetList(threadId))
                {
                    if (resultOperation.IsSuccess)
                        resultOperation = threadEventService.Delete(sysBpmsThreadEvent.ID);
                }

                //Delete sysBpmsThreadTask 
                ThreadTaskService threadTaskService = new ThreadTaskService(base.UnitOfWork);
                foreach (sysBpmsThreadTask sysBpmsThreadTask in threadTaskService.GetList(threadId, null, null, null))
                {
                    if (resultOperation.IsSuccess)
                        resultOperation = threadTaskService.Delete(sysBpmsThreadTask.ID);
                }

                //Delete Thread
                if (resultOperation.IsSuccess)
                {
                    this.UnitOfWork.Repository<IThreadRepository>().Delete(threadId);
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

        public static string GetFormattedNumber()
        {
            PersianCalendar pc = new PersianCalendar();
            return pc.GetYear(DateTime.Now) + pc.GetMonth(DateTime.Now) + pc.GetDayOfMonth(DateTime.Now) + new Random().Next(1000, 99999).ToString("0000#");
        }
    }
}
