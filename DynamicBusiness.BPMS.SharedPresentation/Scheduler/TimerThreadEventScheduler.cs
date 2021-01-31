using DotNetNuke.Services.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.BusinessLogic;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    public class TimerThreadEventScheduler : SchedulerClient
    {
        public TimerThreadEventScheduler(ScheduleHistoryItem oItem)
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

                    List<sysBpmsThreadEvent> listThreadEvent = threadEventService.GetTimerActive(new string[] { nameof(sysBpmsThreadEvent.Thread), nameof(sysBpmsThreadEvent.Event) });
                    foreach (var Item in listThreadEvent)
                    {
                        using (ProcessEngine processEngine = new ProcessEngine(new EngineSharedModel(Item.Thread, Item.Thread.ProcessID, null, string.Empty, string.Empty)))
                        {
                            (ResultOperation result, List<MessageModel> message) = processEngine.ContinueProcess(Item, true);
                            if (result.IsSuccess)
                            {

                                //Add new event If it is an interval timer event
                                if (Item.Event.SubTypeTimerEventModel?.Type == (int)SubTypeTimerEventModel.e_Type.Interval &&
                                    Item.Event.CancelActivity != true)
                                {
                                    if (Item.ThreadTaskID.HasValue)
                                    {
                                        sysBpmsThreadTask boundedThreadTask = new ThreadTaskService().GetInfo(Item.ThreadTaskID.Value);
                                        if (boundedThreadTask.StatusLU != (int)sysBpmsThreadTask.e_StatusLU.Done)
                                            new EventEngine(processEngine.EngineSharedModel).NextTimerExecuteDate(Item.Event, Item.ThreadTaskID);
                                    }
                                }
                            }
                            else
                            {
                                DotNetNuke.Services.Exceptions.Exceptions.LogException(new Exception(result.GetErrors()));
                            }
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
            string typeName = "DynamicBusiness.BPMS.SharedPresentation.TimerThreadEventScheduler, DynamicBusiness.BPMS.SharedPresentation";
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
                objSchedule.FriendlyName = "Continue Process With List Active ThreadEvent.";
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