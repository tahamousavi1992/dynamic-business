using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
using Newtonsoft.Json.Linq;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DynamicFormService : ServiceBase
    {
        public DynamicFormService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsDynamicForm dynamicForm, sysBpmsApplicationPage appPage, string userName)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                if (dynamicForm.ProcessId.HasValue && !new ProcessService(base.UnitOfWork).GetInfo(dynamicForm.ProcessId.Value).AllowEdit())
                    resultOperation.AddError(LangUtility.Get("NotAllowEdit.Text", nameof(sysBpmsProcess)));

                if (resultOperation.IsSuccess)
                {
                    base.BeginTransaction();
                    if (!dynamicForm.ProcessId.HasValue && !dynamicForm.ApplicationPageID.HasValue)
                    {
                        appPage = appPage ?? new sysBpmsApplicationPage().Update(0, String.Empty, false);
                        appPage.ID = Guid.Empty;
                        resultOperation = new ApplicationPageService(base.UnitOfWork).Add(appPage);
                        dynamicForm.ApplicationPageID = appPage.ID;
                    }
                    if (resultOperation.IsSuccess)
                    {
                        dynamicForm.CreatedBy = userName.ToStringObj();
                        dynamicForm.CreatedDate = DateTime.Now;
                        dynamicForm.UpdatedBy = userName.ToStringObj();
                        dynamicForm.UpdatedDate = DateTime.Now;

                        this.UnitOfWork.Repository<IDynamicFormRepository>().Add(dynamicForm);
                        this.UnitOfWork.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                return base.ExceptionHandler(ex);
            }
            base.FinalizeService(resultOperation);

            return resultOperation;
        }

        public ResultOperation Update(sysBpmsDynamicForm dynamicForm, string userName)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (dynamicForm.ProcessId.HasValue && !new ProcessService(base.UnitOfWork).GetInfo(dynamicForm.ProcessId.Value).AllowEdit())
                resultOperation.AddError(LangUtility.Get("NotAllowEdit.Text", nameof(sysBpmsProcess)));

            if (resultOperation.IsSuccess)
            {
                dynamicForm.UpdatedBy = userName;
                dynamicForm.UpdatedDate = DateTime.Now;
                this.UnitOfWork.Repository<IDynamicFormRepository>().Update(dynamicForm);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Copy(Guid dynamicFormId, string userName)
        {
            sysBpmsDynamicForm dynamicForm = this.GetInfo(dynamicFormId);
            dynamicForm.ID = Guid.Empty;
            dynamicForm.Name += " - Copy";

            this.UpdateBackendCodeID(dynamicForm);
            this.GetSourceCode(dynamicForm);
            if (dynamicForm.ApplicationPageID.HasValue)
            {
                ApplicationPageService applicationPageService = new ApplicationPageService(base.UnitOfWork);
                ApplicationPageAccessService applicationPageAccessService = new ApplicationPageAccessService(base.UnitOfWork);

                sysBpmsApplicationPage sysBpmsApplicationPage = applicationPageService.GetInfo(dynamicForm.ApplicationPageID.Value);
                List<sysBpmsApplicationPageAccess> sysBpmsApplicationPageAccessList = applicationPageAccessService.GetList(dynamicForm.ApplicationPageID.Value, null);

                //Adding sysBpmsApplicationPage
                sysBpmsApplicationPage.ID = Guid.NewGuid();
                applicationPageService.Add(sysBpmsApplicationPage);

                foreach (var item in sysBpmsApplicationPageAccessList)
                {
                    item.ID = Guid.NewGuid();
                    item.ApplicationPageID = sysBpmsApplicationPage.ID;
                    applicationPageAccessService.Add(item);
                }
                dynamicForm.ApplicationPageID = sysBpmsApplicationPage.ID;
            }
            dynamicForm.CreatedBy = userName;
            dynamicForm.CreatedDate = DateTime.Now;
            dynamicForm.UpdatedBy = userName;
            dynamicForm.UpdatedDate = DateTime.Now;
            this.UnitOfWork.Repository<IDynamicFormRepository>().Add(dynamicForm);
            this.UnitOfWork.Save();


            return new ResultOperation();
        }

        public ResultOperation Delete(Guid dynamicFormId)
        {
            ResultOperation resultOperation = null;
            try
            {
                resultOperation = new ResultOperation();
                if (resultOperation.IsSuccess)
                {
                    base.BeginTransaction();

                    List<sysBpmsStep> list = new StepService(this.UnitOfWork).GetList(null, dynamicFormId);
                    foreach (var item in list)
                    {
                        resultOperation = new StepService(this.UnitOfWork).Delete(item.ID);
                        if (!resultOperation.IsSuccess)
                            break;
                    }
                    if (resultOperation.IsSuccess)
                    {
                        sysBpmsDynamicForm dynamicForm = this.GetInfo(dynamicFormId);
                        this.UnitOfWork.Repository<IDynamicFormRepository>().Delete(dynamicFormId);

                        if (resultOperation.IsSuccess && dynamicForm.ApplicationPageID.HasValue)
                        {
                            //delete page variables
                            List<sysBpmsVariable> listVariable = new VariableService(this.UnitOfWork).GetList(null, dynamicForm.ApplicationPageID.Value, null, "", null, null);
                            foreach (var item in listVariable)
                            {
                                resultOperation = new VariableService(this.UnitOfWork).Delete(item.ID);
                                if (!resultOperation.IsSuccess)
                                    break;
                            }
                            //delete application page
                            if (resultOperation.IsSuccess)
                                resultOperation = new ApplicationPageService(base.UnitOfWork).Delete(dynamicForm.ApplicationPageID.Value);
                        }
                    }
                    this.UnitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
                return base.ExceptionHandler(ex);
            }
            base.FinalizeService(resultOperation);

            return resultOperation;
        }

        public sysBpmsDynamicForm GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<IDynamicFormRepository>().GetInfo(ID);
        }

        public sysBpmsDynamicForm GetInfoByPageID(Guid ID)
        {
            return this.UnitOfWork.Repository<IDynamicFormRepository>().GetInfoByPageID(ID);
        }

        public sysBpmsDynamicForm GetInfoByStepID(Guid ID)
        {
            return this.UnitOfWork.Repository<IDynamicFormRepository>().GetInfoByStepID(ID);
        }

        public List<sysBpmsDynamicForm> GetList(Guid? processId, Guid? applicationPageID, bool? GetPages, string Name, bool? showInOverview, PagingProperties currentPaging)
        {
            return this.UnitOfWork.Repository<IDynamicFormRepository>().GetList(processId, applicationPageID, GetPages, Name, showInOverview, currentPaging);
        }

        //Get list controls as Dictionary<string, string> Key=Id Value=label
        public Dictionary<string, string> GetControls(sysBpmsDynamicForm dynamicForm)
        {
            if (string.IsNullOrWhiteSpace(dynamicForm.DesignJson))
                return new Dictionary<string, string>();

            JObject obj = JObject.Parse(dynamicForm.DesignJson);
            if (obj != null && obj["type"].ToString() == "CONTENT")
            {
                return dynamicForm.GetControls().ToDictionary(
                    f => f["id"].ToStringObj(),
                    g => (g["type"].ToStringObj() == "HTMLCODE" || string.IsNullOrWhiteSpace(g["label"].ToStringObj())) ? g["id"].ToStringObj() : g["label"].ToStringObj());
            }
            else
                return new Dictionary<string, string>();
        }

        public ResultOperation GetSourceCode(sysBpmsDynamicForm dynamicForm)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                FormModel formModel = new FormModel(JObject.Parse(dynamicForm.DesignJson), HtmlElementHelper.MakeModel(null, null, HtmlElementHelperModel.e_FormAction.FillMode, dynamicForm), null, null, dynamicForm, false);
                string renderedCode = string.Empty;
                formModel.ContentHtml.Rows.ForEach((row) =>
                {
                    renderedCode += renderCode(row);
                    if (row is RowHtml)
                        readRow(row);
                    else
                    {
                        ((AccordionHtml)row).Cards.ForEach((car) =>
                        {
                            renderedCode += renderCode(car);
                            ((CardHtml)car).Rows.ForEach((item) =>
                            {
                                readRow(item);
                            });
                        });
                    }
                });

                void readRow(object row)
                {
                    ((RowHtml)row).Columns.ForEach((column) =>
                    {
                        renderedCode += renderCode(column);
                        column.children.ForEach((item) =>
                        {
                            renderedCode += renderCode(item);
                        });
                    });
                }

                string renderCode(object item)
                {
                    DesignCodeModel designCode = null;
                    string code = string.Empty;
                    //VisibilityDesignCodeModel
                    code += makeClass(((ElementBase)item).VisibilityDesignCodeModel);
                    //if is button
                    if (item is ButtonHtml)
                    {
                        code += makeClass(((ButtonHtml)item).ConfirmDesignCodeModel);
                        code += makeClass(DesignCodeUtility.GetDesignCodeFromXml(((ButtonHtml)item).BackendCoding));
                    }
                    if (item is DataGridHtml)
                    {
                        DataGridHtml dataGridHtml = (DataGridHtml)item;
                        dataGridHtml.DataGridColumns.ForEach((column) =>
                        {
                            column.ItemList.ForEach((cItem) =>
                            {
                                designCode = DesignCodeUtility.GetDesignCodeFromXml(cItem.ExpressionConfirmCode.FromBase64());
                                if (designCode != null && !string.IsNullOrWhiteSpace(designCode.Code))
                                {
                                    code += makeClass(designCode);
                                }
                                designCode = DesignCodeUtility.GetDesignCodeFromXml(cItem.RunCodeData.FromBase64());
                                if (designCode != null && !string.IsNullOrWhiteSpace(designCode.Code))
                                {
                                    code += makeClass(designCode);
                                }
                            });
                        });
                    }
                    return code;
                }

                string makeClass(DesignCodeModel designCode)
                {
                    string code = string.Empty;
                    if (designCode != null && !string.IsNullOrWhiteSpace(designCode.Code))
                    {
                        code = DynamicCodeEngine.MakeClass(designCode.Code, designCode.ID);
                    }
                    return code;
                }
                renderedCode += makeClass(DesignCodeUtility.GetDesignCodeFromXml(dynamicForm.OnEntryFormCode));
                renderedCode += makeClass(DesignCodeUtility.GetDesignCodeFromXml(dynamicForm.OnExitFormCode));

                dynamicForm.SourceCode = renderedCode;
                if (dynamicForm.ApplicationPageID.HasValue)
                    DynamicCodeEngine.GenerateAppPageAssembly(dynamicForm);
            }
            catch (Exception ex)
            {
                resultOperation.AddError(ex.ToString());
                resultOperation.AddError("Error while creating code");
            }
            return resultOperation;
        }

        /// <summary>
        /// this will change all back end code id because of compiled dll duplicated.
        /// </summary>
        /// <param name="dynamicForm"></param>
        public void UpdateBackendCodeID(sysBpmsDynamicForm dynamicForm)
        {
            if (!string.IsNullOrWhiteSpace(dynamicForm.DesignJson))
            {
                FormModel formModel = new FormModel(JObject.Parse(dynamicForm.DesignJson), HtmlElementHelper.MakeModel(null, null, HtmlElementHelperModel.e_FormAction.FillMode, dynamicForm), null, null, dynamicForm, false);
                formModel.ContentHtml.Rows.ForEach((row) =>
                {
                    generateCodeID(row);
                    if (row is RowHtml)
                        readRow(row);
                    else
                    {
                        ((AccordionHtml)row).Cards.ForEach((car) =>
                        {
                            generateCodeID(car);
                            ((CardHtml)car).Rows.ForEach((item) =>
                            {
                                readRow(item);
                            });
                        });
                    }
                });

                void readRow(object row)
                {
                    ((RowHtml)row).Columns.ForEach((column) =>
                    {
                        generateCodeID(column);
                        column.children.ForEach((item) =>
                        {
                            generateCodeID(item);
                        });
                    });
                }

                void generateCodeID(object item)
                {
                    replaceID(((ElementBase)item).ExpressionVisibilityCode);
                    if (item is ButtonHtml)
                    {
                        replaceID(((ButtonHtml)item).ExpressionConfirmCode);
                        replaceID(((ButtonHtml)item).BackendCoding);
                    }
                    else
                    if (item is DataGridHtml)
                    {
                        DataGridHtml dataGridHtml = (DataGridHtml)item;
                        dataGridHtml.DataGridColumns.ForEach((column) =>
                        {
                            column.ItemList.ForEach((cItem) =>
                            {
                                if (!string.IsNullOrWhiteSpace(cItem.ExpressionConfirmCode))
                                    replaceID(cItem.ExpressionConfirmCode.FromBase64());
                                if (!string.IsNullOrWhiteSpace(cItem.RunCodeData))
                                    replaceID(cItem.RunCodeData.FromBase64());
                            });
                        });
                    }
                }

                void replaceID(string decodedCode)
                {
                    if (!string.IsNullOrWhiteSpace(decodedCode))
                    {
                        DesignCodeModel designCode = DesignCodeUtility.GetDesignCodeFromXml(decodedCode);
                        if (designCode != null && !string.IsNullOrWhiteSpace(designCode.ID))
                        {
                            dynamicForm.DesignJson = dynamicForm.DesignJson.Replace(decodedCode.ToBase64(), decodedCode.Replace(designCode.ID, Guid.NewGuid().ToStringObj()).ToBase64());
                        }
                    }
                }

                DesignCodeModel codeModel = DesignCodeUtility.GetDesignCodeFromXml(dynamicForm.OnEntryFormCode);
                if (codeModel != null && !string.IsNullOrWhiteSpace(codeModel.ID))
                    dynamicForm.OnEntryFormCode = dynamicForm.OnEntryFormCode.Replace(codeModel.ID, Guid.NewGuid().ToString());
                codeModel = DesignCodeUtility.GetDesignCodeFromXml(dynamicForm.OnExitFormCode);
                if (codeModel != null && !string.IsNullOrWhiteSpace(codeModel.ID))
                    dynamicForm.OnExitFormCode = dynamicForm.OnExitFormCode.Replace(codeModel.ID, Guid.NewGuid().ToString());

            }
        }

        public EngineFormModel PreviewForm(Guid formID, string userName)
        {
            sysBpmsDynamicForm dynamicForm = this.GetInfo(formID);

            EngineSharedModel engineSharedModel = dynamicForm.ApplicationPageID.HasValue ?
                new EngineSharedModel(dynamicForm.ApplicationPageID.Value, new List<QueryModel>(), userName, string.Empty) :
                new EngineSharedModel(Guid.Empty, dynamicForm.ProcessId, new List<QueryModel>(), userName, string.Empty);

            FormModel formModel = new FormModel();
            //convert form xml code to json object
            JObject obj = JObject.Parse(dynamicForm.DesignJson);
            HtmlElementHelperModel htmlElementHelperModel = HtmlElementHelper.MakeModel(engineSharedModel, base.UnitOfWork, HtmlElementHelperModel.e_FormAction.Preview, dynamicForm);
            //if json object has a control with type = CONTENT
            if (obj != null && obj["type"].ToString() == "CONTENT")
            {
                formModel = new FormModel(obj, htmlElementHelperModel, null, null, dynamicForm, false);
            }
            EngineFormModel engineForm = dynamicForm.ApplicationPageID.HasValue ?
                new EngineFormModel(formModel, dynamicForm.ApplicationPageID) :
                new EngineFormModel(formModel, Guid.Empty, Guid.Empty, dynamicForm.ProcessId);

            return engineForm;
        }
    }
}
