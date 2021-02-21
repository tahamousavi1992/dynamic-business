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

namespace DynamicBusiness.BPMS.SingleAction.Controllers
{
    public class SingleActionWorkerController : SingleActionApiControllerBase
    {

        public bool IsEncrypted { get { return FormTokenUtility.GetIsEncrypted(base.MyRequest.QueryString[FormTokenUtility.FormToken].ToStringObj(), base.ApiSessionId); } }


        /// <param name="formId">for calling end process form</param>
        /// <param name="threadId">for calling a thread from other pages</param>
        /// <returns></returns>
        public object GetIndex(Guid? threadTaskID = null, Guid? stepID = null, Guid? applicationPageId = null, Guid? formId = null, Guid? threadId = null)
        {
            try
            {
                if (SingleActionHomeController.Setting.ProcessID.HasValue)
                {
                    #region .:: Thread ::.
                    EngineProcessProxy engineProcessProxy = new EngineProcessProxy(SingleActionHomeController.Setting.WebApiAddress, SingleActionHomeController.Setting.WebServicePass, base.ClientUserName, ApiUtility.GetIPAddress(), base.ApiSessionId, this.IsEncrypted);
                    if (!threadTaskID.HasValue && !threadId.HasValue)
                    {
                        //begin Process
                        BeginTaskResponseModel beginTaskResponseVM = engineProcessProxy.BeginTask(SingleActionHomeController.Setting.ProcessID.Value, this.MyRequest.GetList(false, string.Empty).ToList());
                        threadTaskID = beginTaskResponseVM.ThreadTaskID;
                        if (!beginTaskResponseVM.Result)
                        {
                            return new
                            {
                                MessageList = new List<PostMethodMessage>() { new PostMethodMessage(beginTaskResponseVM.Message, DisplayMessageType.error) },
                                Result = false,
                                SingleActionHomeController.Setting.ShowCardBody
                            };
                        }
                    }
                    if (!threadTaskID.HasValue && threadId.HasValue)
                    {
                        threadTaskID = engineProcessProxy.GetAccessibleThreadTasks(threadId.Value).FirstOrDefault();
                        if (!threadTaskID.HasValue || threadTaskID == Guid.Empty)
                        {
                            //show history
                            ThreadDetailDTO threadDetailDTO = engineProcessProxy.GetThreadDetails(threadId.Value);
                            return new
                            {
                                ThreadDetailModel = threadDetailDTO,
                                Result = true,
                                SingleActionHomeController.Setting.ShowCardBody
                            };
                        }
                    }
                    GetTaskFormResponseModel responseVM;
                    if (formId.HasValue)
                    {
                        responseVM = engineProcessProxy.GetForm(threadTaskID.Value, formId.Value, this.MyRequest.GetList(false, string.Empty).ToList(), false);
                        if (responseVM.EngineFormModel != null)
                            responseVM.EngineFormModel.FormModel.HasSubmitButton = true;
                    }
                    else
                    {
                        responseVM = threadTaskID.HasValue ?
                             engineProcessProxy.GetTaskForm(threadTaskID.Value, stepID, this.MyRequest.GetList(false, string.Empty).ToList()) : null;
                    }
                    if (responseVM?.EngineFormModel != null)
                    {
                        string popUpUrl = UrlUtility.GetSingleActionApiUrl(base.MyRequest, base.PortalSettings.DefaultPortalAlias, nameof(SingleActionWorkerController.GetPopUp), nameof(SingleActionWorkerController), "", "threadTaskID=" + threadTaskID);
                        string postUrl = UrlUtility.GetSingleActionApiUrl(base.MyRequest, base.PortalSettings.DefaultPortalAlias, nameof(SingleActionWorkerController.PostIndex), nameof(SingleActionWorkerController), "", UrlUtility.GetParamsAsArray(new HttpRequestWrapper(base.MyRequest), new string[] { $"threadTaskID={threadTaskID}", $"stepID={responseVM.EngineFormModel.FormModel.StepID}" }).ToArray());

                        responseVM.EngineFormModel.SetUrlsFromOtherSite(base.PortalSettings.DefaultPortalAlias, new HttpRequestWrapper(base.MyRequest), popUpUrl, postUrl, FormTokenUtility.GetFormToken(base.ApiSessionId, responseVM?.EngineFormModel?.FormModel?.ContentHtml?.DynamicFormID ?? Guid.Empty, responseVM?.EngineFormModel?.FormModel?.IsEncrypted ?? false));

                        return new
                        {
                            Model = responseVM?.EngineFormModel,
                            MessageList = responseVM?.ListMessageModel.Select(c => new PostMethodMessage(c.Message, c.DisplayMessageType)),
                            RedirectUrl = base.GetRedirectUrl(responseVM?.RedirectUrlModel),
                            Result = true,
                            SingleActionHomeController.Setting.ShowCardBody
                        };
                    }
                    else
                        return new
                        {
                            MessageList = new List<PostMethodMessage>() { new PostMethodMessage("Error in getting information", DisplayMessageType.error) },
                            Result = false,
                            SingleActionHomeController.Setting.ShowCardBody
                        };

                    #endregion
                }
                else
                {
                    #region .:: Application Page ::.
                    EngineApplicationProxy engineApplicationProxy = new EngineApplicationProxy(SingleActionHomeController.Setting.WebApiAddress, SingleActionHomeController.Setting.WebServicePass, base.ClientUserName, ApiUtility.GetIPAddress(), base.ApiSessionId, this.IsEncrypted);
                    applicationPageId = applicationPageId ?? SingleActionHomeController.Setting.ApplicationPageID;
                    GetFormResponseModel responseVM = engineApplicationProxy.GetForm(applicationPageId, null, this.MyRequest.GetList(false, string.Empty).ToList());
                    if (responseVM?.EngineFormModel != null)
                    {
                        string popUpUrl = UrlUtility.GetSingleActionApiUrl(base.MyRequest, base.PortalSettings.DefaultPortalAlias, nameof(SingleActionWorkerController.GetPopUp), nameof(SingleActionWorkerController), "");
                        string postUrl = UrlUtility.GetSingleActionApiUrl(base.MyRequest, base.PortalSettings.DefaultPortalAlias, nameof(SingleActionWorkerController.PostIndex), nameof(SingleActionWorkerController), "", UrlUtility.GetParamsAsArray(new HttpRequestWrapper(base.MyRequest), new string[] { $"applicationPageId={applicationPageId}" }).ToArray());

                        responseVM.EngineFormModel.SetUrlsFromOtherSite(base.PortalSettings.DefaultPortalAlias, new HttpRequestWrapper(base.MyRequest), popUpUrl, postUrl, FormTokenUtility.GetFormToken(base.ApiSessionId, responseVM?.EngineFormModel?.FormModel?.ContentHtml?.DynamicFormID ?? Guid.Empty, responseVM?.EngineFormModel?.FormModel?.IsEncrypted ?? false));

                        return new
                        {
                            Model = responseVM?.EngineFormModel,
                            MessageList = responseVM?.ListMessageModel.Select(c => new PostMethodMessage(c.Message, c.DisplayMessageType)),
                            RedirectUrl = base.GetRedirectUrl(responseVM?.RedirectUrlModel),
                            Result = true,
                            SingleActionHomeController.Setting.ShowCardBody
                        };
                    }
                    else
                        return new
                        {
                            MessageList = new List<PostMethodMessage>() { new PostMethodMessage("Error while getting information", DisplayMessageType.error) },
                            Result = false,
                            SingleActionHomeController.Setting.ShowCardBody
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
                    SingleActionHomeController.Setting.ShowCardBody
                };
            }
        }

