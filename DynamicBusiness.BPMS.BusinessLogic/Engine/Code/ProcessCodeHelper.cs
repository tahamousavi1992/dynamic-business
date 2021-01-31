using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ProcessCodeHelper : IProcessCodeHelper
    {
        private IUnitOfWork UnitOfWork { get; set; }
        public ProcessCodeHelper(IUnitOfWork unitOfWork)
        {
            this.UnitOfWork = unitOfWork;
        }

        public string GetThreadProcessStatus(Guid threadID)
        {
            sysBpmsThread sysBpmsThread = new ThreadService(this.UnitOfWork).GetInfo(threadID);
            if (sysBpmsThread.StatusLU == (int)sysBpmsThread.Enum_StatusLU.InProgress ||
                sysBpmsThread.StatusLU == (int)sysBpmsThread.Enum_StatusLU.Draft)
                return string.Join(",", new ThreadTaskService(this.UnitOfWork).GetList(threadID, null, null, null).Where(c => c.StatusLU == (int)sysBpmsThreadTask.e_StatusLU.New || c.StatusLU == (int)sysBpmsThreadTask.e_StatusLU.Ongoing).Select(c => c.Task.Element.Name).ToList());
            else return ((sysBpmsThread.Enum_StatusLU)sysBpmsThread.StatusLU).GetDescription();
        }
    }
}
