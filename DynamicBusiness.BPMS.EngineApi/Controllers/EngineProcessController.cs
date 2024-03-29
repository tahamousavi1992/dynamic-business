﻿using DotNetNuke.Web.Api;
using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace DynamicBusiness.BPMS.EngineApi.Controllers
{
    public class EngineProcessController : BpmsEngineApiControlBase
    {
        [BpmsAuth]
        [HttpPost]
        public BeginTaskResponseModel BeginTask(Guid processID)
        {
            List<QueryModel> baseQueryModel = base.MyRequest?.GetList(base.IsEncrypted, base.ApiSessionId).ToList();
            using (ProcessEngine processEngine = new ProcessEngine(new EngineSharedModel(currentThread: null, currentProcessID: processID, baseQueryModel: baseQueryModel, currentUserName: base.ClientUserName, apiSessionId: base.ApiSessionId)))
            {
                using (ThreadTaskService threadTaskService = new ThreadTaskService())
                {
                    using (UserService userService = new UserService())
                    {
                        (ResultOperation result, List<MessageModel> msgModel) = processEngine.StartProcess(userService.GetInfo(base.ClientUserName)?.ID);
                        if (result.IsSuccess)
                        {
                            sysBpmsThreadTask threadTask = threadTaskService.GetList(((sysBpmsThread)result.CurrentObject).ID, (int)sysBpmsTask.e_TypeLU.UserTask, null, (int)sysBpmsThreadTask.e_StatusLU.New).LastOrDefault();
                            return new BeginTaskResponseModel(string.Join(",", msgModel), true, ((sysBpmsThread)result.CurrentObject)?.ID, threadTask?.ID); ;
                        }
                        else
                        {
                            return new BeginTaskResponseModel(result.GetErrors(), false, null, null);
                        }
                    }
                }
            }
        }

        [BpmsAuth]
        [HttpGet]
        public GetTaskFormResponseModel GetTaskForm(Guid threadTaskID, Guid? stepID = null)
        {
            using (ThreadTaskService threadTaskService = new ThreadTaskService())
            {
                sysBpmsThreadTask threadTask = threadTaskService.GetInfo(threadTaskID, new string[] { nameof(sysBpmsThreadTask.Thread) });
                using (ProcessEngine processEngine = new ProcessEngine(new EngineSharedModel(threadTask.ThreadID, threadTask.Thread.ProcessID, base.MyRequest.GetList(base.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId)))
                {
                    return processEngine.GetTaskForm(threadTaskID, stepID);
                }
            }
        }

        /// <summary>
        /// it is called when a pop up should be open.
        /// </summary>
        [BpmsAuth]
        [HttpGet]
        public GetTaskFormResponseModel GetForm(Guid threadTaskID, Guid formID, bool? cAccess = null)
        {
            using (ThreadTaskService threadTaskService = new ThreadTaskService())
            {
                List<QueryModel> baseQueryModel = base.MyRequest.GetList(base.IsEncrypted, base.ApiSessionId).ToList();
                sysBpmsThreadTask threadTask = new ThreadTaskService().GetInfo(threadTaskID, new string[] { nameof(sysBpmsThreadTask.Thread) });
                using (ProcessEngine processEngine = new ProcessEngine(new EngineSharedModel(threadTask.ThreadID, threadTask.Thread.ProcessID, baseQueryModel, base.ClientUserName, base.ApiSessionId)))
                {
                    return processEngine.GetForm(threadTaskID, formID, cAccess);
                }
            }
        }

        /// <summary>
        /// It is called when threadTask main form with steps post to server.
        /// </summary>
        [BpmsAuth]
        [HttpPost]
        public PostTaskFormResponseModel PostTaskForm(Guid threadTaskID, Guid stepID, bool? goNext = null, string controlId = "")
        {
            using (ThreadTaskService threadTaskService = new ThreadTaskService())
            {
                sysBpmsThreadTask threadTask = threadTaskService.GetInfo(threadTaskID, new string[] { nameof(sysBpmsThreadTask.Thread) });
                using (ProcessEngine processEngine = new ProcessEngine(new EngineSharedModel(threadTask.Thread, threadTask.Thread.ProcessID, base.MyRequest.GetList(base.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId)))
                {
                    return processEngine.PostTaskForm(threadTaskID, stepID, goNext, controlId);
                }
            }
        }
        /// <summary>
        /// It is called when threadTask pop up form post to server.
        /// </summary>

        [BpmsAuth]
        [HttpPost]
        public PostTaskFormResponseModel PostForm(Guid threadTaskID, Guid formID, string controlId = "")
        {
            using (ThreadTaskService threadTaskService = new ThreadTaskService())
            {
                sysBpmsThreadTask threadTask = threadTaskService.GetInfo(threadTaskID, new string[] { nameof(sysBpmsThreadTask.Thread) });
                using (ProcessEngine processEngine = new ProcessEngine(new EngineSharedModel(threadTask.ThreadID, threadTask.Thread.ProcessID, base.MyRequest.GetList(base.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId)))
                {
                    return processEngine.PostForm(threadTaskID, formID, controlId);
                }
            }
        }

        [BpmsAuth]
        [HttpGet]
        public List<ComboSearchItem> GetList(string query = "")
        {
            using (ProcessService processService = new ProcessService())
            {
                return processService.GetList(null, null, null, null)
                    .Where(c => string.IsNullOrWhiteSpace(query) || c.Name.Contains(query))
                    .Select(c => new ComboSearchItem(c.ID.ToString(), c.Name)).ToList();
            }
        }
 
        [BpmsAuth]
        [HttpGet]
        public List<Guid> GetAccessibleThreadTasks(Guid threadId)
        {
            List<Guid> listItems = new List<Guid>();
            using (ThreadTaskService threadTaskService = new ThreadTaskService())
            {
                List<sysBpmsThreadTask> listThreadTask = threadTaskService.GetListRunning(threadId);
                foreach (sysBpmsThreadTask item in listThreadTask)
                {
                    using (UserService userService = new UserService())
                        if (threadTaskService.CheckAccess(item.ID, userService.GetInfo(base.ClientUserName)?.ID, item.Task.ProcessID, false, true).Item1)
                            listItems.Add(item.ID);
                }
            }
            return listItems;
        }

        [BpmsAuth]
        [HttpGet]
        public ThreadDetailDTO GetThreadDetails(Guid threadId)
        {
            using (ThreadService threadService = new ThreadService())
            {
                using (ThreadTaskService threadTaskService = new ThreadTaskService())
                {
                    using (DynamicFormService dynamicFormService = new DynamicFormService())
                    {
                        ThreadDetailDTO threadDetailDTO = new ThreadDetailDTO(
                          threadService.GetInfo(threadId,
                          new string[] { nameof(sysBpmsThread.User), nameof(sysBpmsThread.Process) }),
                          threadTaskService.GetList(threadId, (int)sysBpmsTask.e_TypeLU.UserTask, null, null, new string[] { $"{nameof(sysBpmsThreadTask.Task)}.{nameof(sysBpmsThreadTask.Task.Element)}", nameof(sysBpmsThreadTask.User) }).Select(c => new ThreadHistoryDTO(c)).ToList());

                        List<sysBpmsDynamicForm> listForms = dynamicFormService.GetList(threadDetailDTO.ProcessID, null, null, "", true, null);
                        using (ProcessEngine processEngine = new ProcessEngine(new EngineSharedModel(threadId, threadDetailDTO.ProcessID, this.MyRequest.GetList(false, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId)))
                        {
                            foreach (var item in listForms)
                            {
                                var result = processEngine.GetContentHtmlByFormID(item.ID, true);
                                EngineFormModel engineFormModel = new EngineFormModel(result.FormModel, threadId, null, threadDetailDTO.ProcessID);
                                engineFormModel.GetPopUpUrl = string.Empty;

                                threadDetailDTO.ListOverviewForms.Add(engineFormModel);
                            }
                        }
                        return threadDetailDTO;
                    }
                }
            }
        }

        [BpmsAuth]
        [HttpGet]
        public ProcessDTO GetInfo(Guid processId)
        {
            using (ProcessService processService = new ProcessService())
            {
                return new ProcessDTO(processService.GetInfo(processId));
            }
        }

    }
}