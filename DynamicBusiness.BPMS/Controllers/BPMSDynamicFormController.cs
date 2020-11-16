using DynamicBusiness.BPMS.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DynamicBusiness.BPMS.Domain;
using System.Web.Script.Serialization;
using DynamicBusiness.BPMS.SharedPresentation;
using Newtonsoft.Json.Linq;

namespace DynamicBusiness.BPMS.Controllers
{
    [System.Web.Http.AllowAnonymous]
    public class BpmsDynamicFormController : BpmsAdminApiControlBase
    {
        public object GetList([System.Web.Http.FromUri] DynamicFormIndexSearchDTO indexSearchVM)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            { 
                indexSearchVM.Update(dynamicFormService.GetList(indexSearchVM.ProcessId, null, !indexSearchVM.ProcessId.HasValue, indexSearchVM.Name, null, indexSearchVM.GetPagingProperties).Select(c => new DynamicFormDTO(c)).ToList());
                return indexSearchVM;
            }
        }

        [HttpGet]
        public object GetAddEdit(Guid? ID = null)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                DynamicFormDTO dynamicFormDTO = ID.HasValue ? new DynamicFormDTO(dynamicFormService.GetInfo(ID.Value)) : new DynamicFormDTO();
                dynamicFormDTO.ProcessId = base.ProcessId;
                return dynamicFormDTO;
            }
        }

        [HttpPost]
        public object PostAddEdit(DynamicFormDTO dynamicFormDTO)
        {
            ResultOperation resultOperation = new ResultOperation();
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                if (dynamicFormDTO.ID != Guid.Empty)
                {
                    sysBpmsDynamicForm dynamicForm = dynamicFormService.GetInfo(dynamicFormDTO.ID);
                    dynamicForm.Update(dynamicFormDTO.ProcessId, dynamicFormDTO.ApplicationPageID, dynamicFormDTO.Name, dynamicFormDTO.Version, dynamicFormDTO.ShowInOverview);

                    DynamicFormConfigXmlModel configXmlModel = dynamicForm.ConfigXmlModel;
                    configXmlModel.IsEncrypted = dynamicFormDTO.IsEncrypted;
                    dynamicForm.Update(configXmlModel);

                    resultOperation = dynamicFormService.Update(dynamicForm, base.UserInfo?.Username);
                }
                else
                {
                    sysBpmsDynamicForm dynamicForm = new sysBpmsDynamicForm().Update(dynamicFormDTO.ProcessId, dynamicFormDTO.ApplicationPageID, dynamicFormDTO.Name, dynamicFormDTO.Version, dynamicFormDTO.ShowInOverview);

                    DynamicFormConfigXmlModel configXmlModel = dynamicForm.ConfigXmlModel;
                    configXmlModel.IsEncrypted = dynamicFormDTO.IsEncrypted;
                    dynamicForm.Update(configXmlModel);

                    resultOperation = dynamicFormService.Add(dynamicForm, base.UserInfo?.Username);
                    base.ApplicationPageId = dynamicForm.ApplicationPageID;
                }
            }
            if (resultOperation.IsSuccess)
                return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success, base.ApplicationPageId);
            else
                return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
        }

        [HttpGet]
        public object GetEditApplicationPage(Guid ApplicationPageID)
        {
            //base.SetMenuIndex(AdminMenuIndex.ApplicationPageEdit);
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                DynamicFormDTO dynamicFormDTO = new DynamicFormDTO(dynamicFormService.GetInfoByPageID(ApplicationPageID));
                using (LURowService luRowService = new LURowService())
                {
                    dynamicFormDTO.ApplicationPageDTO.ListGroupLU = luRowService.GetList(sysBpmsLUTable.e_LUTable.ApplicationPageGroupLU.ToString()).Select(c => new LURowDTO(c)).ToList();
                }

                using (ApplicationPageAccessService applicationPageAccessService = new ApplicationPageAccessService())
                using (LURowService luRowService = new LURowService())
                using (UserService userService = new UserService())
                using (DepartmentService departmentService = new DepartmentService())
                    return Json(new
                    {
                        ListApplicationPageAccess = applicationPageAccessService.GetList(dynamicFormDTO.ApplicationPageID, null).Select(c => new ApplicationPageAccessDTO(c)).ToList(),
                        ListDepartments = departmentService.GetList(true, "", null).Select(c => new DepartmentDTO(c)).ToList(),
                        ListRoles = luRowService.GetList(sysBpmsLUTable.e_LUTable.DepartmentRoleLU.ToString()).Select(c => new LURowDTO(c)).ToList(),
                        ListUsers = userService.GetList("", null).Select(c => new UserDTO(c)).ToList(),
                        Model = dynamicFormDTO
                    });
            }
        }

        [HttpPost]
        public object PostEditApplicationPage(PostEditApplicationPageDTO model)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                sysBpmsDynamicForm dynamicForm = dynamicFormService.GetInfo(model.DynamicFormDTO.ID);
                dynamicForm.Update(null, model.DynamicFormDTO.ApplicationPageID, model.DynamicFormDTO.Name, model.DynamicFormDTO.Version, model.DynamicFormDTO.ShowInOverview);

                DynamicFormConfigXmlModel configXmlModel = dynamicForm.ConfigXmlModel;
                configXmlModel.IsEncrypted = model.DynamicFormDTO.IsEncrypted;
                dynamicForm.Update(configXmlModel);

                dynamicFormService.Update(dynamicForm, base.UserInfo?.Username);

                using (ApplicationPageService applicationPageService = new ApplicationPageService())
                {
                    sysBpmsApplicationPage applicationPage = applicationPageService.GetInfo(model.DynamicFormDTO.ApplicationPageID.Value);
                    applicationPage.Update(model.DynamicFormDTO.ApplicationPageDTO.GroupLU, model.DynamicFormDTO.ApplicationPageDTO.Description, model.DynamicFormDTO.ApplicationPageDTO.ShowInMenu);
                    applicationPageService.Update(applicationPage);

                    using (ApplicationPageAccessService applicationPageAccessService = new ApplicationPageAccessService())
                    {
                        List<sysBpmsApplicationPageAccess> newApplicationPageAccess =
                            (model.listRole ?? new List<ApplicationPageAccessDTO>()).Select(c => new sysBpmsApplicationPageAccess().Update(c.ID, model.DynamicFormDTO.ApplicationPageID.Value, c.DepartmentID, c.RoleLU, c.UserID, c.AllowAdd, c.AllowEdit, c.AllowDelete, c.AllowView)).ToList()
                            .Union((model.listUser ?? new List<ApplicationPageAccessDTO>()).Select(c => new sysBpmsApplicationPageAccess().Update(c.ID, model.DynamicFormDTO.ApplicationPageID.Value, c.DepartmentID, c.RoleLU, c.UserID, c.AllowAdd, c.AllowEdit, c.AllowDelete, c.AllowView))).ToList();
                        applicationPageAccessService.AddEdit(applicationPage.ID, newApplicationPageAccess);
                    }
                }
            }
            return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
        }

        [HttpDelete]
        public object Delete(Guid ID)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                ResultOperation resultOperation = dynamicFormService.Delete(ID);
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }

        [HttpGet]
        public object GetCopy(Guid ID)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                ResultOperation resultOperation = dynamicFormService.Copy(ID, base.UserInfo?.Username);
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }

        [HttpGet]
        public object GetAddEditFormDesign(Guid? ID = null)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                DynamicFormDTO dynamicFormDTO = new DynamicFormDTO(ID.HasValue ? dynamicFormService.GetInfo(ID.Value) :
                    dynamicFormService.GetInfoByPageID(base.ApplicationPageId.Value));
                if (dynamicFormDTO != null)
                {
                    using (VariableService variableService = new VariableService())
                    using (OperationService operationService = new OperationService())
                        return Json(new
                        {
                            //it is used in dataGrid seting for binging form to openForm Column Item Type.
                            ProcessForms = dynamicFormService.GetList(base.ProcessId, null, base.ApplicationPageId.HasValue, string.Empty, null, null).Select(c => new { value = c.ID, text = c.Name }).ToList(),
                            //it is used in dataGrid seting for binging form to operation Column Item Type.
                            Operations = new OperationService().GetList(null, null).Select(c => new OperationDTO(c)).ToList(),
                            //it is used for binding variable of entity type to fileupload controls.
                            EntityVariables = variableService.GetList(base.ProcessId, base.ApplicationPageId, (int)sysBpmsVariable.e_VarTypeLU.Object, "", null, true).Select(c => new { text = c.Name, value = c.ID }).ToList(),
                            //it is used for binding variable of list type to list controls like dropdownlist or checkboxlist.
                            ListVariables = variableService.GetList(base.ProcessId, base.ApplicationPageId, (int)sysBpmsVariable.e_VarTypeLU.List, "", null, true).Select(c => new { text = c.Name, value = c.Name }).ToList(),
                            Model = dynamicFormDTO
                        });
                }
                else return new PostMethodMessage(SharedLang.Get("NotFound.Text"), DisplayMessageType.error);
            }
        }

        [HttpPost]
        public object PostAddEditFormDesign(PostAddEditFormDesignDTO model)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                sysBpmsDynamicForm dynamicForm = dynamicFormService.GetInfo(model.DynamicFormId);
                if (dynamicForm != null)
                {
                    ResultOperation resultOperation = dynamicForm.Update(model.DesignJson);
                    if (!resultOperation.IsSuccess)
                        return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                    resultOperation = dynamicFormService.GetSourceCode(dynamicForm);
                    if (!resultOperation.IsSuccess)
                        return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                    resultOperation = dynamicFormService.Update(dynamicForm, base.UserInfo?.Username);
                    if (resultOperation.IsSuccess)
                        return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                    else
                        return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                }
                else return new PostMethodMessage(SharedLang.Get("NotFound.Text"), DisplayMessageType.error);
            }

        }

        [HttpPost]
        public object PostAddEditOnExitFormCode(PostAddEditOnEntryFormCodeDTO model)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                sysBpmsDynamicForm dynamicForm = dynamicFormService.GetInfo(model.DynamicFormId);
                dynamicForm.UpdateOnExitFormCode(model.DesignCode);
                ResultOperation resultOperation = dynamicFormService.GetSourceCode(dynamicForm);
                if (!resultOperation.IsSuccess)
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                resultOperation = dynamicFormService.Update(dynamicForm, base.UserInfo?.Username);

                if (!resultOperation.IsSuccess)
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);

                return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
            }
        }

        [HttpPost]
        public object PostAddEditOnEntryFormCode(PostAddEditOnEntryFormCodeDTO model)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                sysBpmsDynamicForm dynamicForm = dynamicFormService.GetInfo(model.DynamicFormId);
                dynamicForm.UpdateOnEntryFormCode(model.DesignCode);
                ResultOperation resultOperation = dynamicFormService.GetSourceCode(dynamicForm);
                if (!resultOperation.IsSuccess)
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                resultOperation = dynamicFormService.Update(dynamicForm, base.UserInfo?.Username);

                if (!resultOperation.IsSuccess)
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);

                return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
            }
        }

        [HttpPost]
        public object PostOnLoadScriptCode(PostOnLoadScriptCodeDTO model)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                sysBpmsDynamicForm dynamicForm = dynamicFormService.GetInfo(model.DynamicFormId);

                DynamicFormConfigXmlModel configXmlModel = dynamicForm.ConfigXmlModel;
                configXmlModel.OnLoadFunctionBody = model.FunctionCode;
                dynamicForm.Update(configXmlModel);
                ResultOperation resultOperation = dynamicFormService.Update(dynamicForm, base.UserInfo?.Username);

                if (!resultOperation.IsSuccess)
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);

                return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
            }
        }

        /// <summary>
        /// it is called from _FormGenerator for binding documentDef list to fileupload controls.
        /// </summary>
        [HttpGet]
        public object GetDocumentFolder()
        {
            return new DocumentDefService().HelperGetTreeList();
        }

        #region .:: Preview ::.

        [HttpGet]
        public object PreviewForm(Guid formID, bool isModal = false)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                EngineFormModel engineForm = dynamicFormService.PreviewForm(formID, base.UserInfo?.Username);
                engineForm.SetUrls(string.Empty, string.Empty, new HttpRequestWrapper(base.MyRequest), base.PortalSettings.DefaultPortalAlias, FormTokenUtility.GetFormToken(HttpContext.Current.Session.SessionID, engineForm?.FormModel?.ContentHtml?.DynamicFormID ?? Guid.Empty, engineForm?.FormModel?.IsEncrypted ?? false));
                return engineForm;
            }
        }

        #endregion

        #region ..:: style sheet ::..
        [HttpGet]
        public object GetAddEditStyleSheet(Guid DynamicFormId)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                sysBpmsDynamicForm dynamicForm = dynamicFormService.GetInfo(DynamicFormId);
                return Json(new
                {
                    StyleCode = dynamicForm.ConfigXmlModel.StyleSheetCode.ToStringObj(),
                    DynamicFormId
                });
            }
        }

        [HttpPost]
        public object PostAddEditStyleSheet(PostAddEditStyleSheetDTO model)
        {

            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {

                sysBpmsDynamicForm dynamicForm = dynamicFormService.GetInfo(model.DynamicFormId);

                DynamicFormConfigXmlModel configXmlModel = dynamicForm.ConfigXmlModel;
                configXmlModel.StyleSheetCode = model.StyleCode;
                dynamicForm.Update(configXmlModel);
                ResultOperation resultOperation = dynamicFormService.Update(dynamicForm, base.UserInfo?.Username);

                if (!resultOperation.IsSuccess)
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);

                return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
            }
        }

        #endregion


    }


}
