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
    public class CartablePageController : BpmsApiControlBase
    {
        // GET: KartableThread 
        public object GetIndex(Guid applicationPageId)
        {
            //base.SetMenuIndex(applicationPageId);
            EngineSharedModel engineSharedModel = new EngineSharedModel(applicationPageId, base.MyRequest.GetList(this.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId);
            using (ApplicationPageEngine applicationPageEngine = new ApplicationPageEngine(engineSharedModel))
            {
                GetFormResponseModel responseVM = applicationPageEngine.GetForm();
                if (responseVM.EngineFormModel != null)
                {
                    string popUpUrl = UrlUtility.GetCartableApiUrl(base.MyRequest, base.PortalSettings.DefaultPortalAlias, nameof(CartablePageController.GetPopUp), nameof(CartablePageController), "");
                    string postUrl = UrlUtility.GetCartableApiUrl(base.MyRequest, base.PortalSettings.DefaultPortalAlias, nameof(CartablePageController.PostIndex), nameof(CartablePageController), "", UrlUtility.GetParamsAsArray(new HttpRequestWrapper(base.MyRequest), new string[] { $"applicationPageId={applicationPageId}" }).ToArray());

                    responseVM.EngineFormModel.SetUrls(popUpUrl, postUrl, new HttpRequestWrapper(base.MyRequest), base.PortalSettings.DefaultPortalAlias, FormTokenUtility.GetFormToken(base.ApiSessionId, responseVM?.EngineFormModel?.FormModel?.ContentHtml?.DynamicFormID ?? Guid.Empty, responseVM?.EngineFormModel?.FormModel?.IsEncrypted ?? false));
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

        [HttpPost]
        [BpmsFormTokenAuth]
        public object PostIndex(Guid applicationPageId, string controlId = "")
        {
            EngineSharedModel engineSharedModel = new EngineSharedModel(applicationPageId, base.MyRequest.GetList(this.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId);
            using (ApplicationPageEngine applicationPageEngine = new ApplicationPageEngine(engineSharedModel))
            {
                PostFormResponseModel postFormResponseModel = applicationPageEngine.PostForm(controlId);
                if (!postFormResponseModel.IsSuccess)
                    return new EngineFormResponseDTO(base.GetRedirectUrl(postFormResponseModel.RedirectUrlModel), false, null, false, postFormResponseModel?.ListMessageModel, false);
                else
                {
                    return
                        new EngineFormResponseDTO(
                        redirectUrl: base.GetRedirectUrl(postFormResponseModel.RedirectUrlModel),
                        true,
                        listDownloadModel: postFormResponseModel.ListDownloadModel, false,
                        postFormResponseModel.ListMessageModel
                         );
                }
            }
        }

        [HttpGet]
        public object GetPopUp(Guid formID)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                sysBpmsDynamicForm dynamicForm = dynamicFormService.GetInfo(formID);
                Guid applicationId = dynamicForm.ApplicationPageID.Value;
                EngineSharedModel engineSharedModel = new EngineSharedModel(applicationId, base.MyRequest.GetList(this.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId);
                using (ApplicationPageEngine applicationPageEngine = new ApplicationPageEngine(engineSharedModel))
                {
                    GetFormResponseModel responseVM = applicationPageEngine.GetForm();
                    if (responseVM.EngineFormModel != null)
                    {
                        string popUpUrl = UrlUtility.GetCartableApiUrl(base.MyRequest, base.PortalSettings.DefaultPortalAlias, nameof(CartablePageController.GetPopUp), nameof(CartablePageController), "");
                        string postUrl = UrlUtility.GetCartableApiUrl(base.MyRequest, base.PortalSettings.DefaultPortalAlias, nameof(CartablePageController.PostPopUp), nameof(CartablePageController), "", UrlUtility.GetParamsAsArray(new HttpRequestWrapper(base.MyRequest), new string[] { $"applicationPageId={applicationId}" }).ToArray());
                        responseVM.EngineFormModel.SetUrls(popUpUrl, postUrl, new HttpRequestWrapper(base.MyRequest), base.PortalSettings.DefaultPortalAlias, FormTokenUtility.GetFormToken(base.ApiSessionId, responseVM?.EngineFormModel?.FormModel?.ContentHtml?.DynamicFormID ?? Guid.Empty, responseVM?.EngineFormModel?.FormModel?.IsEncrypted ?? false));
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

        [HttpPost]
        [BpmsFormTokenAuth]
        public object PostPopUp(Guid applicationPageId, string controlId = "")
        {
            EngineSharedModel engineSharedModel = new EngineSharedModel(applicationPageId, base.MyRequest.GetList(this.IsEncrypted, base.ApiSessionId).ToList(), base.ClientUserName, base.ApiSessionId);
            using (ApplicationPageEngine applicationPageEngine = new ApplicationPageEngine(engineSharedModel))
            {
                PostFormResponseModel responseVM = applicationPageEngine.PostForm(controlId);

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
            }
        }

        private string[] GetFormUrlParams(Guid applicationPageId)
        {
            return UrlUtility.GetParamsAsArray(new HttpRequestWrapper(base.MyRequest), new string[] { $"applicationPageId={applicationPageId}" }).Where(c => !this.MyRequest.Form.AllKeys.Union(new string[] { "controlId" }).Any(d => c.Contains(d + "="))).ToArray();
        }
    }
}