        [HttpGet]
        public object GetPopUp(Guid formID, Guid? threadTaskID = null)
        {
            if (SingleActionHomeController.Setting.ProcessID.HasValue)
            {
                #region .:: Thread ::.
                GetTaskFormResponseModel responseVM = new EngineProcessProxy(SingleActionHomeController.Setting.WebApiAddress, SingleActionHomeController.Setting.WebServicePass, base.ClientUserName, ApiUtility.GetIPAddress(), base.ApiSessionId, this.IsEncrypted).GetForm(threadTaskID.Value, formID, this.MyRequest.GetList(false, string.Empty).ToList());
                if (responseVM.EngineFormModel != null)
                {
                    string popUpUrl = UrlUtility.GetSingleActionApiUrl(base.MyRequest, base.PortalSettings.DefaultPortalAlias, nameof(SingleActionWorkerController.GetPopUp), nameof(SingleActionWorkerController), "", "threadTaskID=" + threadTaskID);
                    string postUrl = UrlUtility.GetSingleActionApiUrl(base.MyRequest, base.PortalSettings.DefaultPortalAlias, nameof(SingleActionWorkerController.PostPopUp), nameof(SingleActionWorkerController), "", UrlUtility.GetParamsAsArray(new HttpRequestWrapper(base.MyRequest), new string[] { $"formID={formID}", $"threadTaskID={threadTaskID}", $"stepID={responseVM.EngineFormModel.FormModel.StepID}" }).ToArray());

                    responseVM.EngineFormModel.SetUrlsFromOtherSite(base.PortalSettings.DefaultPortalAlias, new HttpRequestWrapper(base.MyRequest), popUpUrl, postUrl, FormTokenUtility.GetFormToken(base.ApiSessionId, responseVM?.EngineFormModel?.FormModel?.ContentHtml?.DynamicFormID ?? Guid.Empty, responseVM?.EngineFormModel?.FormModel?.IsEncrypted ?? false));
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
                EngineApplicationProxy engineApplicationProxy = new EngineApplicationProxy(SingleActionHomeController.Setting.WebApiAddress, SingleActionHomeController.Setting.WebServicePass, base.ClientUserName, ApiUtility.GetIPAddress(), base.ApiSessionId, this.IsEncrypted);
                GetFormResponseModel responseVM = engineApplicationProxy.GetForm(null, formID, new HttpRequestWrapper(base.MyRequest).GetList(false, string.Empty).ToList());
                if (responseVM.EngineFormModel != null)
                {
                    string popUpUrl = UrlUtility.GetSingleActionApiUrl(base.MyRequest, base.PortalSettings.DefaultPortalAlias, nameof(SingleActionWorkerController.GetPopUp), nameof(SingleActionWorkerController), "");
                    string postUrl = UrlUtility.GetSingleActionApiUrl(base.MyRequest, base.PortalSettings.DefaultPortalAlias, nameof(SingleActionWorkerController.PostPopUp), nameof(SingleActionWorkerController), "", UrlUtility.GetParamsAsArray(new HttpRequestWrapper(base.MyRequest), new string[] { $"applicationPageId={responseVM.EngineFormModel.ApplicationID}" }).ToArray());

                    responseVM.EngineFormModel.SetUrlsFromOtherSite(base.PortalSettings.DefaultPortalAlias, new HttpRequestWrapper(base.MyRequest), popUpUrl, postUrl, FormTokenUtility.GetFormToken(base.ApiSessionId, responseVM?.EngineFormModel?.FormModel?.ContentHtml?.DynamicFormID ?? Guid.Empty, responseVM?.EngineFormModel?.FormModel?.IsEncrypted ?? false));
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
            if (SingleActionHomeController.Setting.ProcessID.HasValue)
            {
                #region .:: Thread ::.
                EngineProcessProxy engineProcessProxy = new EngineProcessProxy(SingleActionHomeController.Setting.WebApiAddress, SingleActionHomeController.Setting.WebServicePass, base.ClientUserName, ApiUtility.GetIPAddress(), base.ApiSessionId, this.IsEncrypted);
                PostTaskFormResponseModel responseVM = engineProcessProxy.PostTaskForm(threadTaskID.Value, controlId, stepID.Value, goNext, this.MyRequest.GetList(false, string.Empty).ToList());

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
                                        base.GetRedirectUrl(responseVM.RedirectUrlModel), reloadForm: SingleActionHomeController.Setting.ProcessEndFormID.HasValue,
                                       listDownloadModel: responseVM.ListDownloadModel,
                                       messageList: responseVM.ListMessageModel
                                       )
                            {
                                EndAppPageID = SingleActionHomeController.Setting.ProcessEndFormID,
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
                EngineApplicationProxy engineApplicationProxy = new EngineApplicationProxy(SingleActionHomeController.Setting.WebApiAddress, SingleActionHomeController.Setting.WebServicePass, base.ClientUserName, ApiUtility.GetIPAddress(), base.ApiSessionId, this.IsEncrypted);
                PostFormResponseModel responseVM = engineApplicationProxy.PostForm(applicationPageId.Value, controlId, this.MyRequest.GetList(false, string.Empty).ToList());

                if (!responseVM.IsSuccess)
                    return new EngineFormResponseDTO(base.GetRedirectUrl(responseVM.RedirectUrlModel), false, null, false, responseVM?.ListMessageModel, false);
                else
                {
                    return
                        new EngineFormResponseDTO(
                        redirectUrl: base.GetRedirectUrl(responseVM.RedirectUrlModel),
                        true,
                        listDownloadModel: responseVM.ListDownloadModel, false,
                        responseVM.ListMessageModel
                         );
                }
                #endregion
            }
        }

        [HttpPost]
        public object PostPopUp(Guid? applicationPageId = null, Guid? threadTaskID = null, Guid? formID = null, string controlId = "")
        {
            if (SingleActionHomeController.Setting.ProcessID.HasValue)
            {
                #region .:: Thread ::.
                EngineProcessProxy engineProcessProxy = new EngineProcessProxy(SingleActionHomeController.Setting.WebApiAddress, SingleActionHomeController.Setting.WebServicePass, base.ClientUserName, ApiUtility.GetIPAddress(), base.ApiSessionId, this.IsEncrypted);
                PostTaskFormResponseModel responseVM = engineProcessProxy.PostForm(threadTaskID.Value, formID.Value, controlId, this.MyRequest.GetList(false, string.Empty).ToList());

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
                EngineApplicationProxy engineApplicationProxy = new EngineApplicationProxy(SingleActionHomeController.Setting.WebApiAddress, SingleActionHomeController.Setting.WebServicePass, base.ClientUserName, ApiUtility.GetIPAddress(), base.ApiSessionId, this.IsEncrypted);
                PostFormResponseModel responseVM = engineApplicationProxy.PostForm(applicationPageId.Value, controlId, this.MyRequest.GetList(false, string.Empty).ToList());

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
    }
}