using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using Newtonsoft.Json.Linq;
using System.Web;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ApplicationPageEngine : BaseEngine, IApplicationPageEngine
    {

        public ApplicationPageEngine(EngineSharedModel engineSharedModel, IUnitOfWork unitOfWork = null) : base(engineSharedModel, unitOfWork) { }

        public List<sysBpmsApplicationPage> GetAvailable(Guid? userID, string UserName, bool? ShowInMenu)
        {
            userID = userID ?? (string.IsNullOrWhiteSpace(UserName) ? (Guid?)null : new UserService(base.UnitOfWork).GetInfo(UserName)?.ID);
            return this.UnitOfWork.Repository<IApplicationPageRepository>().GetAvailable(userID, ShowInMenu);
        }

        public bool CheckUserAccessByForm(Guid dynamicFormID, ElementBase.e_AccessType e_AccessType)
        {
            sysBpmsDynamicForm dynamicForm = new DynamicFormService(base.UnitOfWork).GetInfo(dynamicFormID);
            if (dynamicForm == null) return false;
            if (!dynamicForm.ApplicationPageID.HasValue) return true;
            if (base.EngineSharedModel == null) return true;
            sysBpmsUser user = string.IsNullOrWhiteSpace(base.EngineSharedModel.CurrentUserName) ? null :
                new UserService(base.UnitOfWork).GetInfo(base.EngineSharedModel.CurrentUserName);

            return this.UnitOfWork.Repository<IApplicationPageAccessRepository>().GetUserAccess(user?.ID, dynamicForm.ApplicationPageID.Value, e_AccessType);
        }

        public bool CheckUserAccessByApplicationID(Guid applicationID, ElementBase.e_AccessType e_AccessType)
        {
            sysBpmsUser user = string.IsNullOrWhiteSpace(base.EngineSharedModel.CurrentUserName) ? null :
                new UserService(base.UnitOfWork).GetInfo(base.EngineSharedModel.CurrentUserName);
            return this.UnitOfWork.Repository<IApplicationPageAccessRepository>().GetUserAccess(user?.ID, applicationID, e_AccessType);
        }

        public GetFormResponseModel GetForm()
        {
            if (!base.EngineSharedModel.CurrentApplicationPageID.HasValue)
                return null;
            using (ApplicationPageEngine applicationPageEngine = new ApplicationPageEngine(base.EngineSharedModel))
            {
                if (applicationPageEngine.CheckUserAccessByApplicationID(base.EngineSharedModel.CurrentApplicationPageID.Value, ElementBase.e_AccessType.AllowView))
                {
                    var result = this.GetContentHtmlByPage(base.EngineSharedModel.CurrentApplicationPageID.Value);
                    EngineFormModel formVM = new EngineFormModel(result.FormModel, base.EngineSharedModel.CurrentApplicationPageID.Value);
                    return new GetFormResponseModel(formVM, result.ListMessageModel, result.RedirectUrlModel);
                }
                else
                    return new GetFormResponseModel(null, new List<MessageModel>() { new MessageModel(DisplayMessageType.error, "عدم دسترسی به فرم") }, null);
            }
        }

        public PostFormResponseModel PostForm(string controlId)
        {
            using (ApplicationPageEngine applicationPageEngine = new ApplicationPageEngine(base.EngineSharedModel))
            {
                if (applicationPageEngine.CheckUserAccessByApplicationID(base.EngineSharedModel.CurrentApplicationPageID.Value, ElementBase.e_AccessType.AllowView))
                {
                    var result = this.SaveContentHtmlByPage(base.EngineSharedModel.CurrentApplicationPageID.Value, controlId);
                    if (!result.ResultOperation.IsSuccess)
                        return new PostFormResponseModel(result.ListMessageModel, result.ResultOperation.GetErrors(), false, false, result.RedirectUrlModel);
                    else
                    {
                        if (result.IsSubmit)
                            return new PostFormResponseModel(result.ListMessageModel, SharedLang.Get("Success.Text"), true, result.IsSubmit, result.RedirectUrlModel, result.ListDownloadModel);
                        else
                            return new PostFormResponseModel(result.ListMessageModel, SharedLang.Get("Success.Text"), true, result.IsSubmit, result.RedirectUrlModel, result.ListDownloadModel);
                    }
                }
                else
                    return new PostFormResponseModel(new List<MessageModel>() { }, LangUtility.Get("PostFormNotAccess.Text", "Engine"), false, false, null);
            }
        }


        #region .:: private methods ::.
        private EngineResponseModel GetContentHtmlByPage(Guid applicationPageId)
        {
            CodeBaseSharedModel codeBaseShared = new CodeBaseSharedModel();
            FormModel formModel = new FormModel();
            sysBpmsDynamicForm dynamicForm = new DynamicFormService(base.UnitOfWork).GetInfoByPageID(applicationPageId);
            ResultOperation resultOperation = new ResultOperation();
            //convert form xml code to json object
            JObject obj = JObject.Parse(dynamicForm.DesignJson);
            HtmlElementHelperModel htmlElementHelperModel = HtmlElementHelper.MakeModel(base.EngineSharedModel, base.UnitOfWork, HtmlElementHelperModel.e_FormAction.Onload, dynamicForm);
            //if json object has a control with type = CONTENT
            if (obj != null && obj["type"].ToString() == "CONTENT")
            {
                formModel = new FormModel(obj, htmlElementHelperModel, null, null, dynamicForm, false);
                resultOperation = formModel.ResultOperation;
            }
            CodeResultModel codeResultModel = null;
            if (resultOperation.IsSuccess)
            {
                if (!string.IsNullOrWhiteSpace(dynamicForm.OnEntryFormCode))
                {
                    DynamicCodeEngine dynamicCodeEngine = new DynamicCodeEngine(base.EngineSharedModel, base.UnitOfWork);
                    codeResultModel = dynamicCodeEngine.ExecuteOnEntryFormCode(DesignCodeUtility.GetDesignCodeFromXml(dynamicForm.OnEntryFormCode), formModel, codeBaseShared);
                    DynamicCodeEngine.SetToErrorMessage(codeResultModel, resultOperation);
                    //If in code any variable is set, it Will save them all at the end
                    dynamicCodeEngine.SaveExternalVariable(codeResultModel);
                }
            }

            return new EngineResponseModel().InitGet(resultOperation, codeBaseShared.MessageList, codeResultModel?.RedirectUrlModel, formModel);
        }

        private EngineResponseModel SaveContentHtmlByPage(Guid applicationPageId, string buttonControlId)
        {
            ResultOperation resultOperation = new ResultOperation();
            RedirectUrlModel redirectUrlModel = null;
            CodeBaseSharedModel codeBaseShared = new CodeBaseSharedModel();
            try
            {
                FormModel formModel = new FormModel();
                sysBpmsDynamicForm dynamicForm = new DynamicFormService(base.UnitOfWork).GetInfoByPageID(applicationPageId);

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
                                return new EngineResponseModel().InitPost(resultOperation, codeBaseShared.MessageList, redirectUrlModel, isSubmit: false, listDownloadModel: codeBaseShared.ListDownloadModel);
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
                            resultOperation = new DataManageEngine(base.EngineSharedModel, base.UnitOfWork).SaveIntoDataBase(formModel.ContentHtml, null, codeBaseShared.ListSetVariable, null);
                    }
                }
                base.FinalizeService(resultOperation);

                resultOperation.CurrentObject = formModel;
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
