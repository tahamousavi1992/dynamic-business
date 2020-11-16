using DynamicBusiness.BPMS.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DynamicBusiness.BPMS.Domain;
using Newtonsoft.Json.Linq;
using DynamicBusiness.BPMS.SharedPresentation;

namespace DynamicBusiness.BPMS.Cartable.Controllers
{
    public class CartableThreadController : BpmsApiControlBase
    {
        // GET: KartableThread
        public object GetIndex(Guid threadTaskID, Guid? stepID = null)
        {
            using (ThreadTaskService threadTaskService = new ThreadTaskService())
            {
                sysBpmsThreadTask threadTask = threadTaskService.GetInfo(threadTaskID, new string[] { nameof(sysBpmsThreadTask.sysBpmsThread) });
                using (ProcessEngine ProcessEngine = new ProcessEngine(new EngineSharedModel(threadTask.ThreadID, threadTask.sysBpmsThread.ProcessID, this.MyRequest.GetList(false, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId)))
                {
                    GetTaskFormResponseModel responseVM = ProcessEngine.GetTaskForm(threadTaskID, stepID);
                    if (responseVM.EngineFormModel != null)
                    {
                        string popUpUrl = UrlUtility.GetCartableApiUrl(base.MyRequest, base.PortalSettings.DefaultPortalAlias, nameof(CartableThreadController.GetPopUp), nameof(CartableThreadController), "", "threadTaskID=" + threadTaskID);
                        string postUrl = UrlUtility.GetCartableApiUrl(base.MyRequest, base.PortalSettings.DefaultPortalAlias, nameof(CartableThreadController.PostIndex), nameof(CartableThreadController), "", UrlUtility.GetParamsAsArray(new HttpRequestWrapper(base.MyRequest), new string[] { $"threadTaskID={threadTaskID}", $"stepID={responseVM.EngineFormModel.FormModel.StepID}" }).ToArray());
                        responseVM.EngineFormModel.SetUrls(popUpUrl, postUrl, new HttpRequestWrapper(base.MyRequest), base.PortalSettings.DefaultPortalAlias, FormTokenUtility.GetFormToken(base.ApiSessionId, responseVM.EngineFormModel?.FormModel?.ContentHtml?.DynamicFormID ?? Guid.Empty, responseVM?.EngineFormModel?.FormModel?.IsEncrypted ?? false));
                    }

                    return new
                    {
                        Model = responseVM?.EngineFormModel,
                        MessageList = responseVM?.ListMessageModel.Select(c => new PostMethodMessage(c.Message, c.DisplayMessageType)),
                        RedirectUrl = base.GetRedirectUrl(responseVM?.RedirectUrlModel),
                        Result = true,
                        ThreadTasks = threadTaskService.GetList(threadTask.ThreadID, (int)sysBpmsTask.e_TypeLU.UserTask, null, null, new string[] { "sysBpmsTask.sysBpmsElement", nameof(sysBpmsThreadTask.sysBpmsUser) }).Select(c => new ThreadHistoryDTO(c)).ToList()
                    };

                }
            }
        }

        // GET: KartableThread
        [HttpGet]
        public object GetPopUp(Guid threadTaskID, Guid formID)
        {
            using (ThreadTaskService threadTaskService = new ThreadTaskService())
            {
                using (DynamicFormService dynamicFormService = new DynamicFormService())
                {
                    sysBpmsThreadTask threadTask = threadTaskService.GetInfo(threadTaskID, new string[] { nameof(sysBpmsThreadTask.sysBpmsThread) });
                    using (ProcessEngine ProcessEngine = new ProcessEngine(new EngineSharedModel(threadTask.ThreadID, threadTask.sysBpmsThread.ProcessID, this.MyRequest.GetList(dynamicFormService.GetInfo(formID).ConfigXmlModel.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId)))
                    {
                        GetTaskFormResponseModel responseVM = ProcessEngine.GetForm(threadTaskID, formID);
                        if (responseVM.EngineFormModel != null)
                        {
                            string popUpUrl = UrlUtility.GetCartableApiUrl(base.MyRequest, base.PortalSettings.DefaultPortalAlias, nameof(CartableThreadController.GetPopUp), nameof(CartableThreadController), "", "threadTaskID=" + threadTaskID);
                            string postUrl = UrlUtility.GetCartableApiUrl(base.MyRequest, base.PortalSettings.DefaultPortalAlias, nameof(CartableThreadController.PostPopUp), nameof(CartableThreadController), "", UrlUtility.GetParamsAsArray(new HttpRequestWrapper(base.MyRequest), new string[] { $"formID={formID}", $"threadTaskID={threadTaskID}", $"stepID={responseVM.EngineFormModel.FormModel.StepID}" }).ToArray());

                            responseVM.EngineFormModel.SetUrls(popUpUrl, postUrl, new HttpRequestWrapper(base.MyRequest), base.PortalSettings.DefaultPortalAlias, FormTokenUtility.GetFormToken(base.ApiSessionId, responseVM.EngineFormModel?.FormModel?.ContentHtml?.DynamicFormID ?? Guid.Empty, responseVM?.EngineFormModel?.FormModel?.IsEncrypted ?? false));
                        }
                        return new
                        {
                            Model = responseVM?.EngineFormModel,
                            MessageList = responseVM?.ListMessageModel.Select(c => new PostMethodMessage(c.Message, c.DisplayMessageType)),
                            RedirectUrl = base.GetRedirectUrl(responseVM?.RedirectUrlModel),
                            Result = true,
                        };
                    }
                }
            }
        }

