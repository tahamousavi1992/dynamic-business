using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using DotNetNuke.Collections;
using DotNetNuke.Security;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Modules;

namespace DynamicBusiness.BPMS.SingleAction.Controllers
{
    public class SingleActionWorkerController : SingleActionApiControllerBase
    {
        public bool IsEncrypted { get { return FormTokenUtility.GetIsEncrypted(base.MyRequest.QueryString[FormTokenUtility.FormToken].ToStringObj(), base.ApiSessionId); } }

        /// <param name="formId">for calling end process form</param>
        /// <param name="threadId">for calling a thread from other pages</param>
        /// <returns></returns>
        [HttpGet]
        public object GetIndex(Guid? threadTaskID = null, Guid? stepID = null, Guid? applicationPageId = null, Guid? formId = null, Guid? threadId = null)
        {
            SingleActionSettingDTO setting = base.GetSetting();
            try
            {
                if (setting.ProcessID.HasValue)
                {
                    #region .:: Thread ::. 
                    //If bpms engine is in different domain.
                    EngineProcessProxy engineProcessProxy = null;
                    if (!string.IsNullOrWhiteSpace(setting.WebApiAddress))
                    {
                        engineProcessProxy = new EngineProcessProxy(setting.WebApiAddress, setting.WebServicePass, base.ClientUserName, ApiUtility.GetIPAddress(), base.ApiSessionId, this.IsEncrypted);
                    }

                    if (!threadTaskID.HasValue && !threadId.HasValue)
                    {
                        //begin Process
                        BeginTaskResponseModel beginTaskResponseVM = null;
                        //If bpms engine is in different domain.
                        if (!string.IsNullOrWhiteSpace(setting.WebApiAddress))
                            beginTaskResponseVM = engineProcessProxy.BeginTask(setting.ProcessID.Value, base.MyRequest.GetList(false, string.Empty).ToList());
                        else
                            beginTaskResponseVM = this.BeginTask(setting.ProcessID.Value);

                        threadTaskID = beginTaskResponseVM.ThreadTaskID;
                        if (!beginTaskResponseVM.Result)
                        {
                            return new
                            {
                                MessageList = new List<PostMethodMessage>() { new PostMethodMessage(beginTaskResponseVM.Message, DisplayMessageType.error) },
                                Result = false,
                                setting.ShowCardBody
                            };
                        }
                    }

                    if (!threadTaskID.HasValue && threadId.HasValue)
                    {
                        //If bpms engine is in different domain.
                        if (!string.IsNullOrWhiteSpace(setting.WebApiAddress))
                            threadTaskID = engineProcessProxy.GetAccessibleThreadTasks(threadId.Value).FirstOrDefault();
                        else
                            threadTaskID = this.GetAccessibleThreadTasks(threadId.Value).FirstOrDefault();

                        if (!threadTaskID.HasValue || threadTaskID == Guid.Empty)
                        {
                            ThreadDetailDTO threadDetailDTO = null;
                            //show history
                            //If bpms engine is in different domain.
                            if (!string.IsNullOrWhiteSpace(setting.WebApiAddress))
                                threadDetailDTO = engineProcessProxy.GetThreadDetails(threadId.Value);
                            else
                            {
                                threadDetailDTO = this.GetThreadDetails(threadId.Value);
                            }
                            return new
                            {
                                ThreadDetailModel = threadDetailDTO,
                                Result = true,
                                setting.ShowCardBody
                            };
                        }
                    }
                    GetTaskFormResponseModel responseVM = null;
                    //If it must load end process form.
                    if (formId.HasValue)
                    {
                        //If bpms engine is in different domain.
                        if (!string.IsNullOrWhiteSpace(setting.WebApiAddress))
                            responseVM = engineProcessProxy.GetForm(threadTaskID.Value, formId.Value, base.MyRequest.GetList(false, string.Empty).ToList(), false);
                        else
                        {
                            //if engine is in same domain, call it directly.
                            using (ThreadTaskService threadTaskService = new ThreadTaskService())
                            {
                                sysBpmsThreadTask threadTask = new ThreadTaskService().GetInfo(threadTaskID.Value, new string[] { nameof(sysBpmsThreadTask.Thread) });
                                using (ProcessEngine ProcessEngine = new ProcessEngine(new EngineSharedModel(threadTask.ThreadID, threadTask.Thread.ProcessID, base.MyRequest.GetList(this.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId)))
                                    responseVM = ProcessEngine.GetForm(threadTask.ID, formId.Value, false);
                            }
                        }

                        if (responseVM.EngineFormModel != null)
                            responseVM.EngineFormModel.FormModel.HasSubmitButton = true;
                    }
                    else
                    {
                        if (threadTaskID.HasValue)
                        {
                            //If bpms engine is in different domain.
                            if (!string.IsNullOrWhiteSpace(setting.WebApiAddress))
                                engineProcessProxy.GetTaskForm(threadTaskID.Value, stepID, base.MyRequest.GetList(false, string.Empty).ToList());
                            else
                            {
                                //If engine is in same domain, call it directly.
                                using (ThreadTaskService threadTaskService = new ThreadTaskService())
                                {
                                    sysBpmsThreadTask threadTask = new ThreadTaskService().GetInfo(threadTaskID.Value, new string[] { nameof(sysBpmsThreadTask.Thread) });
                                    using (ProcessEngine ProcessEngine = new ProcessEngine(new EngineSharedModel(threadTask.ThreadID, threadTask.Thread.ProcessID, base.MyRequest.GetList(this.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId)))
                                        responseVM = ProcessEngine.GetTaskForm(threadTaskID.Value, stepID);
                                }
                            }
                        }
                        else
                            responseVM = null;
                    }
                    if (responseVM?.EngineFormModel != null)
                    {
                        string popUpUrl = UrlUtility.GetSingleActionApiUrl(base.MyRequest, base.TabModuleID, base.PortalSettings.DefaultPortalAlias, nameof(SingleActionWorkerController.GetPopUp), nameof(SingleActionWorkerController), "", "threadTaskID=" + threadTaskID);
                        string postUrl = UrlUtility.GetSingleActionApiUrl(base.MyRequest, base.TabModuleID, base.PortalSettings.DefaultPortalAlias, nameof(SingleActionWorkerController.PostIndex), nameof(SingleActionWorkerController), "", UrlUtility.GetParamsAsArray(new HttpRequestWrapper(base.MyRequest), new string[] { $"threadTaskID={threadTaskID}", $"stepID={responseVM.EngineFormModel.FormModel.StepID}" }).ToArray());

                        //If bpms engine is in different domain.
                        if (!string.IsNullOrWhiteSpace(setting.WebApiAddress))
                            responseVM.EngineFormModel.SetUrlsForSingleAction(base.PortalSettings.DefaultPortalAlias, new HttpRequestWrapper(base.MyRequest), popUpUrl, postUrl, FormTokenUtility.GetFormToken(base.ApiSessionId, responseVM?.EngineFormModel?.FormModel?.ContentHtml?.DynamicFormID ?? Guid.Empty, responseVM?.EngineFormModel?.FormModel?.IsEncrypted ?? false), base.TabModuleID);
                        else
                            responseVM.EngineFormModel.SetUrls(popUpUrl, postUrl, new HttpRequestWrapper(base.MyRequest), base.PortalSettings.DefaultPortalAlias, FormTokenUtility.GetFormToken(base.ApiSessionId, responseVM?.EngineFormModel?.FormModel?.ContentHtml?.DynamicFormID ?? Guid.Empty, responseVM?.EngineFormModel?.FormModel?.IsEncrypted ?? false));

                        return new
                        {
                            Model = responseVM?.EngineFormModel,
                            MessageList = responseVM?.ListMessageModel.Select(c => new PostMethodMessage(c.Message, c.DisplayMessageType)),
                            RedirectUrl = base.GetRedirectUrl(responseVM?.RedirectUrlModel),
                            Result = true,
                            setting.ShowCardBody
                        };
                    }
                    else
                        return new
                        {
                            MessageList = new List<PostMethodMessage>() { new PostMethodMessage("Error in getting information", DisplayMessageType.error) },
                            Result = false,
                            setting.ShowCardBody
                        };

                    #endregion
                }
                else
                {
                    #region .:: Application Page ::.
                    applicationPageId = applicationPageId ?? setting.ApplicationPageID;
                    GetFormResponseModel responseVM = null;

                    //if bpms engine is in different domain
                    if (!string.IsNullOrWhiteSpace(setting.WebApiAddress))
                    {
                        EngineApplicationProxy engineApplicationProxy = new EngineApplicationProxy(setting.WebApiAddress, setting.WebServicePass, base.ClientUserName, ApiUtility.GetIPAddress(), base.ApiSessionId, this.IsEncrypted);
                        responseVM = engineApplicationProxy.GetForm(applicationPageId, null, base.MyRequest.GetList(false, string.Empty).ToList());
                    }
                    else
                    {
                        EngineSharedModel engineSharedModel = new EngineSharedModel(applicationPageId.Value, base.MyRequest.GetList(this.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId);
                        using (ApplicationPageEngine applicationPageEngine = new ApplicationPageEngine(engineSharedModel))
                            responseVM = applicationPageEngine.GetForm();
                    }
                    if (responseVM?.EngineFormModel != null)
                    {
                        string popUpUrl = UrlUtility.GetSingleActionApiUrl(base.MyRequest, base.TabModuleID, base.PortalSettings.DefaultPortalAlias, nameof(SingleActionWorkerController.GetPopUp), nameof(SingleActionWorkerController), "");
                        string postUrl = UrlUtility.GetSingleActionApiUrl(base.MyRequest, base.TabModuleID, base.PortalSettings.DefaultPortalAlias, nameof(SingleActionWorkerController.PostIndex), nameof(SingleActionWorkerController), "", UrlUtility.GetParamsAsArray(new HttpRequestWrapper(base.MyRequest), new string[] { $"applicationPageId={applicationPageId}" }).ToArray());

                        if (!string.IsNullOrWhiteSpace(setting.WebApiAddress))
                            responseVM.EngineFormModel.SetUrlsForSingleAction(base.PortalSettings.DefaultPortalAlias, new HttpRequestWrapper(base.MyRequest), popUpUrl, postUrl, FormTokenUtility.GetFormToken(base.ApiSessionId, responseVM?.EngineFormModel?.FormModel?.ContentHtml?.DynamicFormID ?? Guid.Empty, responseVM?.EngineFormModel?.FormModel?.IsEncrypted ?? false), base.TabModuleID);
                        else
                            responseVM.EngineFormModel.SetUrls(popUpUrl, postUrl, new HttpRequestWrapper(base.MyRequest), base.PortalSettings.DefaultPortalAlias, FormTokenUtility.GetFormToken(base.ApiSessionId, responseVM?.EngineFormModel?.FormModel?.ContentHtml?.DynamicFormID ?? Guid.Empty, responseVM?.EngineFormModel?.FormModel?.IsEncrypted ?? false));

                        return new
                        {
                            Model = responseVM?.EngineFormModel,
                            MessageList = responseVM?.ListMessageModel.Select(c => new PostMethodMessage(c.Message, c.DisplayMessageType)),
                            RedirectUrl = base.GetRedirectUrl(responseVM?.RedirectUrlModel),
                            Result = true,
                            setting.ShowCardBody
                        };
                    }
                    else
                        return new
                        {
                            MessageList = new List<PostMethodMessage>() { new PostMethodMessage("Error while getting information", DisplayMessageType.error) },
                            Result = false,
                            setting.ShowCardBody
                        };
                    #endregion
                }
            }
            catch
            {
                return new
                {
                    MessageList = new List<PostMethodMessage>() { new PostMethodMessage("Setting is not complete", DisplayMessageType.error) },
                    Result = false,
                    setting.ShowCardBody
                };
            }
        }

        [HttpGet]
        public object GetPopUp(Guid formID, Guid? threadTaskID = null)
        {
            SingleActionSettingDTO setting = base.GetSetting();
            if (setting.ProcessID.HasValue)
            {
                #region .:: Thread ::.
                GetTaskFormResponseModel responseVM = null;
                //If bpms engine is in different domain.
                if (!string.IsNullOrWhiteSpace(setting.WebApiAddress))
                    responseVM = new EngineProcessProxy(setting.WebApiAddress, setting.WebServicePass, base.ClientUserName, ApiUtility.GetIPAddress(), base.ApiSessionId, this.IsEncrypted).GetForm(threadTaskID.Value, formID, base.MyRequest.GetList(false, string.Empty).ToList());
                else
                {
                    //if engine is in same domain, call it directly.
                    using (ThreadTaskService threadTaskService = new ThreadTaskService())
                    {
                        sysBpmsThreadTask threadTask = new ThreadTaskService().GetInfo(threadTaskID.Value, new string[] { nameof(sysBpmsThreadTask.Thread) });
                        using (ProcessEngine ProcessEngine = new ProcessEngine(new EngineSharedModel(threadTask.ThreadID, threadTask.Thread.ProcessID, base.MyRequest.GetList(this.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId)))
                            responseVM = ProcessEngine.GetForm(threadTask.ID, formID, null);
                    }
                }
                if (responseVM.EngineFormModel != null)
                {
                    string popUpUrl = UrlUtility.GetSingleActionApiUrl(base.MyRequest, base.TabModuleID, base.PortalSettings.DefaultPortalAlias, nameof(SingleActionWorkerController.GetPopUp), nameof(SingleActionWorkerController), "", "threadTaskID=" + threadTaskID);
                    string postUrl = UrlUtility.GetSingleActionApiUrl(base.MyRequest, base.TabModuleID, base.PortalSettings.DefaultPortalAlias, nameof(SingleActionWorkerController.PostPopUp), nameof(SingleActionWorkerController), "", UrlUtility.GetParamsAsArray(new HttpRequestWrapper(base.MyRequest), new string[] { $"formID={formID}", $"threadTaskID={threadTaskID}", $"stepID={responseVM.EngineFormModel.FormModel.StepID}" }).ToArray());

                    if (!string.IsNullOrWhiteSpace(setting.WebApiAddress))
                        responseVM.EngineFormModel.SetUrlsForSingleAction(base.PortalSettings.DefaultPortalAlias, new HttpRequestWrapper(base.MyRequest), popUpUrl, postUrl, FormTokenUtility.GetFormToken(base.ApiSessionId, responseVM?.EngineFormModel?.FormModel?.ContentHtml?.DynamicFormID ?? Guid.Empty, responseVM?.EngineFormModel?.FormModel?.IsEncrypted ?? false), base.TabModuleID);
                    else
                        responseVM.EngineFormModel.SetUrls(popUpUrl, postUrl, new HttpRequestWrapper(base.MyRequest), base.PortalSettings.DefaultPortalAlias, FormTokenUtility.GetFormToken(base.ApiSessionId, responseVM?.EngineFormModel?.FormModel?.ContentHtml?.DynamicFormID ?? Guid.Empty, responseVM?.EngineFormModel?.FormModel?.IsEncrypted ?? false));

                }
                return new
                {
                    Model = responseVM?.EngineFormModel,
                    MessageList = responseVM?.ListMessageModel.Select(c => new PostMethodMessage(c.Message, c.DisplayMessageType)),
                    RedirectUrl = base.GetRedirectUrl(responseVM?.RedirectUrlModel),
                    Result = true,
                };
                #endregion
            }
            else
            {
                #region .:: Application ::.
                GetFormResponseModel responseVM = null;
                //if bpms engine is in different domain
                if (!string.IsNullOrWhiteSpace(setting.WebApiAddress))
                {
                    EngineApplicationProxy engineApplicationProxy = new EngineApplicationProxy(setting.WebApiAddress, setting.WebServicePass, base.ClientUserName, ApiUtility.GetIPAddress(), base.ApiSessionId, this.IsEncrypted);
                    responseVM = engineApplicationProxy.GetForm(null, formID, new HttpRequestWrapper(base.MyRequest).GetList(false, string.Empty).ToList());
                }
                else
                {
                    using (DynamicFormService dynamicFormService = new DynamicFormService())
                    {
                        EngineSharedModel engineSharedModel = new EngineSharedModel(dynamicFormService.GetInfo(formID).ApplicationPageID.Value, base.MyRequest.GetList(false, string.Empty).ToList(), base.ClientUserName, base.ApiSessionId);
                        using (ApplicationPageEngine applicationPageEngine = new ApplicationPageEngine(engineSharedModel))
                            responseVM = applicationPageEngine.GetForm();
                    }
                }

                if (responseVM.EngineFormModel != null)
                {
                    string popUpUrl = UrlUtility.GetSingleActionApiUrl(base.MyRequest, base.TabModuleID, base.PortalSettings.DefaultPortalAlias, nameof(SingleActionWorkerController.GetPopUp), nameof(SingleActionWorkerController), "");
                    string postUrl = UrlUtility.GetSingleActionApiUrl(base.MyRequest, base.TabModuleID, base.PortalSettings.DefaultPortalAlias, nameof(SingleActionWorkerController.PostPopUp), nameof(SingleActionWorkerController), "", UrlUtility.GetParamsAsArray(new HttpRequestWrapper(base.MyRequest), new string[] { $"applicationPageId={responseVM.EngineFormModel.ApplicationID}" }).ToArray());
                    if (!string.IsNullOrWhiteSpace(setting.WebApiAddress))
                        responseVM.EngineFormModel.SetUrlsForSingleAction(base.PortalSettings.DefaultPortalAlias, new HttpRequestWrapper(base.MyRequest), popUpUrl, postUrl, FormTokenUtility.GetFormToken(base.ApiSessionId, responseVM?.EngineFormModel?.FormModel?.ContentHtml?.DynamicFormID ?? Guid.Empty, responseVM?.EngineFormModel?.FormModel?.IsEncrypted ?? false), base.TabModuleID);
                    else
                        responseVM.EngineFormModel.SetUrls(popUpUrl, postUrl, new HttpRequestWrapper(base.MyRequest), base.PortalSettings.DefaultPortalAlias, FormTokenUtility.GetFormToken(base.ApiSessionId, responseVM?.EngineFormModel?.FormModel?.ContentHtml?.DynamicFormID ?? Guid.Empty, responseVM?.EngineFormModel?.FormModel?.IsEncrypted ?? false));

                }
                return new
                {
                    Model = responseVM?.EngineFormModel,
                    MessageList = responseVM?.ListMessageModel.Select(c => new PostMethodMessage(c.Message, c.DisplayMessageType)),
                    RedirectUrl = base.GetRedirectUrl(responseVM?.RedirectUrlModel),
                    Result = true,
                };
                #endregion
            }
        }

        [HttpPost]
        public object PostIndex(Guid? applicationPageId = null, Guid? threadTaskID = null, string controlId = "", Guid? stepID = null, bool? goNext = null)
        {
            SingleActionSettingDTO setting = base.GetSetting();
            if (setting.ProcessID.HasValue)
            {
                #region .:: Thread ::.
                PostTaskFormResponseModel responseVM = null;
                //If bpms engine is in different domain.
                if (!string.IsNullOrWhiteSpace(setting.WebApiAddress))
                {
                    EngineProcessProxy engineProcessProxy = new EngineProcessProxy(setting.WebApiAddress, setting.WebServicePass, base.ClientUserName, ApiUtility.GetIPAddress(), base.ApiSessionId, this.IsEncrypted);
                    responseVM = engineProcessProxy.PostTaskForm(threadTaskID.Value, controlId, stepID.Value, goNext, base.MyRequest.GetList(false, string.Empty).ToList());
                }
                else
                {
                    //If engine is in same domain, call it directly.
                    using (ThreadTaskService threadTaskService = new ThreadTaskService())
                    {
                        sysBpmsThreadTask threadTask = threadTaskService.GetInfo(threadTaskID.Value, new string[] { nameof(sysBpmsThreadTask.Thread) });
                        using (ProcessEngine processEngine = new ProcessEngine(new EngineSharedModel(threadTask.Thread, threadTask.Thread.ProcessID, base.MyRequest.GetList(this.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId)))
                        {
                            responseVM = processEngine.PostTaskForm(threadTask.ID, stepID.Value, goNext, controlId);
                        }
                    }
                }

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
                                        base.GetRedirectUrl(responseVM.RedirectUrlModel), reloadForm: setting.ProcessEndFormID.HasValue,
                                       listDownloadModel: responseVM.ListDownloadModel,
                                       messageList: responseVM.ListMessageModel
                                       )
                            {
                                EndAppPageID = setting.ProcessEndFormID,
                                StepID = responseVM?.StepID,
                            };
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
                #endregion
            }
            else
            {
                #region .:: Application ::.
                PostFormResponseModel responseVM = null;

                //if bpms engine is in different domain
                if (!string.IsNullOrWhiteSpace(setting.WebApiAddress))
                {
                    EngineApplicationProxy engineApplicationProxy = new EngineApplicationProxy(setting.WebApiAddress, setting.WebServicePass, base.ClientUserName, ApiUtility.GetIPAddress(), base.ApiSessionId, this.IsEncrypted);
                    responseVM = engineApplicationProxy.PostForm(applicationPageId.Value, controlId, base.MyRequest.GetList(false, string.Empty).ToList());
                }
                else
                {
                    EngineSharedModel engineSharedModel = new EngineSharedModel(applicationPageId.Value, base.MyRequest.GetList(this.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId);
                    using (ApplicationPageEngine applicationPageEngine = new ApplicationPageEngine(engineSharedModel))
                        responseVM = applicationPageEngine.PostForm(controlId);
                }

                if (!responseVM.IsSuccess)
                    return new EngineFormResponseDTO(base.GetRedirectUrl(responseVM.RedirectUrlModel), false, null, false, responseVM?.ListMessageModel, false);
                else
                {
                    return
                        new EngineFormResponseDTO(
                        redirectUrl: base.GetRedirectUrl(responseVM.RedirectUrlModel),
                        reloadForm: false,
                        listDownloadModel: responseVM.ListDownloadModel, isSubmit: responseVM.IsSubmit,
                        responseVM.ListMessageModel, submittedHtmlMessage: setting.AppPageSubmitMessage
                         );
                }
                #endregion
            }
        }

        [HttpPost]
        public object PostPopUp(Guid? applicationPageId = null, Guid? threadTaskID = null, Guid? formID = null, string controlId = "")
        {
            SingleActionSettingDTO setting = base.GetSetting();
            if (setting.ProcessID.HasValue)
            {
                #region .:: Thread ::.
                PostTaskFormResponseModel responseVM = null;

                //If bpms engine is in different domain.
                if (!string.IsNullOrWhiteSpace(setting.WebApiAddress))
                {
                    EngineProcessProxy engineProcessProxy = new EngineProcessProxy(setting.WebApiAddress, setting.WebServicePass, base.ClientUserName, ApiUtility.GetIPAddress(), base.ApiSessionId, this.IsEncrypted);
                    responseVM = engineProcessProxy.PostForm(threadTaskID.Value, formID.Value, controlId, base.MyRequest.GetList(false, string.Empty).ToList());
                }
                else
                {

                }

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
                #endregion
            }
            else
            {
                #region .:: Application ::.
                PostFormResponseModel responseVM = null;
                //if bpms engine is in different domain
                if (!string.IsNullOrWhiteSpace(setting.WebApiAddress))
                {
                    EngineApplicationProxy engineApplicationProxy = new EngineApplicationProxy(setting.WebApiAddress, setting.WebServicePass, base.ClientUserName, ApiUtility.GetIPAddress(), base.ApiSessionId, this.IsEncrypted);
                    responseVM = engineApplicationProxy.PostForm(applicationPageId.Value, controlId, base.MyRequest.GetList(false, string.Empty).ToList());
                }
                else
                {
                    EngineSharedModel engineSharedModel = new EngineSharedModel(applicationPageId.Value, base.MyRequest.GetList(this.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId);
                    using (ApplicationPageEngine applicationPageEngine = new ApplicationPageEngine(engineSharedModel))
                        responseVM = applicationPageEngine.PostForm(controlId);
                }

                if (!responseVM.IsSuccess)
                    return new EngineFormResponseDTO(
                        redirectUrl: base.GetRedirectUrl(responseVM.RedirectUrlModel), reloadForm: false, null,
                        isSubmit: false, responseVM.ListMessageModel, false);
                else
                {
                    if (responseVM.IsSubmit)
                        return new EngineFormResponseDTO(
                        redirectUrl: base.GetRedirectUrl(responseVM.RedirectUrlModel), reloadForm: false,
                        listDownloadModel: responseVM.ListDownloadModel, isSubmit: true,
                        responseVM.ListMessageModel
                        );
                    else
                        return new EngineFormResponseDTO(
                               base.GetRedirectUrl(responseVM.RedirectUrlModel), reloadForm: true,
                               responseVM.ListDownloadModel, isSubmit: false,
                               responseVM.ListMessageModel, true
                              );
                }
                #endregion
            }
        }

        private ThreadDetailDTO GetThreadDetails(Guid threadId)
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
                        using (ProcessEngine processEngine = new ProcessEngine(new EngineSharedModel(threadId, threadDetailDTO.ProcessID, base.MyRequest.GetList(false, string.Empty).ToList(), base.ClientUserName, base.ApiSessionId)))
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

        private BeginTaskResponseModel BeginTask(Guid processID)
        {
            List<QueryModel> baseQueryModel = base.MyRequest.GetList(false, string.Empty).ToList();
            using (ProcessEngine processEngine = new ProcessEngine(new EngineSharedModel(currentThread: null, currentProcessID: processID, baseQueryModel: baseQueryModel, currentUserName: base.ClientUserName, apiSessionId: base.ApiSessionId)))
            {
                using (ThreadTaskService threadTaskService = new ThreadTaskService())
                {
                    using (UserService userService = new UserService())
                    {
                        var result = processEngine.StartProcess(userService.GetInfo(base.ClientUserName)?.ID);
                        if (result.Item1.IsSuccess)
                        {
                            sysBpmsThreadTask threadTask = threadTaskService.GetList(((sysBpmsThread)result.Item1.CurrentObject).ID, (int)sysBpmsTask.e_TypeLU.UserTask, null, (int)sysBpmsThreadTask.e_StatusLU.New).LastOrDefault();
                            return new BeginTaskResponseModel(string.Join(",", result.Item2), true, ((sysBpmsThread)result.Item1.CurrentObject)?.ID, threadTask?.ID); ;
                        }
                        else
                        {
                            return new BeginTaskResponseModel(result.Item1.GetErrors(), false, null, null);
                        }
                    }
                }
            }
        }

        private List<Guid> GetAccessibleThreadTasks(Guid threadId)
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
    }
}