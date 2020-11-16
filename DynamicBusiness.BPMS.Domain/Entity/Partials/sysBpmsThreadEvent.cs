using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsThreadEvent
    {
        public sysBpmsThreadEvent Update(Guid threadID, Guid eventID, DateTime startDate,
            DateTime executeDate, int StatusLU, Guid? threadTaskID)
        {
            this.ThreadID = threadID;
            this.EventID = eventID;
            this.StartDate = startDate;
            this.ExecuteDate = executeDate;
            this.StatusLU = StatusLU;
            this.ThreadTaskID = threadTaskID;
            return this;
        }

        public sysBpmsThreadEvent Done()
        {
            this.ExecuteDate = DateTime.Now;
            this.StatusLU = (int)sysBpmsThreadEvent.e_StatusLU.Done;
            return this;
        }

        public void Load(sysBpmsThreadEvent threadEvent)
        {
            this.ID = threadEvent.ID;
            this.ThreadID = threadEvent.ThreadID;
            this.EventID = threadEvent.EventID;
            this.StartDate = threadEvent.StartDate;
            this.ExecuteDate = threadEvent.ExecuteDate;
            this.StatusLU = threadEvent.StatusLU;
            this.ThreadTaskID = threadEvent.ThreadTaskID;
        }

        public enum e_StatusLU
        {
            [Description("In Progress")]
            InProgress = 1,
            [Description("Done")]
            Done = 2,
        }
    }
}