        [BpmsFormTokenAuth]
        [HttpPost]
        public object PostPopUp(Guid threadTaskID, Guid formID, string controlId = "")
        {
            using (ThreadTaskService threadTaskService = new ThreadTaskService())
            {
                using (DynamicFormService dynamicFormService = new DynamicFormService())
                {
                    sysBpmsThreadTask threadTask = threadTaskService.GetInfo(threadTaskID, new string[] { nameof(sysBpmsThreadTask.sysBpmsThread) });
                    using (ProcessEngine ProcessEngine = new ProcessEngine(new EngineSharedModel(threadTask.ThreadID, threadTask.sysBpmsThread.ProcessID, this.MyRequest.GetList(dynamicFormService.GetInfo(formID).ConfigXmlModel.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId)))
                    {
                        PostTaskFormResponseModel responseVM = ProcessEngine.PostForm(threadTaskID, formID, controlId);

                        if (!responseVM.IsSuccess)
                            return new EngineFormResponseDTO(redirectUrl: base.GetRedirectUrl(responseVM.RedirectUrlModel), reloadForm: false,
                                null, false, messageList: responseVM.ListMessageModel, false);
                        else
                        {
                            if (responseVM.IsSubmit)
                                return new EngineFormResponseDTO(
                                        redirectUrl: base.GetRedirectUrl(responseVM.RedirectUrlModel), reloadForm: false,
                                        listDownloadModel: responseVM.ListDownloadModel,
                                        isSubmit: true,
                                        responseVM.ListMessageModel, true
                                        );
                            else
                                return new EngineFormResponseDTO(
                                        redirectUrl: base.GetRedirectUrl(responseVM.RedirectUrlModel), reloadForm: true,
                                        listDownloadModel: responseVM.ListDownloadModel,
                                        isSubmit: false,
                                        responseVM.ListMessageModel
                                        );
                        }
                    }
                }
            }
        }

        [BpmsFormTokenAuth]
        [HttpPost]
        public object PostIndex(Guid threadTaskID, Guid stepID, bool? goNext = null, string controlId = "")
        {
            using (ThreadTaskService threadTaskService = new ThreadTaskService())
            {
                using (DynamicFormService dynamicFormService = new DynamicFormService())
                {
                    sysBpmsThreadTask threadTask = new ThreadTaskService().GetInfo(threadTaskID, new string[] { nameof(sysBpmsThreadTask.sysBpmsThread) });
                    using (ProcessEngine processEngine = new ProcessEngine(new EngineSharedModel(threadTask.sysBpmsThread, threadTask.sysBpmsThread.ProcessID, this.MyRequest.GetList(dynamicFormService.GetInfoByStepID(stepID).ConfigXmlModel.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId)))
                    {
                        PostTaskFormResponseModel responseVM = processEngine.PostTaskForm(threadTaskID, stepID, goNext, controlId);

                        if (!responseVM.IsSuccess)
                            return new EngineFormResponseDTO(redirectUrl: base.GetRedirectUrl(responseVM.RedirectUrlModel), reloadForm: false,
                                null, false, messageList: responseVM.ListMessageModel, false);
                        else
                        {
                            if (responseVM.IsSubmit)
                            {
                                if (responseVM.IsNextPrevious == true)
                                {
                                    return new EngineFormResponseDTO(
                                            redirectUrl: base.GetRedirectUrl(responseVM.RedirectUrlModel),
                                            reloadForm: true,
                                            listDownloadModel: responseVM.ListDownloadModel,
                                            messageList: responseVM.ListMessageModel
                                            )
                                    {
                                        StepID = responseVM.StepID.Value,
                                    };
                                }
                                else
                                    return new EngineFormResponseDTO(
                                            redirectUrl: string.IsNullOrWhiteSpace(base.GetRedirectUrl(responseVM.RedirectUrlModel)) ? "CartableManage" :
                                             base.GetRedirectUrl(responseVM.RedirectUrlModel), reloadForm: false,
                                            listDownloadModel: responseVM.ListDownloadModel,
                                            messageList: responseVM.ListMessageModel
                                            );
                            }
                            else
                                return new EngineFormResponseDTO(
                                        redirectUrl: base.GetRedirectUrl(responseVM.RedirectUrlModel),
                                        reloadForm: true,
                                        listDownloadModel: responseVM.ListDownloadModel,
                                        messageList: responseVM.ListMessageModel
                                        )
                                {
                                    StepID = responseVM.StepID.Value,
                                };
                        }
                    }
                }
            }
        }

    }
}