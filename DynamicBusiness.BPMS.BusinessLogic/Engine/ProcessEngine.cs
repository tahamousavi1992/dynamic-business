using DynamicBusiness.BPMS.Domain;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ProcessEngine : BaseEngine
    {
        public enum e_ActionType
        {
            Done,
            NextStep,
            PreviousStep,
        }

        /// <summary>
        /// it is used to set dynamic code message and send it to client.
        /// </summary>
        private List<MessageModel> MessageList { get; set; }

        public ProcessEngine(EngineSharedModel engineSharedModel, IUnitOfWork unitOfWork = null) : base(engineSharedModel, unitOfWork)
        {

        }

        public bool CanBeginProcess(Guid? userID, sysBpmsProcess process)
        {
            List<Guid> removedItems = new List<Guid>();
            if (process.StatusLU != (int)sysBpmsProcess.Enum_StatusLU.Published)
                return false;
            process.BeginTasks.Split(',').Where(c => !string.IsNullOrWhiteSpace(c)).ToList();

            List<Domain.sysBpmsTask> taskList = this.UnitOfWork.Repository<ITaskRepository>().GetListBeginTasks(process.ID);
            List<sysBpmsDepartmentMember> rolelist = userID.HasValue ? new DepartmentMemberService(base.UnitOfWork).GetList(null, null, userID).ToList() : null;
            foreach (var task in taskList)
            {
                if (task.UserTaskRuleModel?.AccessType == (int)UserTaskRuleModel.e_UserAccessType.Static)
                {
                    if ((task.OwnerTypeLU == (int)Domain.sysBpmsTask.e_OwnerTypeLU.User && userID.HasValue && task.UserID.Contains(userID.ToString())) ||
                       (task.OwnerTypeLU == (int)Domain.sysBpmsTask.e_OwnerTypeLU.Role &&
                       (task.RoleName == string.Empty
                       || task.RoleName == (",0:" + (int)sysBpmsDepartmentMember.e_RoleLU.Requester + ",")//it means that everyone can start this proccess.
                       || rolelist?.Count(c => task.RoleName.Split(',').Any(f => f == ("0:" + c.RoleLU.ToString().Trim()) || f == (c.DepartmentID.ToString() + ":" + c.RoleLU.ToString().Trim()))) > 0)
                       ))
                    {
                        return true;
                    }
                }
                else
                {
                    TaskEngine taskEngine = new TaskEngine(new EngineSharedModel(currentThread: null, currentProcessID: process.ID, baseQueryModel: base.EngineSharedModel.BaseQueryModel, currentUserName: base.EngineSharedModel.CurrentUserName, apiSessionId: base.EngineSharedModel.ApiSessionID), this.UnitOfWork);
                    if (taskEngine.CheckUserAccess(task, userID, rolelist))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// This method returns Processes available for users according to their access conditions.
        /// </summary>
        public List<sysBpmsProcess> GetAvailableProccess(Guid userID)
        {
            List<Guid> removedItems = new List<Guid>();
            List<Domain.sysBpmsTask> taskList = this.UnitOfWork.Repository<IProcessRepository>().GetAvailableProccess(userID);
            List<sysBpmsDepartmentMember> rolelist = new DepartmentMemberService(base.UnitOfWork).GetList(null, null, userID).ToList();
            ThreadService threadService = new ThreadService(base.UnitOfWork);
            //Remove exceeded paralled count per user
            foreach (Domain.sysBpmsTask item in taskList.Where(c => c.Element.Process.ParallelCountPerUser > 0))
            {
                if (userID != Guid.Empty && threadService.GetCountActive(userID, item.Element.ProcessID) >= item.Element.Process.ParallelCountPerUser)
                    removedItems.Add(item.ID);
            }
            //Remove items that their task is evaluated by Access Code and user's roles and ID is not included in returned method.
            foreach (Domain.sysBpmsTask item in taskList.Where(c => c.UserTaskRuleModel?.AccessType != null && c.UserTaskRuleModel?.AccessType != (int)UserTaskRuleModel.e_UserAccessType.Static))
            {
                TaskEngine taskEngine = new TaskEngine(new EngineSharedModel(currentThread: null, currentProcessID: item.Element.ProcessID, baseQueryModel: base.EngineSharedModel.BaseQueryModel, currentUserName: base.EngineSharedModel.CurrentUserName, apiSessionId: base.EngineSharedModel.ApiSessionID), this.UnitOfWork);
                if (!taskEngine.CheckUserAccess(item, userID, rolelist))
                {
                    removedItems.Add(item.ID);
                }
            }
            return taskList.Where(c => !removedItems.Any(d => d == c.ID)).GroupBy(c => c.Element.Process.ID).Select(c => c.FirstOrDefault().Element.Process).ToList();
        }

        public Tuple<ResultOperation, List<MessageModel>> NextFlow(Guid threadTaskId)
        {
            ResultOperation resultOperation = new ResultOperation();
            this.MessageList = new List<MessageModel>();
            try
            {
                sysBpmsThreadTask currentThreadTask = new ThreadTaskService(base.UnitOfWork).GetInfo(threadTaskId, new[] { nameof(sysBpmsThreadTask.Task), nameof(sysBpmsThreadTask.Thread) });
                sysBpmsUser currentUser = new UserService(base.UnitOfWork).GetInfo(base.EngineSharedModel.CurrentUserName);

                this.BeginTransaction();
                resultOperation = this.GetRecursiveElement(currentThreadTask.Task, false, currentThreadTask);
                if (!resultOperation.IsSuccess)
                    resultOperation.AddError(LangUtility.Get("Failed.Text", "Engine"));

                if (resultOperation.IsSuccess)
                {
                    //if current Thread StatusLU is Draft
                    if (this.EngineSharedModel.CurrentThread.StatusLU == (int)sysBpmsThread.Enum_StatusLU.Draft)
                    {
                        this.EngineSharedModel.CurrentThread.Update((int)sysBpmsThread.Enum_StatusLU.InProgress);
                        resultOperation = new ThreadService(base.UnitOfWork).Update(this.EngineSharedModel.CurrentThread);
                    }

                    if (resultOperation.IsSuccess)
                        resultOperation = new TaskEngine(base.EngineSharedModel, base.UnitOfWork).DoneThreadTask(currentThreadTask, currentUser);

                    if (resultOperation.IsSuccess)
                        resultOperation = new ThreadEngine(base.EngineSharedModel, base.UnitOfWork).TerminateIfPossible(currentUser);

                    if (resultOperation.IsSuccess)
                        this.UnitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
                return new Tuple<ResultOperation, List<MessageModel>>(base.ExceptionHandler(ex), this.MessageList);
            }
            base.FinalizeService(resultOperation);

            return new Tuple<ResultOperation, List<MessageModel>>(resultOperation, this.MessageList);

        }

        /// <summary>
        /// This method is called by system ,therefore EngineSharedModel should be set exclusively and also at the end this method end threadEvent.
        /// If threadEvent is start events, this method will add thread and set EngineSharedModel.CurrentThreadID.
        /// </summary>
        /// <param name="threadEvent"></param>
        public Tuple<ResultOperation, List<MessageModel>> ContinueProcess(sysBpmsThreadEvent threadEvent, bool doneThreadEvent, Dictionary<string, object> listDefaultVariables = null)
        {
            ResultOperation resultOperation = new ResultOperation();
            this.MessageList = new List<MessageModel>();
            try
            {
                sysBpmsProcess process = new ProcessService(base.UnitOfWork).GetInfo(this.EngineSharedModel.CurrentProcessID.Value);
                if (process.StatusLU == (int)sysBpmsProcess.Enum_StatusLU.Inactive || process.StatusLU == (int)sysBpmsProcess.Enum_StatusLU.Draft)
                    resultOperation.AddError(LangUtility.Get("FailedContinueStatus.Text", "Engine"));
                else
                {
                    sysBpmsEvent sysBpmsEvent = threadEvent.Event ?? new EventService(base.UnitOfWork).GetInfo(threadEvent.EventID);
                    sysBpmsUser currentUser = new UserService(base.UnitOfWork).GetInfo(base.EngineSharedModel.CurrentUserName);

                    this.BeginTransaction();

                    switch ((sysBpmsEvent.e_TypeLU)sysBpmsEvent.TypeLU)
                    {
                        case sysBpmsEvent.e_TypeLU.boundary:
                            if (sysBpmsEvent.CancelActivity == true)
                            {
                                if (threadEvent.ThreadTaskID.HasValue)
                                {
                                    sysBpmsThreadTask boundedThreadTask = new ThreadTaskService(base.UnitOfWork).GetInfo(threadEvent.ThreadTaskID.Value);
                                    //If it was done with user do nothing and return.
                                    if (boundedThreadTask.StatusLU == (int)sysBpmsThreadTask.e_StatusLU.Done)
                                        return new Tuple<ResultOperation, List<MessageModel>>(resultOperation, this.MessageList);
                                    resultOperation = new TaskEngine(base.EngineSharedModel, base.UnitOfWork).DoneThreadTask(boundedThreadTask, currentUser);
                                }
                            }
                            break;
                        case sysBpmsEvent.e_TypeLU.IntermediateCatch:

                            break;
                        case sysBpmsEvent.e_TypeLU.IntermediateThrow:

                            break;
                        case sysBpmsEvent.e_TypeLU.StartEvent:
                            sysBpmsThread thread = new sysBpmsThread();
                            thread.Update(base.EngineSharedModel.CurrentProcessID.Value, null, DateTime.Now, null, ThreadService.GetFormattedNumber(), (int)sysBpmsThread.Enum_StatusLU.Draft);
                            resultOperation = new ThreadEngine(base.EngineSharedModel, base.UnitOfWork).Add(thread);
                            base.EngineSharedModel.CurrentThread = thread;
                            base.EngineSharedModel.CurrentThreadID = thread.ID;
                            break;
                        case sysBpmsEvent.e_TypeLU.EndEvent:

                            break;
                    }
                    //If it is called from a sender message event, This is executed.
                    if (listDefaultVariables != null)
                    {
                        foreach (var item in listDefaultVariables.GroupBy(c => c.Key.Split('.')[0]))
                        {
                            VariableModel variableModel = new VariableModel(item.Key, new DataModel(item.ToList().ToDictionary(c => c.Key, c => c.Value)));
                            new DataManageEngine(base.EngineSharedModel, base.UnitOfWork).SaveIntoDataBase(variableModel, null);
                        }
                    }
                    if (resultOperation.IsSuccess)
                    {
                        resultOperation = this.GetRecursiveElement(sysBpmsEvent, false, null);
                        if (!resultOperation.IsSuccess)
                            resultOperation.AddError(LangUtility.Get("Failed.Text", "Engine"));

                        switch ((sysBpmsEvent.e_TypeLU)sysBpmsEvent.TypeLU)
                        {
                            case sysBpmsEvent.e_TypeLU.StartEvent:
                                //if a task start automatically ,it's thread userId must be set by threadTask's OwnerUserID.
                                sysBpmsThreadTask threadTask = new ThreadTaskService(base.UnitOfWork).GetList(base.EngineSharedModel.CurrentThreadID.Value, (int)sysBpmsTask.e_TypeLU.UserTask, null, (int)sysBpmsThreadTask.e_StatusLU.New).LastOrDefault();
                                if (threadTask != null && threadTask.OwnerUserID.HasValue)
                                {
                                    base.EngineSharedModel.CurrentThread.Update(userID: threadTask.OwnerUserID);
                                    new ThreadService(base.UnitOfWork).Update(base.EngineSharedModel.CurrentThread);
                                }
                                break;
                        }
                    }
                    if (resultOperation.IsSuccess)
                    {
                        if (doneThreadEvent)
                        {
                            //Make done threadEvent before checking for terminating.
                            threadEvent.Done();
                            new ThreadEventService(base.UnitOfWork).Update(threadEvent);
                        }
                        resultOperation = new ThreadEngine(base.EngineSharedModel, base.UnitOfWork).TerminateIfPossible(currentUser);

                        if (resultOperation.IsSuccess)
                            this.UnitOfWork.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                return new Tuple<ResultOperation, List<MessageModel>>(base.ExceptionHandler(ex), this.MessageList);
            }
            base.FinalizeService(resultOperation);

            return new Tuple<ResultOperation, List<MessageModel>>(resultOperation, this.MessageList);
        }

        /// <summary>
        /// this method starts a process
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="listDefaultVariables">If this is called from senderEvent it is filled by table value parameter</param>
        public Tuple<ResultOperation, List<MessageModel>> BegingProcess(Guid? userID, Dictionary<string, object> listDefaultVariables = null, bool fromSendMessageEvent = false)
        {
            ResultOperation resultOperation = new ResultOperation();
            this.MessageList = new List<MessageModel>();
            try
            {
                sysBpmsProcess process = new ProcessService(base.UnitOfWork).GetInfo(base.EngineSharedModel.CurrentProcessID.Value);
                if (!process.AllowNextFlow())
                    resultOperation.AddError(LangUtility.Get("FailedContinueStatus.Text", "Engine"));
                else
                {
                    if (process.StatusLU != (int)sysBpmsProcess.Enum_StatusLU.Published)
                        resultOperation.AddError(LangUtility.Get("FailedIsNotPublished.Text", "Engine"));

                    if(!this.CanBeginProcess(userID, process))
                        resultOperation.AddError(LangUtility.Get("AccessDeniedToBeginProcess.Text", "Engine"));

                    if (process.ParallelCountPerUser > 0)
                    {
                        if (userID.HasValue && userID != Guid.Empty && new ThreadService(base.UnitOfWork).GetCountActive(userID.Value, process.ID) >= process.ParallelCountPerUser)
                            resultOperation.AddError(LangUtility.Get("ExceedingParallel.Text", "Engine"));
                    }

                    if (resultOperation.IsSuccess)
                    {
                        if (userID.HasValue)
                        {

                        }
                        this.BeginTransaction();

                        sysBpmsThread thread = new sysBpmsThread();
                        thread.Update(base.EngineSharedModel.CurrentProcessID.Value, userID, DateTime.Now, null, ThreadService.GetFormattedNumber(), (int)sysBpmsThread.Enum_StatusLU.Draft);
                        new ThreadEngine(base.EngineSharedModel, base.UnitOfWork).Add(thread);
                        base.EngineSharedModel.CurrentThread = thread;
                        base.EngineSharedModel.CurrentThreadID = thread.ID;
                        //If it is called from a sender event, This is executed.
                        if (listDefaultVariables != null)
                        {
                            foreach (var item in listDefaultVariables.GroupBy(c => c.Key.Split('.')[0]))
                            {
                                VariableModel variableModel = new VariableModel(item.Key, new DataModel(item.ToList().ToDictionary(c => c.Key, c => c.Value)));
                                new DataManageEngine(base.EngineSharedModel, base.UnitOfWork).SaveIntoDataBase(variableModel, null);
                            }
                        }
                        sysBpmsEvent _event = new EventService(this.UnitOfWork).GetList((int)sysBpmsEvent.e_TypeLU.StartEvent, base.EngineSharedModel.CurrentProcessID, "", null).LastOrDefault();
                        resultOperation = this.GetRecursiveElement(_event, true, null);
                        if (!resultOperation.IsSuccess)
                            resultOperation.AddError(LangUtility.Get("Failed.Text", "Engine"));

                        //If it is called from a sender event, This is executed.
                        if (!userID.HasValue && fromSendMessageEvent)
                        {
                            //if a task start systemic from a  api or sendMessage ,it's thread userId must be set by threadTask's OwnerUserID.
                            sysBpmsThreadTask threadTask = new ThreadTaskService(base.UnitOfWork).GetList(base.EngineSharedModel.CurrentThreadID.Value, (int)sysBpmsTask.e_TypeLU.UserTask, null, (int)sysBpmsThreadTask.e_StatusLU.New).LastOrDefault();
                            if (threadTask != null && threadTask.OwnerUserID.HasValue)
                            {
                                base.EngineSharedModel.CurrentThread.Update(userID: threadTask.OwnerUserID);
                                new ThreadService(base.UnitOfWork).Update(base.EngineSharedModel.CurrentThread);
                            }
                        }

                        if (resultOperation.IsSuccess)
                            this.UnitOfWork.Save();

                        resultOperation.CurrentObject = thread;
                    }
                }
            }
            catch (Exception ex)
            {
                return new Tuple<ResultOperation, List<MessageModel>>(base.ExceptionHandler(ex), this.MessageList);
            }
            base.FinalizeService(resultOperation);

            return new Tuple<ResultOperation, List<MessageModel>>(resultOperation, this.MessageList);
        }

        public GetTaskFormResponseModel GetTaskForm(Guid threadTaskID, Guid? stepID)
        {
            using (ThreadTaskService threadTaskService = new ThreadTaskService())
            {
                using (UserService userService = new UserService())
                {
                    (bool access, string msg) = threadTaskService.CheckAccess(threadTaskID, userService.GetInfo(base.EngineSharedModel.CurrentUserName)?.ID, base.EngineSharedModel.CurrentProcessID.Value, false);
                    if (access)
                    {
                        var result = this.GetContentHtmlByTaskID(threadTaskID, stepID);
                        EngineFormModel engineFormModel = new EngineFormModel(result.FormModel, base.EngineSharedModel.CurrentThreadID, threadTaskID, base.EngineSharedModel.CurrentProcessID);
                        return new GetTaskFormResponseModel(engineFormModel, result.ListMessageModel, result.RedirectUrlModel);
                    }
                    else
                        return new GetTaskFormResponseModel(null, new List<MessageModel>() { new MessageModel(DisplayMessageType.error, msg) }, null);
                }
            }
        }

        public GetTaskFormResponseModel GetForm(Guid threadTaskID, Guid formID, bool? chechAccess = null)
        {
            using (ThreadTaskService threadTaskService = new ThreadTaskService())
            {
                using (UserService userService = new UserService())
                {
                    (bool access, string msg) = threadTaskService.CheckAccess(threadTaskID, userService.GetInfo(base.EngineSharedModel.CurrentUserName)?.ID, base.EngineSharedModel.CurrentProcessID.Value, false);
                    if (access || chechAccess == false)
                    {
                        var result = this.GetContentHtmlByFormID(formID, false);
                        EngineFormModel engineFormModel = new EngineFormModel(result.FormModel, base.EngineSharedModel.CurrentThreadID, threadTaskID, base.EngineSharedModel.CurrentProcessID);
                        return new GetTaskFormResponseModel(engineFormModel, result.ListMessageModel, result.RedirectUrlModel);
                    }
                    else
                        return new GetTaskFormResponseModel(null, new List<MessageModel>() { new MessageModel(DisplayMessageType.error, msg) }, null);
                }
            }
        }

        public PostTaskFormResponseModel PostTaskForm(Guid threadTaskID, Guid stepID, bool? goNext, string controlId)
        {
            using (ThreadTaskService threadTaskService = new ThreadTaskService())
            {
                using (UserService userService = new UserService())
                {
                    (bool access, string msg) = threadTaskService.CheckAccess(threadTaskID, userService.GetInfo(base.EngineSharedModel.CurrentUserName)?.ID, base.EngineSharedModel.CurrentProcessID.Value, true);
                    if (access)
                    {
                        e_ActionType e_ActionType = !goNext.HasValue ? e_ActionType.Done :
                            goNext == true ? e_ActionType.NextStep : e_ActionType.PreviousStep;

                        EngineResponseModel result = this.SaveContentHtmlTaskID(threadTaskID, stepID, e_ActionType, controlId);
                        if (result.ResultOperation.IsSuccess)
                        {
                            if (result.IsSubmit)
                            {
                                FormModel formModel = (FormModel)result.ResultOperation.CurrentObject;
                                if (formModel.NextStepID.HasValue && e_ActionType == e_ActionType.NextStep)
                                    return new PostTaskFormResponseModel(result.RedirectUrlModel, SharedLang.Get("Success.Text"), true, result.IsSubmit, formModel.NextStepID, true, result.ListMessageModel, result.ListDownloadModel);
                                else
                                {
                                    if (formModel.PreviousStepID.HasValue && e_ActionType == e_ActionType.PreviousStep)
                                        return new PostTaskFormResponseModel(result.RedirectUrlModel, LangUtility.Get("BackPreviousStep.Text", "Engine"), true, result.IsSubmit, formModel.PreviousStepID, true, result.ListMessageModel, result.ListDownloadModel);
                                    else
                                        return new PostTaskFormResponseModel(result.RedirectUrlModel, SharedLang.Get("Success.Text"), true, result.IsSubmit, null, false, result.ListMessageModel, result.ListDownloadModel);
                                }
                            }
                            else
                                return new PostTaskFormResponseModel(result.RedirectUrlModel, SharedLang.Get("Success.Text"), true, result.IsSubmit, stepID, false, result.ListMessageModel, result.ListDownloadModel);
                        }
                        else
                            return new PostTaskFormResponseModel(result.RedirectUrlModel, result.ResultOperation.GetErrors(), false, false, null);
                    }
                    else
                        return new PostTaskFormResponseModel(null, msg, false, false, null);
                }
            }
        }

        public PostTaskFormResponseModel PostForm(Guid threadTaskID, Guid formID, string controlId = "")
        {
            using (ThreadTaskService threadTaskService = new ThreadTaskService())
            {
                using (UserService userService = new UserService())
                {
                    (bool access, string msg) = threadTaskService.CheckAccess(threadTaskID, userService.GetInfo(base.EngineSharedModel.CurrentUserName)?.ID, base.EngineSharedModel.CurrentProcessID.Value, true);
                    if (access)
                    {
                        var result = this.SaveContentHtmlByFormID(threadTaskID, formID, controlId);
                        if (result.ResultOperation.IsSuccess)
                        {
                            if (result.IsSubmit)
                                return new PostTaskFormResponseModel(result.RedirectUrlModel, SharedLang.Get("Success.Text"), true, result.IsSubmit, null, null, result.ListMessageModel, result.ListDownloadModel);
                            else
                                return new PostTaskFormResponseModel(result.RedirectUrlModel, SharedLang.Get("Success.Text"), true, result.IsSubmit, null, null, result.ListMessageModel, result.ListDownloadModel);
                        }
                        else
                            return new PostTaskFormResponseModel(result.RedirectUrlModel, result.ResultOperation.GetErrors(), false, false, null, null, result.ListMessageModel);
                    }
                    else
                        return new PostTaskFormResponseModel(null, msg, false, false, null, null);
                }
            }
        }

        #region .:: private methods ::.

        private ResultOperation GetRecursiveElement(sysBpmsEvent _event, bool isFirstTask, sysBpmsThreadTask currentThreadTask)
        {
            switch ((sysBpmsEvent.e_TypeLU)_event.TypeLU)
            {
                case sysBpmsEvent.e_TypeLU.EndEvent:
                case sysBpmsEvent.e_TypeLU.IntermediateThrow:
                    switch ((WorkflowIntermediateThrowEvent.BPMNIntermediateThrowType)_event.SubType)
                    {
                        case WorkflowIntermediateThrowEvent.BPMNIntermediateThrowType.Message:

                            switch ((SubTypeMessageEventModel.e_Type)_event.SubTypeMessageEventModel.Type)
                            {
                                case SubTypeMessageEventModel.e_Type.Email:
                                    ResultOperation resultEmailOperation = new EventEngine(base.EngineSharedModel, base.UnitOfWork).SendEmail(_event);
                                    if (!resultEmailOperation.IsSuccess)
                                        return resultEmailOperation;
                                    break;
                                case SubTypeMessageEventModel.e_Type.Message:
                                    ResultOperation resultMessageOperation = new EventEngine(base.EngineSharedModel, base.UnitOfWork).SendMessage(_event);
                                    if (!resultMessageOperation.IsSuccess)
                                        return resultMessageOperation;
                                    break;
                            }
                            break;
                    }
                    break;
            }

            foreach (sysBpmsSequenceFlow item in new SequenceFlowService(this.UnitOfWork).GetList(base.EngineSharedModel.CurrentProcessID.Value, "", _event.ElementID, "").ToList())
            {
                sysBpmsElement element = new ElementService(base.UnitOfWork).GetInfo(item.TargetElementID, item.ProcessID);
                ResultOperation rOperation = this.EvaluateNextRecursiveItem(element, isFirstTask, currentThreadTask, item);
                if (!rOperation.IsSuccess)
                    return rOperation;
            }
            return new ResultOperation();
        }

        private ResultOperation GetRecursiveElement(sysBpmsGateway gateway, bool isFirstTask, sysBpmsThreadTask currentThreadTask, sysBpmsSequenceFlow joinFromSequenceFlow)
        {
            (List<sysBpmsSequenceFlow> SequenceFlows, bool result) = new GatewayEngine(base.EngineSharedModel, base.UnitOfWork).CheckGateway(gateway, joinFromSequenceFlow, currentThreadTask?.ID);
            if (!result)
            {
                ResultOperation resultOperation = new ResultOperation();
                resultOperation.AddError(string.Format(LangUtility.Get("NoPathSelected.Text", "Engine"), gateway.Element?.Name));
                return resultOperation;
            }
            foreach (sysBpmsSequenceFlow item in SequenceFlows)
            {
                ResultOperation rOperation = this.EvaluateNextRecursiveItem(new ElementService(base.UnitOfWork).GetInfo(item.TargetElementID, item.ProcessID), isFirstTask, currentThreadTask, item);
                if (!rOperation.IsSuccess)
                    return rOperation;
            }
            return new ResultOperation();
        }

        private ResultOperation GetRecursiveElement(sysBpmsTask task, bool isFirstTask, sysBpmsThreadTask currentThreadTask)
        {
            if (task.TypeLU == (int)sysBpmsTask.e_TypeLU.UserTask && task.MarkerTypeLU.HasValue)
            {
                switch ((sysBpmsTask.e_MarkerTypeLU)task.MarkerTypeLU.Value)
                {
                    case sysBpmsTask.e_MarkerTypeLU.NonSequential:
                    case sysBpmsTask.e_MarkerTypeLU.Sequential:
                        //if there is one another task,it should'nt go to next step.
                        if (new ThreadTaskService(base.UnitOfWork).GetList(this.EngineSharedModel.CurrentThreadID.Value, task.TypeLU, task.ID, null).Any(c => c.ID != currentThreadTask?.ID && c.StatusLU != (int)sysBpmsThreadTask.e_StatusLU.Done))
                            return new ResultOperation();
                        break;
                }
            }

            foreach (sysBpmsSequenceFlow item in new SequenceFlowService(base.UnitOfWork).GetList(base.EngineSharedModel.CurrentProcessID.Value, "", task.ElementID, "").ToList())
            {
                sysBpmsElement element = new ElementService(base.UnitOfWork).GetInfo(item.TargetElementID, item.ProcessID);
                ResultOperation rOperation = this.EvaluateNextRecursiveItem(element, isFirstTask, currentThreadTask, item);
                if (!rOperation.IsSuccess)
                    return rOperation;
            }

            return new ResultOperation();
        }

        private ResultOperation EvaluateNextRecursiveItem(sysBpmsElement element, bool isFirstTask, sysBpmsThreadTask currentThreadTask, sysBpmsSequenceFlow joinFromSequenceFlow)
        {
            switch ((sysBpmsElement.e_TypeLU)element.TypeLU)
            {
                case sysBpmsElement.e_TypeLU.Event:
                    sysBpmsEvent _event = new EventService(base.UnitOfWork).GetInfo(element.ID, element.ProcessID);
                    switch ((sysBpmsEvent.e_TypeLU)_event.TypeLU)
                    {
                        case sysBpmsEvent.e_TypeLU.StartEvent:
                        case sysBpmsEvent.e_TypeLU.IntermediateThrow:
                            ResultOperation rOperation = this.GetRecursiveElement(_event, isFirstTask, currentThreadTask);
                            if (!rOperation.IsSuccess)
                                return rOperation;
                            break;
                        case sysBpmsEvent.e_TypeLU.EndEvent:
                            switch ((WorkflowEndEvent.BPMNEndEventType)_event.SubType)
                            {
                                case WorkflowEndEvent.BPMNEndEventType.None:
                                    //just add to list ThreadEvent
                                    break;
                                case WorkflowEndEvent.BPMNEndEventType.Message:
                                    rOperation = this.GetRecursiveElement(_event, isFirstTask, currentThreadTask);
                                    if (!rOperation.IsSuccess)
                                        return rOperation;
                                    break;
                                case WorkflowEndEvent.BPMNEndEventType.Termination:
                                    ResultOperation resultOperation = new ThreadService(this.UnitOfWork).DoneThread(base.EngineSharedModel.CurrentThreadID.Value);
                                    if (!resultOperation.IsSuccess)
                                        throw new Exception(LangUtility.Get("TerminateError.Text", "Engine"));
                                    else
                                        //to update EngineSharedModel.CurrentThread properties.
                                        this.EngineSharedModel.CurrentThread = (sysBpmsThread)resultOperation.CurrentObject;
                                    break;
                            }
                            break;
                        case sysBpmsEvent.e_TypeLU.IntermediateCatch:
                            switch ((WorkflowIntermediateCatchEvent.BPMNIntermediateCatchType)_event.SubType)
                            {
                                case WorkflowIntermediateCatchEvent.BPMNIntermediateCatchType.Timer:
                                    return new EventEngine(base.EngineSharedModel, base.UnitOfWork).NextTimerExecuteDate(_event, null);
                                case WorkflowIntermediateCatchEvent.BPMNIntermediateCatchType.Message:
                                    return new EventEngine(base.EngineSharedModel, base.UnitOfWork).AddThreadEventMessage(_event, null);
                                default:
                                    ResultOperation resultOperation = this.GetRecursiveElement(_event, isFirstTask, currentThreadTask);
                                    if (!resultOperation.IsSuccess)
                                        return resultOperation;
                                    break;
                            }
                            break;
                        case sysBpmsEvent.e_TypeLU.boundary:
                            switch ((WorkflowBoundaryEvent.BPMNBoundaryType)_event.SubType)
                            {
                                case WorkflowBoundaryEvent.BPMNBoundaryType.timer:
                                    return new EventEngine(base.EngineSharedModel, base.UnitOfWork).NextTimerExecuteDate(_event, currentThreadTask.ID);
                                case WorkflowBoundaryEvent.BPMNBoundaryType.Message:
                                    return new EventEngine(base.EngineSharedModel, base.UnitOfWork).AddThreadEventMessage(_event, currentThreadTask.ID);
                                default:
                                    ResultOperation resultOperation = this.GetRecursiveElement(_event, isFirstTask, currentThreadTask);
                                    if (!resultOperation.IsSuccess)
                                        return resultOperation;
                                    break;
                            }
                            break;
                    }
                    break;
                case sysBpmsElement.e_TypeLU.Gateway:
                    {
                        sysBpmsGateway gateway = new GatewayService().GetInfo(element.ID, element.ProcessID);
                        ResultOperation resultOperation = this.GetRecursiveElement(gateway, isFirstTask, currentThreadTask, joinFromSequenceFlow);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                    }
                    break;
                case sysBpmsElement.e_TypeLU.Task:
                    {
                        sysBpmsTask task = new TaskService(this.UnitOfWork).GetInfo(element.ID, element.ProcessID);
                        ResultOperation rOperation = null;
                        switch ((sysBpmsTask.e_TypeLU)task.TypeLU)
                        {
                            case sysBpmsTask.e_TypeLU.ServiceTask:
                            case sysBpmsTask.e_TypeLU.Task:
                            case sysBpmsTask.e_TypeLU.ScriptTask:
                                DynamicCodeEngine dynamicCodeEngine = new DynamicCodeEngine(base.EngineSharedModel, base.UnitOfWork);
                                CodeBaseSharedModel codeBaseShared = new CodeBaseSharedModel(MessageList);
                                CodeResultModel codeResultModel = dynamicCodeEngine.ExecuteScriptCode(DesignCodeUtility.GetDesignCodeFromXml(task.Code), codeBaseShared);
                                DynamicCodeEngine.SetToErrorMessage(codeResultModel, null);
                                //If in script task any variable is set, it Will save them all at the end
                                dynamicCodeEngine.SaveExternalVariable(codeResultModel);

                                if (!(codeResultModel?.Result ?? true))
                                {
                                    rOperation = new ResultOperation();
                                    rOperation.AddError(string.Format(LangUtility.Get("ErrorActivity.Text", "Engine"), task.Element.Name));
                                    return rOperation;
                                }


                                //add service thread task
                                sysBpmsThreadTask threadTask = new sysBpmsThreadTask();
                                threadTask.Update(base.EngineSharedModel.CurrentThreadID.Value, task.ID, DateTime.Now, DateTime.Now, string.Empty, 0, (int)sysBpmsThreadTask.e_StatusLU.Done);
                                new ThreadTaskService(base.UnitOfWork).Add(threadTask);

                                rOperation = this.GetRecursiveElement(task, isFirstTask, currentThreadTask);
                                if (!rOperation.IsSuccess)
                                    return rOperation;
                                break;
                            case sysBpmsTask.e_TypeLU.UserTask:
                                (ResultOperation result, List<sysBpmsThreadTask> listTask) = new TaskEngine(base.EngineSharedModel, base.UnitOfWork).AddThreadTask(task, isFirstTask);
                                if (!result.IsSuccess)
                                    return result;
                                //make boundary event.
                                foreach (sysBpmsEvent itemEvent in new EventService(base.UnitOfWork).GetList(null, base.EngineSharedModel.CurrentProcessID, task.ElementID, null, new[] { nameof(sysBpmsEvent.Element) }))
                                {
                                    foreach (sysBpmsThreadTask item in listTask)
                                    {
                                        rOperation = this.EvaluateNextRecursiveItem(itemEvent.Element, isFirstTask, item, null);
                                        if (!rOperation.IsSuccess)
                                            return rOperation;
                                    }
                                }
                                break;
                        }
                    }
                    break;
            }
            return new ResultOperation();
        }

        public EngineResponseModel GetContentHtmlByFormID(Guid dynamicFormID, bool readOnlyMode)
        {

            CodeBaseSharedModel codeBaseShared = new CodeBaseSharedModel();
            FormModel formModel = new FormModel();
            ResultOperation resultOperation = new ResultOperation();
            sysBpmsDynamicForm dynamicForm = new DynamicFormService(base.UnitOfWork).GetInfo(dynamicFormID);

            //convert form xml code to json object
            JObject obj = JObject.Parse(dynamicForm.DesignJson);
            HtmlElementHelperModel htmlElementHelperModel = HtmlElementHelper.MakeModel(base.EngineSharedModel, base.UnitOfWork, HtmlElementHelperModel.e_FormAction.Onload, dynamicForm);
            //if json object has a control with type = CONTENT
            if (obj != null && obj["type"].ToString() == "CONTENT")
            {
                formModel = new FormModel(obj, htmlElementHelperModel, null, null, dynamicForm, readOnlyMode);
                resultOperation = formModel.ResultOperation;
            }
            CodeResultModel codeResultModel = null;
            if (resultOperation.IsSuccess)
                if (!string.IsNullOrWhiteSpace(dynamicForm.OnEntryFormCode))
                {
                    codeResultModel = new DynamicCodeEngine(base.EngineSharedModel, base.UnitOfWork).ExecuteOnEntryFormCode(DesignCodeUtility.GetDesignCodeFromXml(dynamicForm.OnEntryFormCode), formModel, codeBaseShared);
                    DynamicCodeEngine.SetToErrorMessage(codeResultModel, resultOperation);
                }
            return new EngineResponseModel().InitGet(resultOperation, codeBaseShared.MessageList, codeResultModel?.RedirectUrlModel, formModel);
        }

        private EngineResponseModel GetContentHtmlByTaskID(Guid threadTaskID, Guid? stepID)
        {
            FormModel formModel = new FormModel();
            CodeBaseSharedModel codeBaseShared = new CodeBaseSharedModel();
            sysBpmsThreadTask threadTask = new ThreadTaskService().GetInfo(threadTaskID, new string[] { nameof(sysBpmsThreadTask.Task), nameof(sysBpmsThreadTask.Thread) });
            ResultOperation resultOperation = new ResultOperation();
            List<sysBpmsStep> listStep = new StepService().GetList(threadTask.Task.ID, null);
            //if step is not specific , this line retrieve first step of task
            sysBpmsStep step = listStep.FirstOrDefault(c => !stepID.HasValue || c.ID == stepID);
            //convert form xml code to json object
            JObject obj = JObject.Parse(step.DynamicForm.DesignJson);
            HtmlElementHelperModel htmlElementHelperModel = HtmlElementHelper.MakeModel(base.EngineSharedModel, base.UnitOfWork, HtmlElementHelperModel.e_FormAction.Onload, step.DynamicForm);
            //if json object has a control with type = CONTENT
            if (obj != null && obj["type"].ToString() == "CONTENT")
            {
                formModel = new FormModel(obj, htmlElementHelperModel, listStep, step, step.DynamicForm, false);
                resultOperation = formModel.ResultOperation;
            }
            CodeResultModel codeResultModel = null;
            if (resultOperation.IsSuccess)
                if (!string.IsNullOrWhiteSpace(step.DynamicForm.OnEntryFormCode))
                {
                    DynamicCodeEngine dynamicCodeEngine = new DynamicCodeEngine(base.EngineSharedModel, base.UnitOfWork);
                    codeResultModel = new DynamicCodeEngine(base.EngineSharedModel, base.UnitOfWork).ExecuteOnEntryFormCode(DesignCodeUtility.GetDesignCodeFromXml(step.DynamicForm.OnEntryFormCode), formModel, codeBaseShared);
                    DynamicCodeEngine.SetToErrorMessage(codeResultModel, resultOperation);
                    //If in code any variable is set, it Will save them all at the end
                    dynamicCodeEngine.SaveExternalVariable(codeResultModel);
                }

            return new EngineResponseModel().InitGet(resultOperation, codeBaseShared.MessageList, codeResultModel?.RedirectUrlModel, formModel);
        }

        private EngineResponseModel SaveContentHtmlByFormID(Guid threadTaskID, Guid formID, string buttonControlId)
        {
            ResultOperation resultOperation = new ResultOperation();
            RedirectUrlModel redirectUrlModel = null;
            CodeBaseSharedModel codeBaseShared = new CodeBaseSharedModel();
            try
            {
                FormModel formModel = new FormModel();
                sysBpmsThreadTask threadTask = new ThreadTaskService(base.UnitOfWork).GetInfo(threadTaskID, new string[] { nameof(sysBpmsThreadTask.Task), nameof(sysBpmsThreadTask.Thread) });
                sysBpmsDynamicForm dynamicForm = new DynamicFormService(base.UnitOfWork).GetInfo(formID);
                //If it was done with user do nothing and return.
                if (threadTask?.StatusLU == (int)sysBpmsThreadTask.e_StatusLU.Done)
                    resultOperation.AddError(LangUtility.Get("TaskAlreadyCompleted.Text", "Engine"));
                else
                {
                    //conver form xml code to json object
                    JObject obj = JObject.Parse(dynamicForm.DesignJson);
                    //if json object has a control with type = CONTENT
                    if (obj != null && obj["type"].ToString() == "CONTENT")
                    {
                        formModel = new FormModel(obj, HtmlElementHelper.MakeModel(base.EngineSharedModel, base.UnitOfWork, HtmlElementHelperModel.e_FormAction.OnPost, dynamicForm), null, null, dynamicForm, false);
                        resultOperation = formModel.ResultOperation;
                    }
                    this.BeginTransaction();
                    if (resultOperation.IsSuccess)
                    {
                        CodeResultModel codeResultModel;
                        //It sets variables by form's widgets and adds to the codeBaseShared's ListSetVariable.
                        resultOperation = DataManageEngine.SetVariableByForms(formModel.ContentHtml, codeBaseShared, base.EngineSharedModel.BaseQueryModel);
                        if (resultOperation.IsSuccess)
                        {
                            //execute form button backend code. 
                            if (!string.IsNullOrWhiteSpace(buttonControlId))
                            {
                                ButtonHtml buttonHtml = (ButtonHtml)formModel.ContentHtml.FindControlByID(buttonControlId);
                                DynamicCodeEngine dynamicCodeEngine = new DynamicCodeEngine(base.EngineSharedModel, base.UnitOfWork);
                                codeResultModel = dynamicCodeEngine.SaveButtonCode(buttonHtml, resultOperation, formModel, codeBaseShared);
                                redirectUrlModel = codeResultModel?.RedirectUrlModel ?? redirectUrlModel;
                                if (buttonHtml.subtype != ButtonHtml.e_subtype.submit)
                                {
                                    //If in code any variable is set, it Will save them all at the end
                                    dynamicCodeEngine.SaveExternalVariable(codeResultModel);

                                    base.FinalizeService(resultOperation);
                                    return new EngineResponseModel().InitPost(resultOperation, codeBaseShared.MessageList, codeResultModel?.RedirectUrlModel, false, codeBaseShared.ListDownloadModel);
                                }
                            }

                            //execute form OnExitFormCode 
                            if (!string.IsNullOrWhiteSpace(dynamicForm.OnExitFormCode))
                            {
                                codeResultModel = new DynamicCodeEngine(base.EngineSharedModel, base.UnitOfWork).ExecuteOnExitFormCode(DesignCodeUtility.GetDesignCodeFromXml(dynamicForm.OnExitFormCode), formModel, codeBaseShared);
                                DynamicCodeEngine.SetToErrorMessage(codeResultModel, resultOperation);
                                redirectUrlModel = codeResultModel?.RedirectUrlModel ?? redirectUrlModel;
                            }
                            if (resultOperation.IsSuccess)
                                //save html element values into database.
                                resultOperation = new DataManageEngine(base.EngineSharedModel, base.UnitOfWork).SaveIntoDataBase(formModel.ContentHtml, threadTask, codeBaseShared.ListSetVariable, codeBaseShared.ThreadTaskDescription);
                        }
                    }
                    base.FinalizeService(resultOperation);

                    resultOperation.CurrentObject = formModel;
                }
            }
            catch (Exception ex)
            {
                return new EngineResponseModel().InitPost(base.ExceptionHandler(ex), codeBaseShared.MessageList, null);
            }

            return new EngineResponseModel().InitPost(resultOperation, codeBaseShared.MessageList, redirectUrlModel, listDownloadModel: codeBaseShared.ListDownloadModel);
        }

        private EngineResponseModel SaveContentHtmlTaskID(Guid threadTaskID, Guid stepID, e_ActionType e_ActionType, string buttonControlId)
        {
            ResultOperation resultOperation = new ResultOperation();
            CodeResultModel codeResultModel = null;
            RedirectUrlModel redirectUrlModel = null;
            CodeBaseSharedModel codeBaseShared = new CodeBaseSharedModel();
            try
            {
                FormModel formModel = new FormModel();
                sysBpmsThreadTask threadTask = new ThreadTaskService(base.UnitOfWork).GetInfo(threadTaskID, new string[] { nameof(sysBpmsThreadTask.Task), nameof(sysBpmsThreadTask.Thread) });
                //If it was done with user do nothing and return.
                if (threadTask.StatusLU == (int)sysBpmsThreadTask.e_StatusLU.Done)
                    resultOperation.AddError(LangUtility.Get("TaskAlreadyCompleted.Text", "Engine"));
                else
                {
                    List<sysBpmsStep> listStep = new StepService(base.UnitOfWork).GetList(threadTask.Task.ID, null);
                    sysBpmsStep step = listStep.FirstOrDefault(c => c.ID == stepID);

                    //conver form xml code to json object
                    JObject obj = JObject.Parse(step.DynamicForm.DesignJson);
                    //if json object has a control with type = CONTENT
                    if (obj != null && obj["type"].ToString() == "CONTENT")
                    {
                        formModel = new FormModel(obj, HtmlElementHelper.MakeModel(base.EngineSharedModel, base.UnitOfWork, HtmlElementHelperModel.e_FormAction.OnPost, step.DynamicForm), listStep, step, step.DynamicForm, false);
                        resultOperation = formModel.ResultOperation;
                    }
                    if (e_ActionType != e_ActionType.PreviousStep)
                    {
                        this.BeginTransaction();
                        if (resultOperation.IsSuccess)
                        {
                            //It sets variables by form's widgets and adds to the codeBaseShared's ListSetVariable.
                            resultOperation = DataManageEngine.SetVariableByForms(formModel.ContentHtml, codeBaseShared, base.EngineSharedModel.BaseQueryModel);
                            if (resultOperation.IsSuccess)
                            {
                                //execute form button backend code. 
                                if (!string.IsNullOrWhiteSpace(buttonControlId))
                                {
                                    ButtonHtml buttonHtml = (ButtonHtml)formModel.ContentHtml.FindControlByID(buttonControlId);
                                    if (buttonHtml != null)
                                    {
                                        DynamicCodeEngine dynamicCodeEngine = new DynamicCodeEngine(base.EngineSharedModel, base.UnitOfWork);
                                        codeResultModel = dynamicCodeEngine.SaveButtonCode(buttonHtml, resultOperation, formModel, codeBaseShared);
                                        redirectUrlModel = codeResultModel?.RedirectUrlModel ?? redirectUrlModel;
                                        if (buttonHtml.subtype != ButtonHtml.e_subtype.submit)
                                        {
                                            //If in code any variable is set, it Will save them all at the end
                                            dynamicCodeEngine.SaveExternalVariable(codeResultModel);

                                            base.FinalizeService(resultOperation);
                                            return new EngineResponseModel().InitPost(resultOperation, codeBaseShared.MessageList, codeResultModel?.RedirectUrlModel, isSubmit: false, listDownloadModel: codeBaseShared.ListDownloadModel);
                                        }
                                    }
                                }
                                if (resultOperation.IsSuccess)
                                {
                                    //execute form OnExitFormCode 
                                    if (!string.IsNullOrWhiteSpace(step.DynamicForm.OnExitFormCode))
                                    {
                                        codeResultModel = new DynamicCodeEngine(base.EngineSharedModel, base.UnitOfWork).ExecuteOnExitFormCode(DesignCodeUtility.GetDesignCodeFromXml(step.DynamicForm.OnExitFormCode), formModel, codeBaseShared);
                                        DynamicCodeEngine.SetToErrorMessage(codeResultModel, resultOperation);
                                        redirectUrlModel = codeResultModel?.RedirectUrlModel ?? redirectUrlModel;
                                    }
                                }

                                if (resultOperation.IsSuccess)
                                    //save html element values into database.
                                    resultOperation = new DataManageEngine(base.EngineSharedModel, base.UnitOfWork).SaveIntoDataBase(formModel.ContentHtml, threadTask, codeBaseShared.ListSetVariable, codeBaseShared.ThreadTaskDescription);
                            }
                        }
                        base.FinalizeService(resultOperation);

                        if (resultOperation.IsSuccess)
                        {   //if NextFlow is true and there is not any next step, set next flow data 
                            if (e_ActionType.Done == e_ActionType && !formModel.NextStepID.HasValue)
                            {
                                //Does not send base.UnitOfWork to force NextFlow to make a new instance of UnitOfWork itself. 
                                var result = new ProcessEngine(base.EngineSharedModel).NextFlow(threadTask.ID);
                                resultOperation = result.Item1;
                                codeBaseShared.MessageList.AddRange(result.Item2);
                            }
                        }
                    }
                    resultOperation.CurrentObject = formModel;
                }
            }
            catch (Exception ex)
            {
                return new EngineResponseModel().InitPost(base.ExceptionHandler(ex), codeBaseShared.MessageList, null);
            }

            return new EngineResponseModel().InitPost(resultOperation, codeBaseShared.MessageList, redirectUrlModel, listDownloadModel: codeBaseShared.ListDownloadModel);
        }


        #endregion

    }
}
