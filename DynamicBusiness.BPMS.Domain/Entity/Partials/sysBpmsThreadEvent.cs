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

        public sysBpmsThreadEvent Clone()
        {
            return new sysBpmsThreadEvent
            {
                ID = this.ID,
                ThreadID = this.ThreadID,
                EventID = this.EventID,
                StartDate = this.StartDate,
                ExecuteDate = this.ExecuteDate,
                StatusLU = this.StatusLU,
                ThreadTaskID = this.ThreadTaskID,
            };
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
