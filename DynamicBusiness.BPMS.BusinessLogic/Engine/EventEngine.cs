using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class EventEngine : BaseEngine
    {
        public EventEngine(EngineSharedModel engineSharedModel, IUnitOfWork unitOfWork = null) : base(engineSharedModel, unitOfWork) { }

        public ResultOperation AddThreadEventMessage(sysBpmsEvent senderEvent, Guid? threadTaskId)
        {
            ResultOperation resultOperation = new ResultOperation();

            ThreadEventService threadEventService = new ThreadEventService(base.UnitOfWork);
            sysBpmsThreadEvent threadEvent = new sysBpmsThreadEvent()
            {
                StartDate = DateTime.Now,
                EventID = senderEvent.ID,
                StatusLU = (int)sysBpmsThreadEvent.e_StatusLU.InProgress,
                ThreadID = base.EngineSharedModel.CurrentThreadID.Value,
                ThreadTaskID = threadTaskId,
                ExecuteDate = DateTime.Now,//it eill be updated in SendMessage method.
            };
            threadEventService.Add(threadEvent);

            return resultOperation;
        }

        public ResultOperation SendMessage(sysBpmsEvent senderEvent)
        {
            ResultOperation resultOperation = new ResultOperation();
            EventService eventService = new EventService(base.UnitOfWork);
            ThreadEventService threadEventService = new ThreadEventService(base.UnitOfWork);
            DataManageEngine dataManageEngine = new DataManageEngine(base.EngineSharedModel, base.UnitOfWork);

            string senderKey = string.Empty;

            switch ((SubTypeMessageEventModel.e_KeyType)senderEvent.SubTypeMessageEventModel.KeyType)
            {
                case SubTypeMessageEventModel.e_KeyType.Static:
                    senderKey = senderEvent.SubTypeMessageEventModel.Key;
                    break;
                case SubTypeMessageEventModel.e_KeyType.Variable:
                    senderKey = dataManageEngine.GetValueByBinding(senderEvent.SubTypeMessageEventModel.Key).ToStringObj();
                    break;
            }
            sysBpmsMessageType messageType = new MessageTypeService(base.UnitOfWork).GetInfo(senderEvent.MessageTypeID.Value);

            Dictionary<string, object> listParameter = senderEvent.SubTypeMessageEventModel.MessageParams.ToDictionary(c => c.Name, c =>
            {
                return dataManageEngine.GetValueByBinding(c.Variable);
            });
            //start events
            List<sysBpmsEvent> sysStartEvents = eventService.GetListStartMessage(senderEvent.ProcessID, senderKey, senderEvent.MessageTypeID.Value);
            foreach (sysBpmsEvent item in sysStartEvents)
            {
                Dictionary<string, object> listDefaultVariables = item.SubTypeMessageEventModel.MessageParams.ToDictionary(c => c.Variable, c =>
                {
                    return listParameter.FirstOrDefault(d => d.Key == c.Name).Value;
                });
                new ProcessEngine(new EngineSharedModel(currentThread: null, item.ProcessID, base.EngineSharedModel.BaseQueryModel, base.EngineSharedModel.CurrentUserName, base.EngineSharedModel.ApiSessionID), base.UnitOfWork).StartProcessBySystem(listDefaultVariables);
                return resultOperation;
            }

            //intermediate and boundary events
            List<sysBpmsThreadEvent> sysBpmsThreadEvents = threadEventService.GetMessageActive(senderEvent.ProcessID, senderEvent.MessageTypeID.Value, new string[] { nameof(sysBpmsThreadEvent.Event), nameof(sysBpmsThreadEvent.Thread) });
            foreach (sysBpmsThreadEvent item in sysBpmsThreadEvents)
            {
                DataManageEngine receiverDataManage = new DataManageEngine(new EngineSharedModel(item.Thread, item.Event.ProcessID, base.EngineSharedModel.BaseQueryModel, base.EngineSharedModel.CurrentUserName, base.EngineSharedModel.ApiSessionID), base.UnitOfWork);
                string receiverKey = string.Empty; ;
                switch ((SubTypeMessageEventModel.e_KeyType)item.Event.SubTypeMessageEventModel.KeyType)
                {
                    case SubTypeMessageEventModel.e_KeyType.Static:
                        receiverKey = item.Event.SubTypeMessageEventModel.Key;
                        break;
                    case SubTypeMessageEventModel.e_KeyType.Variable:
                        receiverKey = receiverDataManage.GetValueByBinding(item.Event.SubTypeMessageEventModel.Key).ToStringObj();
                        break;
                }
                if (senderKey == receiverKey)
                {
                    Dictionary<string, object> listDefaultVariables = item.Event.SubTypeMessageEventModel?.MessageParams?.Where(c => !string.IsNullOrWhiteSpace(c.Variable)).ToDictionary(c => c.Variable, c =>
                      {
                          return listParameter.FirstOrDefault(d => d.Key == c.Name).Value;
                      });
                    new ProcessEngine(new EngineSharedModel(item.Thread, item.Event.ProcessID, base.EngineSharedModel.BaseQueryModel, base.EngineSharedModel.CurrentUserName, base.EngineSharedModel.ApiSessionID), base.UnitOfWork).ContinueProcess(item, true, listDefaultVariables);
                    //Even if there is a error , this method must continue and log that error. 
                    return resultOperation;
                }
            }

            return resultOperation;
        }

        public ResultOperation SendEmail(sysBpmsEvent _event)
        {
            ResultOperation resultOperation = new ResultOperation();
            SubTypeEmailEventModel email = _event.SubTypeMessageEventModel.Email;
            sysBpmsEmailAccount fromEmailAccount = new sysBpmsEmailAccount();
            DataManageEngine dataManageEngine = new DataManageEngine(base.EngineSharedModel, base.UnitOfWork);

            List<string> toEmailList = new List<string>();

            switch (email.From.ToIntObj())
            {
                case (int)SubTypeEmailEventModel.e_FromType.CurrentUser:
                    sysBpmsUser user = new UserService(base.UnitOfWork).GetInfo(base.EngineSharedModel.CurrentUserName);
                    if (user != null)
                        fromEmailAccount = new EmailAccountService(base.UnitOfWork).GetList((int)sysBpmsEmailAccount.e_ObjectTypeLU.User, user.ID, null).LastOrDefault();
                    break;
                case (int)SubTypeEmailEventModel.e_FromType.CurrentThreadUser:
                    user = base.EngineSharedModel.CurrentThread != null && base.EngineSharedModel.CurrentThread.UserID.HasValue ? new UserService(base.UnitOfWork).GetInfo(base.EngineSharedModel.CurrentThread.UserID.Value) : null;
                    if (user != null)
                        fromEmailAccount = new EmailAccountService(base.UnitOfWork).GetList((int)sysBpmsEmailAccount.e_ObjectTypeLU.User, user.ID, null).LastOrDefault();
                    break;
                default:
                    fromEmailAccount = new EmailAccountService(base.UnitOfWork).GetInfo(email.From.ToGuidObj());
                    break;
            }

            switch ((SubTypeEmailEventModel.e_ToType)email.ToType)
            {
                case SubTypeEmailEventModel.e_ToType.Static:
                    toEmailList = email.To.Split(',').Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
                    break;
                case SubTypeEmailEventModel.e_ToType.Systemic:
                    switch ((SubTypeEmailEventModel.e_ToSystemicType)email.To.ToIntObj())
                    {
                        case SubTypeEmailEventModel.e_ToSystemicType.CurrentUser:
                            sysBpmsUser user = new UserService(base.UnitOfWork).GetInfo(base.EngineSharedModel.CurrentUserName);
                            if (user != null)
                                toEmailList.Add(user.Email);
                            break;
                        case SubTypeEmailEventModel.e_ToSystemicType.CurrentThreadUser:
                            user = base.EngineSharedModel.CurrentThread != null && base.EngineSharedModel.CurrentThread.UserID.HasValue ? new UserService(base.UnitOfWork).GetInfo(base.EngineSharedModel.CurrentThread.UserID.Value) : null;
                            if (user != null)
                                toEmailList.Add(user.Email);
                            break;
                    }
                    break;
                case SubTypeEmailEventModel.e_ToType.Variable:
                    toEmailList = dataManageEngine.GetValueByBinding(email.To).ToStringObj().Split(',').Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
                    break;
            }

            string subject = email.Subject;
            string content = email.Content;
            if (!string.IsNullOrWhiteSpace(subject))
            {
                foreach (string item in DomainUtility.GetRegularValue("[", "]", subject).Distinct())
                {
                    subject = subject.Replace("[" + item + "]", dataManageEngine.GetValueByBinding(item.Trim()).ToStringObj());
                }
            }

            if (!string.IsNullOrWhiteSpace(content))
            {
                foreach (string item in DomainUtility.GetRegularValue("[", "]", content).Distinct())
                {
                    content = content.Replace("[" + item + "]", dataManageEngine.GetValueByBinding(item.Trim()).ToStringObj());
                }
            }

            new EmailService().SendEmailAsync(fromEmailAccount.Email, fromEmailAccount.MailPassword, fromEmailAccount.SMTP, fromEmailAccount.Port.ToIntObj(), toEmailList, "", "", subject, content);
            return resultOperation;
        }


        /// <summary>
        /// this method add threadEvent for each type of event but it depend on event type.
        /// </summary>
        /// <param name="_event">if event is start one ,it will add it after calling ProcessEngine.ContinueProcess method, because start event should add after condition time ad continue immediately but other events just add before condition time to execute after schedule executing .</param>
        public ResultOperation NextTimerExecuteDate(sysBpmsEvent _event, Guid? threadTaskId)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (_event.SubTypeTimerEventModel != null && base.EngineSharedModel.CurrentProcessID.HasValue)
            {
                sysBpmsThreadEvent newThreadEvent = new sysBpmsThreadEvent()
                {
                    StartDate = DateTime.Now,
                    EventID = _event.ID,
                    StatusLU = (int)sysBpmsThreadEvent.e_StatusLU.InProgress,
                    ThreadID = base.EngineSharedModel.CurrentThreadID.Value,
                    ThreadTaskID = threadTaskId
                };
                ThreadEventService threadEventService = new ThreadEventService(base.UnitOfWork);
                sysBpmsThreadEvent lastExecutedThreadEvent;
                switch ((SubTypeTimerEventModel.e_Type)_event.SubTypeTimerEventModel.Type)
                {
                    case SubTypeTimerEventModel.e_Type.Interval:
                        lastExecutedThreadEvent = this.GetLastExecutedThreadEventForInterval(_event);
                        this.SetThreadEventOnInterval(_event, newThreadEvent, lastExecutedThreadEvent);
                        break;
                    case SubTypeTimerEventModel.e_Type.WaitFor:
                        lastExecutedThreadEvent = this.GetLastExecutedThreadEvent(_event);
                        this.SetThreadEventOnWaitFor(_event, newThreadEvent);
                        break;
                    case SubTypeTimerEventModel.e_Type.WaitUntil:
                        lastExecutedThreadEvent = this.GetLastExecutedThreadEvent(_event);
                        this.SetThreadEventOnWaitUntil(_event, newThreadEvent);
                        break;
                    default:
                        return resultOperation;
                }
                if (_event.TypeLU == (int)sysBpmsEvent.e_TypeLU.StartEvent)
                {
                    //if threadEvent prevoiusly was created , it would not create new one. 
                    if (newThreadEvent.ExecuteDate > DateTime.Now ||
                       (lastExecutedThreadEvent != null && newThreadEvent.ExecuteDate == lastExecutedThreadEvent.ExecuteDate))
                    {
                        return resultOperation;
                    }
                    else
                    {
                        resultOperation = new ProcessEngine(base.EngineSharedModel, base.UnitOfWork).ContinueProcess(newThreadEvent, false).Item1;
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                        newThreadEvent.ThreadID = base.EngineSharedModel.CurrentThreadID.Value;
                        newThreadEvent.StatusLU = (int)sysBpmsThreadEvent.e_StatusLU.Done;
                    }
                }
                threadEventService.Add(newThreadEvent);
            }
            return resultOperation;
        }

        private void SetThreadEventOnInterval(sysBpmsEvent _event, sysBpmsThreadEvent newThreadEvent, sysBpmsThreadEvent lastExecutedThreadEvent)
        {
            IDataManageEngine dataManageEngine = new DataManageEngine(base.EngineSharedModel, base.UnitOfWork);
            string variableData = _event.SubTypeTimerEventModel.VariableData;
            ThreadEventService threadEventService = new ThreadEventService(base.UnitOfWork);
            int executedCount = 0; 
            if (_event.TypeLU == (int)sysBpmsEvent.e_TypeLU.StartEvent)
            {
                executedCount = threadEventService.GetCount(null, _event.ID, base.EngineSharedModel.CurrentProcessID, (int)sysBpmsThreadEvent.e_StatusLU.Done);
            }
            else
            {
                executedCount = threadEventService.GetCount(base.EngineSharedModel.CurrentThreadID, _event.ID, null, (int)sysBpmsThreadEvent.e_StatusLU.Done);
            }
            PersianCalendar pc = new PersianCalendar();
            if (_event.SubTypeTimerEventModel.RepeatTimes.ToIntObj() == 0 || _event.SubTypeTimerEventModel.RepeatTimes > executedCount)
            {
                switch ((SubTypeTimerEventModel.e_IntervalType)_event.SubTypeTimerEventModel.IntervalType)
                {
                    case SubTypeTimerEventModel.e_IntervalType.SpecificMinute:
                        switch ((SubTypeTimerEventModel.e_SetType)_event.SubTypeTimerEventModel.SetType)
                        {
                            case SubTypeTimerEventModel.e_SetType.Static:
                                newThreadEvent.ExecuteDate = DateTime.Now.AddMinutes(_event.SubTypeTimerEventModel.Minute.ToIntObj());
                                break;
                            case SubTypeTimerEventModel.e_SetType.Variable:
                                newThreadEvent.ExecuteDate = DateTime.Now.AddMinutes(dataManageEngine.GetValueByBinding(variableData).ToIntObj());
                                break;
                        }
                        break;
                    case SubTypeTimerEventModel.e_IntervalType.EveryMonth:
                        int lastDayExecuted = DateTime.Now.Day - 1;
                        if (lastExecutedThreadEvent != null && lastExecutedThreadEvent.ExecuteDate.AreInTheSameMonth(DateTime.Now))
                            lastDayExecuted = lastExecutedThreadEvent.ExecuteDate.Day;

                        List<int> MonthDays = null;
                        switch ((SubTypeTimerEventModel.e_SetType)_event.SubTypeTimerEventModel.SetType)
                        {
                            case SubTypeTimerEventModel.e_SetType.Static:
                                MonthDays = _event.SubTypeTimerEventModel.GetListMonthDays;
                                break;
                            case SubTypeTimerEventModel.e_SetType.Variable:
                                MonthDays = dataManageEngine.GetValueByBinding(variableData).ToStringObj().Split(',').Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => c.ToIntObj()).ToList();
                                break;
                        }
                        int? persianMonthDay = MonthDays.FirstOrDefault(c => c > lastDayExecuted);
                        //if persianMonthDay is null ,monthDate must be filled by next month ,otherwise it is filled by current month.
                        DateTime monthDate = persianMonthDay.HasValue ? DateTime.Now : DateTime.Now.AddMonths(1);
                        if (!persianMonthDay.HasValue)
                        {
                            //if lastDayExecuted is not in the same month with persianMonthDay
                            persianMonthDay = MonthDays.FirstOrDefault();
                        }
                        newThreadEvent.ExecuteDate = pc.ToDateTime(monthDate.Year, monthDate.Month, persianMonthDay.Value, _event.SubTypeTimerEventModel.TimeHour.ToIntObj(), _event.SubTypeTimerEventModel.TimeMinute.ToIntObj(), 0, 0);
                        break;
                    case SubTypeTimerEventModel.e_IntervalType.EveryWeek:
                        lastDayExecuted = DateTime.Now.GetDayOfWeekBy7() - 1;
                        if (lastExecutedThreadEvent != null && lastExecutedThreadEvent.ExecuteDate.AreInTheSameWeek(DateTime.Now))
                            lastDayExecuted = lastExecutedThreadEvent.ExecuteDate.GetDayOfWeekBy7();

                        List<int> weekDays = null;
                        switch ((SubTypeTimerEventModel.e_SetType)_event.SubTypeTimerEventModel.SetType)
                        {
                            case SubTypeTimerEventModel.e_SetType.Static:
                                weekDays = _event.SubTypeTimerEventModel.GetListWeekDays;
                                break;
                            case SubTypeTimerEventModel.e_SetType.Variable:
                                weekDays = dataManageEngine.GetValueByBinding(variableData).ToStringObj().Split(',').Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => c.ToIntObj()).ToList();
                                break;
                        }
                        //if weekDays has a day bigger than last day executed 
                        int? weekDay7 = weekDays.FirstOrDefault(c => c > lastDayExecuted);

                        //if need to go next week because ther is no greater day of week in weekDays and start from the first day of week selected by user after monday. 
                        if (!weekDay7.HasValue)
                        {
                            weekDay7 = weekDays.FirstOrDefault();
                            //lastDayExecuted is not in the same week with persianWeekDay
                            newThreadEvent.ExecuteDate = DateTime.Now.AddDays(weekDay7.Value + (7 - DateTime.Now.GetDayOfWeekBy7()));
                        }
                        else
                        {
                            //lastDayExecuted is in the same week with persianWeekDay
                            newThreadEvent.ExecuteDate = DateTime.Now.AddDays(weekDay7.Value - lastDayExecuted);
                        }
                        break;
                }
            }
        }

        private void SetThreadEventOnWaitFor(sysBpmsEvent _event, sysBpmsThreadEvent newThreadEvent)
        { 
            string variableData = _event.SubTypeTimerEventModel.VariableData;
            switch ((SubTypeTimerEventModel.e_SetType)_event.SubTypeTimerEventModel.SetType)
            {
                case SubTypeTimerEventModel.e_SetType.Static:
                    if (_event.TypeLU == (int)sysBpmsEvent.e_TypeLU.StartEvent)
                        newThreadEvent.ExecuteDate = new ProcessService(base.UnitOfWork).GetInfo(_event.Element.ProcessID).PublishDate.Value.AddMinutes(_event.SubTypeTimerEventModel.Minute.Value);
                    else
                        newThreadEvent.ExecuteDate = DateTime.Now.AddMinutes(_event.SubTypeTimerEventModel.Minute.Value);
                    break;
                case SubTypeTimerEventModel.e_SetType.Variable:
                    newThreadEvent.ExecuteDate = DateTime.Now.AddMinutes(new DataManageEngine(base.EngineSharedModel, base.UnitOfWork).GetValueByBinding(variableData).ToIntObj());
                    break;
            }
        }

        private void SetThreadEventOnWaitUntil(sysBpmsEvent _event, sysBpmsThreadEvent newThreadEvent)
        {
            string variableData = _event.SubTypeTimerEventModel.VariableData;
            switch ((SubTypeTimerEventModel.e_SetType)_event.SubTypeTimerEventModel.SetType)
            {
                case SubTypeTimerEventModel.e_SetType.Static:
                    newThreadEvent.ExecuteDate = _event.SubTypeTimerEventModel.DateTime.Value;
                    break;
                case SubTypeTimerEventModel.e_SetType.Variable:
                    newThreadEvent.ExecuteDate = Convert.ToDateTime(new DataManageEngine(base.EngineSharedModel, base.UnitOfWork).GetValueByBinding(variableData));
                    break;
            }
        }

        private sysBpmsThreadEvent GetLastExecutedThreadEventForInterval(sysBpmsEvent _event)
        {
            if (_event.TypeLU == (int)sysBpmsEvent.e_TypeLU.StartEvent)
            {
                return new ThreadEventService(base.UnitOfWork).GetLastExecuted(null, base.EngineSharedModel.CurrentProcessID, _event.ID);
            }
            else
            {
                return new ThreadEventService(base.UnitOfWork).GetLastExecuted(base.EngineSharedModel.CurrentThreadID, null, _event.ID);
            }
        }

        private sysBpmsThreadEvent GetLastExecutedThreadEvent(sysBpmsEvent _event)
        {
            return new ThreadEventService(base.UnitOfWork).GetLastExecuted(_event.TypeLU == (int)sysBpmsEvent.e_TypeLU.StartEvent ? (Guid?)null : base.EngineSharedModel.CurrentThreadID,
            _event.TypeLU == (int)sysBpmsEvent.e_TypeLU.StartEvent ? base.EngineSharedModel.CurrentProcessID : (Guid?)null, _event.ID);
        }
    }
}
