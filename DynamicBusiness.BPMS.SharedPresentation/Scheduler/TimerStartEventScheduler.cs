using DotNetNuke.Services.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.BusinessLogic;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    public class TimerStartEventScheduler : SchedulerClient
    {
        public TimerStartEventScheduler(ScheduleHistoryItem oItem)
            : base()
        {
            this.ScheduleHistoryItem = oItem;
        }

        public override void DoWork()
        {
            try
            {
                using (ThreadEventService threadEventService = new ThreadEventService())
                {
                    List<sysBpmsEvent> listEvent = new EventService().GetList((int)sysBpmsEvent.e_TypeLU.StartEvent, null, "", (int)WorkflowStartEvent.BPMNStartEventType.Timer, (int)sysBpmsProcess.Enum_StatusLU.Published, new string[] { $"{nameof(sysBpmsEvent.sysBpmsElement)}.{nameof(sysBpmsElement.sysBpmsProcess)}" });
                    foreach (sysBpmsEvent Item in listEvent)
                    {
                        using (EventEngine eventEngine = new EventEngine(new EngineSharedModel(currentThread: null, currentProcessID: Item.sysBpmsElement.ProcessID, baseQueryModel: null, currentUserName: string.Empty, apiSessionId: string.Empty)))
                        {
                            eventEngine.NextTimerExecuteDate(Item, null);
                        }
                    }
                }
                this.ScheduleHistoryItem.Succeeded = true;
            }
            catch (Exception ex)
            {
                this.ScheduleHistoryItem.Succeeded = false;
                this.Errored(ref ex);
                DotNetNuke.Services.Exceptions.Exceptions.LogException(ex);
            }
        }

        public static void CheckScheduler()
        {
            string typeName = "DynamicBusiness.BPMS.SharedPresentation.TimerStartEventScheduler, DynamicBusiness.BPMS.SharedPresentation";
            ScheduleItem objSchedule = SchedulingProvider.Instance().GetSchedule(typeName, string.Empty);
            if (objSchedule == null)
            {
                objSchedule = new ScheduleItem();
                objSchedule.TypeFullName = typeName;
                objSchedule.Enabled = true;
                objSchedule.TimeLapse = 60;
                objSchedule.TimeLapseMeasurement = "s";
                objSchedule.RetryTimeLapse = 60;
                objSchedule.RetryTimeLapseMeasurement = "s";
                objSchedule.AttachToEvent = "";
                objSchedule.CatchUpEnabled = false;
                objSchedule.ObjectDependencies = "";
                objSchedule.Servers = "";
                objSchedule.FriendlyName = "Continue Process With List Start Event.";
                objSchedule.ScheduleID = SchedulingProvider.Instance().AddSchedule(objSchedule);
                return;
            }
            else
            {
                if (objSchedule.Enabled == false)
                    objSchedule.Enabled = true;

                objSchedule.TimeLapse = 60;
                objSchedule.TimeLapseMeasurement = "s";
                objSchedule.RetryTimeLapse = 60;
                objSchedule.RetryTimeLapseMeasurement = "s";
                SchedulingProvider.Instance().UpdateSchedule(objSchedule);
            }
        }
    }
}