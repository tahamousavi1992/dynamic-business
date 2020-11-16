using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DynamicBusiness.BPMS.BusinessLogic;
using System.Web.Script.Serialization;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System.IO;
using System.Net.Http;
using System.Net;
using DotNetNuke.Web.Api;

namespace DynamicBusiness.BPMS.Controllers
{ 
    public class BpmsDesignCodeController : BpmsAdminApiControlBase
    {
        /// <param name="designCode">It is a xml having all info about a code</param>
        /// <param name="callBack">which javascript function should be executed at the end</param>
        [HttpPost]
        public object PostIndex(PostDesignCodePostIndexDTO requestModel)
        {
            Guid? dynamicFormId = requestModel.DynamicFormId.ToGuidObjNull();
            DesignCodeModel codeModel = DesignCodeUtility.GetDesignCodeFromXml(requestModel.DesignCode.ToStringObj());
            DesignCodeDTO model = new DesignCodeDTO()
            {
                Code = codeModel?.Code,
                CodeType = (DesignCodeDTO.e_CodeType)requestModel.CodeType,
                CallBack = requestModel.CallBack.ToStringObj(),
                Assemblies = codeModel?.Assemblies,
                ID = string.IsNullOrWhiteSpace(codeModel.ID) ? Guid.NewGuid().ToString() : codeModel.ID,
                CodeObjects = codeModel.CodeObjects,
                DynamicFormID = dynamicFormId,
                DesignCode = codeModel.DesignCode,
                Diagram = codeModel.Diagram.ToStringObj().Trim(),
            };

            if (model.CodeType == DesignCodeDTO.e_CodeType.ConditionCode)
            {
                Random random = new Random();
                DCConditionModel dcConditionModel = (DCConditionModel)model.CodeObjects?.FirstOrDefault() ??
                    new DCConditionModel(Guid.NewGuid().ToString(), "Condition", string.Empty, string.Empty, null, new List<DCRowConditionModel>(), true, "func" + random.Next(100, 100));
                using (VariableService variableService = new VariableService())
                using (DynamicFormService dynamicFormService = new DynamicFormService())
                    return new
                    {
                        OpenDirectly = true,
                        DesignCodeDTO = model,
                        ProcessControls = dynamicFormId != Guid.Empty && dynamicFormId.HasValue ?
                        dynamicFormService.GetControls(dynamicFormService.GetInfo(dynamicFormId.Value)).Select(c => new QueryModel(c.Key, c.Value)).ToList() : new List<QueryModel>(),
                        ProcessVariables = variableService.GetVariableAsComboTree(base.ProcessId, base.ApplicationPageId, null, "{0}"),
                        ListOperationTypes = EnumObjHelper.GetEnumList<DCRowConditionModel.e_OperationType>().Select(c => new QueryModel(c.Key.ToString(), c.Value)).ToList(),
                        Model = dcConditionModel
                    };
            }
            else
            {
                this.InitialData(model, dynamicFormId);
                return model;
            }
        }

        [HttpGet]
        public object GetServiceTaskCode(string ElementId, Guid ProcessId)
        {
            using (TaskService taskService = new TaskService())
            {
                ServiceTaskDTO ServiceTaskDTO = new ServiceTaskDTO(taskService.GetInfo(ElementId, ProcessId));
                DesignCodeDTO model = new DesignCodeDTO()
                {
                    Code = ServiceTaskDTO.DesignCodeModel.Code,
                    ID = string.IsNullOrWhiteSpace(ServiceTaskDTO.DesignCodeModel.ID) ? Guid.NewGuid().ToString() : ServiceTaskDTO.DesignCodeModel.ID,
                    CodeType = DesignCodeDTO.e_CodeType.TaskServiceCode,
                    CallBack = "",
                    Assemblies = ServiceTaskDTO.DesignCodeModel.Assemblies,
                    DesignCode = ServiceTaskDTO.DesignCodeModel.DesignCode,
                    Diagram = ServiceTaskDTO.DesignCodeModel.Diagram.ToStringObj().Trim(),
                    CodeObjects = ServiceTaskDTO.DesignCodeModel.CodeObjects,
                };
                this.InitialData(model);
                return model;
            }
        }

        [HttpGet]
        public object GetDynamicFormCode(string CallBack, Guid DynamicFormId, bool IsOnExitForm)
        {
            DesignCodeDTO model = null;
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                DynamicFormDTO DynamicFormDTO = new DynamicFormDTO(dynamicFormService.GetInfo(DynamicFormId));

                if (IsOnExitForm)
                {
                    model = new DesignCodeDTO()
                    {
                        Code = DynamicFormDTO.OnExitFormCodeDesign.Code,
                        ID = string.IsNullOrWhiteSpace(DynamicFormDTO.OnExitFormCodeDesign.ID) ? Guid.NewGuid().ToString() : DynamicFormDTO.OnExitFormCodeDesign.ID,
                        CodeType = DesignCodeDTO.e_CodeType.DynamicForm,
                        CallBack = CallBack,
                        Assemblies = DynamicFormDTO.OnExitFormCodeDesign.Assemblies,
                        DynamicFormID = DynamicFormId,
                        DesignCode = DynamicFormDTO.OnExitFormCodeDesign.DesignCode,
                        Diagram = DynamicFormDTO.OnExitFormCodeDesign.Diagram.ToStringObj().Trim(),
                        CodeObjects = DynamicFormDTO.OnExitFormCodeDesign.CodeObjects,
                    };
                }
                else
                {
                    model = new DesignCodeDTO()
                    {
                        Code = DynamicFormDTO.OnEntryFormCodeDesign.Code,
                        ID = string.IsNullOrWhiteSpace(DynamicFormDTO.OnEntryFormCodeDesign.ID) ? Guid.NewGuid().ToString() : DynamicFormDTO.OnEntryFormCodeDesign.ID,
                        CodeType = DesignCodeDTO.e_CodeType.DynamicForm,
                        CallBack = CallBack,
                        Assemblies = DynamicFormDTO.OnEntryFormCodeDesign.Assemblies,
                        DynamicFormID = DynamicFormId,
                        DesignCode = DynamicFormDTO.OnEntryFormCodeDesign.DesignCode,
                        Diagram = DynamicFormDTO.OnEntryFormCodeDesign.Diagram.ToStringObj().Trim(),
                        CodeObjects = DynamicFormDTO.OnEntryFormCodeDesign.CodeObjects,
                    };
                }
            }

            this.InitialData(model);
            return model;
        }

        [HttpGet]
        public object GetDynamicFormOnloadJavaCode(Guid DynamicFormId)
        {
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                JavaScriptDesignCodeDTO codeVM = new JavaScriptDesignCodeDTO()
                {
                    Code = dynamicFormService.GetInfo(DynamicFormId).ConfigXmlModel?.OnLoadFunctionBody ?? string.Empty,
                    CallBack = null,
                    GetControls = dynamicFormService.GetControls(dynamicFormService.GetInfo(DynamicFormId)).Select(c => new QueryModel(c.Key, c.Value)).ToList(),
                    GetAllJavaMethods = DesignCodeUtility.GetAllJavaMethods().Select(c => new QueryModel(c, c)).ToList(),
                };

                return codeVM;
            }
        }

        [HttpGet]
        public object GetJavaScriptIndex([System.Web.Http.FromUri] JavaScriptDesignCodeDTO codeVM)
        {

            if (codeVM != null && codeVM.Code.ToStringObj().ToLower().Trim() == "null")
                codeVM.Code = string.Empty;
            using (DynamicFormService dynamicFormService = new DynamicFormService())
                codeVM.GetControls = dynamicFormService.GetControls(dynamicFormService.GetInfo(codeVM.DynamicFormId)).Select(c => new QueryModel(c.Key, c.Value)).ToList();
            codeVM.GetAllJavaMethods = DesignCodeUtility.GetAllJavaMethods().Select(c => new QueryModel(c, c)).ToList();
            return codeVM;
        }

        /// <summary>
        /// load _DesignCodeActionList  according to data list parameter
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public object PostLoadDesignCodeActionList(PostLoadDesignCodeActionListDTO model)
        {
            var dataList = model.ListData?.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => new
            {
                Base64Code = c,
                ODesign = DesignCodeUtility.GetObjectOfDesignCode<object>(c.FromBase64())
            }).ToList();
            using (OperationService operationService = new OperationService())

                return new
                {
                    ListOperations = operationService.GetList(null, null).Select(c => new OperationDTO(c)).ToList(),
                    ListActionTypes = EnumObjHelper.GetEnumList<DCBaseModel.e_ActionType>().Select(c => new QueryModel(c.Key.ToString(), c.Value)).ToList(),
                    model.ShapeId,
                    DynamicFormId = model.DynamicFormId.ToGuidObjNull(),
                    model.ParentShapeId,
                    model.IsOutputYes,
                    Name = model.Name.ToStringObj().Trim(),
                    model.IsFirst,
                    FuncName = (dataList?.Any() ?? false) && !string.IsNullOrWhiteSpace(((DCBaseModel)dataList.FirstOrDefault().ODesign).FuncName) ?
                    ((DCBaseModel)dataList.FirstOrDefault().ODesign).FuncName :
                    DesignCodeUtility.GetFunctionName(model.ShapeId),
                    Model = dataList,
                };
        }

        [HttpPost]
        public object PostLoadSetVariableForm(PostLoadSetVariableFormDTO model)
        {
            Guid? dynamicFormId = model.DynamicFormId.ToGuidObjNull();
            DCSetVariableModel designCode = null;
            if (!string.IsNullOrWhiteSpace(model.XmlB64Model))
            {
                designCode = DesignCodeUtility.GetObjectOfDesignCode<DCSetVariableModel>(model.XmlB64Model.ToStringObj().FromBase64());
                designCode.IsOutputYes = model.IsOutputYes;
            }
            else
                designCode = new DCSetVariableModel(Guid.NewGuid().ToString(), model.ShapeId.ToStringObj(),
                    model.ParentShapeId.ToStringObj(), string.Empty, DCBaseModel.e_ValueType.Variable, "",
                    model.IsOutputYes, model.IsFirst.ToBoolObj(), null);

            using (DynamicFormService dynamicFormService = new DynamicFormService())
                return new
                {
                    DynamicFormId = dynamicFormId,
                    ProcessControls = dynamicFormId != Guid.Empty && dynamicFormId.HasValue ?
                    dynamicFormService.GetControls(dynamicFormService.GetInfo(dynamicFormId.Value)).Select(c => new QueryModel(c.Key, c.Value)).ToList() : new List<QueryModel>(),
                    Model = designCode
                };
        }

        [HttpPost]
        public object PostLoadCallMethodForm(PostLoadCallMethodFormDTO model)
        {
            Guid? dynamicFormId = model.DynamicFormId.ToGuidObjNull();

            DCCallMethodModel designCode = null;
            if (!string.IsNullOrWhiteSpace(model.XmlB64Model.ToStringObj()))
            {
                designCode = DesignCodeUtility.GetObjectOfDesignCode<DCCallMethodModel>(model.XmlB64Model.ToStringObj().FromBase64());
                designCode.IsOutputYes = model.IsOutputYes;
            }
            else
                designCode = new DCCallMethodModel(Guid.NewGuid().ToString(), string.Empty, model.ShapeId.ToStringObj(),
                    model.ParentShapeId.ToStringObj(), model.IsOutputYes, model.IsFirst, model.DefaultMethodID,
                    model.DefaultMethodGroupType, (c) =>
                    {
                        using (OperationService operationService = new OperationService())
                            return operationService.GetParameters(c);
                    });
            using (DynamicFormService dynamicFormService = new DynamicFormService())
                return new
                {
                    DynamicFormId = dynamicFormId,
                    ProcessControls = dynamicFormId != Guid.Empty && dynamicFormId.HasValue ?
                    dynamicFormService.GetControls(dynamicFormService.GetInfo(dynamicFormId.Value)).Select(c => new QueryModel(c.Key, c.Value)).ToList() : new List<QueryModel>(),
                    Model = designCode
                };
        }

        [HttpPost]
        public object PostLoadConditionForm(PostLoadConditionFormDTO model)
        {
            Guid? dynamicFormId = model.DynamicFormId.ToGuidObjNull();
            DCConditionModel designCode;
            if (!string.IsNullOrWhiteSpace(model.Data))
            {
                designCode = DesignCodeUtility.GetObjectOfDesignCode<DCConditionModel>(model.Data.ToStringObj().FromBase64());
                designCode.IsOutputYes = model.IsOutputYes.ToBoolObjNull();
                if (string.IsNullOrWhiteSpace(designCode.FuncName))
                    designCode.FuncName = DesignCodeUtility.GetFunctionName(designCode.ShapeID);
            }
            else
                designCode = new DCConditionModel(Guid.NewGuid().ToString(), model.Name.ToStringObj(),
                   model.ShapeId.ToStringObj(), model.ParentShapeId.ToStringObj(),
                    model.IsOutputYes.ToBoolObjNull(), new List<DCRowConditionModel>(), model.IsFirst.ToBoolObj(), null);

            using (VariableService variableService = new VariableService())
            using (DynamicFormService dynamicFormService = new DynamicFormService())
                return new
                {
                    ProcessControls = dynamicFormId != Guid.Empty && dynamicFormId.HasValue ?
                    dynamicFormService.GetControls(dynamicFormService.GetInfo(dynamicFormId.Value)).Select(c => new QueryModel(c.Key, c.Value)).ToList() : new List<QueryModel>(),
                    ListOperationTypes = EnumObjHelper.GetEnumList<DCRowConditionModel.e_OperationType>().Select(c => new QueryModel(c.Key.ToString(), c.Value)).ToList(),
                    Model = designCode
                };
        }

        [HttpPost]
        public object PostLoadExpressionCodeForm(PostLoadExpressionCodeFormDTO model)
        {
            DCExpressionModel designCode;
            Guid? dynamicFormId = model.DynamicFormId.ToGuidObjNull();
            if (!string.IsNullOrWhiteSpace(model.Data))
            {
                designCode = DesignCodeUtility.GetObjectOfDesignCode<DCExpressionModel>(model.Data.FromBase64());
                designCode.IsOutputYes = model.IsOutputYes;
                if (string.IsNullOrWhiteSpace(designCode.FuncName))
                    designCode.FuncName = DesignCodeUtility.GetFunctionName(designCode.ShapeID);
            }
            else
                designCode = new DCExpressionModel(Guid.NewGuid().ToString(), model.Name.ToStringObj(),
                    model.ShapeId.ToStringObj(), model.ParentShapeId.ToStringObj(),
                    "", model.IsOutputYes, model.IsFirst.ToBoolObj(), null);
            designCode.Assemblies = string.Join(",", designCode.Assemblies.ToStringObj().Split(',').Select(c => c.Trim()).ToList());
            designCode.ExpressionCode = designCode.ExpressionCode.ToStringObj().Replace("\\", "\\\\");
            using (DocumentFolderService documentFolderService = new DocumentFolderService())
            using (DepartmentService departmentService = new DepartmentService())
            using (LURowService luRowService = new LURowService())
            using (VariableService variableService = new VariableService())
            using (DynamicFormService dynamicFormService = new DynamicFormService())
                return new
                {
                    GetAllSysMethods = DesignCodeUtility.GetAllSysMethods().Select(c => new { Key = c }).ToList(),
                    GetVariableMethods = DesignCodeUtility.GetVariableMethods().Select(c => new { Key = c }).ToList(),
                    GetMessageMethods = DesignCodeUtility.GetMessageMethods().Select(c => new { Key = c }).ToList(),
                    GetAccessMethods = DesignCodeUtility.GetAccessMethods().Select(c => new { Key = c }).ToList(),
                    GetHelperMethods = DesignCodeUtility.GetHelperMethods().Select(c => new { Key = c }).ToList(),
                    GetDocumentMethods = DesignCodeUtility.GetDocumentMethods().Select(c => new { Key = c }).ToList(),
                    GetAllSysProperties = DesignCodeUtility.GetAllSysProperties().Select(c => new { Key = c }).ToList(),

                    AssembliesJson = (Directory.Exists(BPMSResources.FilesRoot + BPMSResources.AssemblyRoot) ? new DirectoryInfo(BPMSResources.FilesRoot + BPMSResources.AssemblyRoot).GetFiles("*.dll").Select(c => new AssemblyDTO(c)).Select(c => new ComboTreeModel() { id = c.FileName, title = c.FileName }).ToList() : new List<ComboTreeModel>()),
                    ApplicationPages = dynamicFormService.GetList(null, null, true, string.Empty, null, null).Select(c => new QueryModel(c.ApplicationPageID.ToString(), c.Name)).ToList(),
                    DepartmentRoles = luRowService.GetList("DepartmentRoleLU").Select(c => new LURowDTO(c)).ToList(),
                    DepartmentList = departmentService.GetList(true, "", null).Select(c => new { ID = $"new Guid(\"{ c.ID }\")", c.Name }),
                    DocumentFolders = documentFolderService.GetList(null, "", "", true).Select(c => new QueryModel(c.ID.ToString(), c.DisplayName)).ToList(),
                    ProcessVariables = variableService.GetVariableAsComboTree(base.ProcessId, base.ApplicationPageId, null, "{0}"),
                    ProcessControls = dynamicFormId != Guid.Empty && dynamicFormId.HasValue ?
                    dynamicFormService.GetControls(dynamicFormService.GetInfo(dynamicFormId.Value)).Select(c => new QueryModel(c.Key, c.Value)).ToList() : new List<QueryModel>(),
                    Model = designCode
                };
        }

        [HttpPost]
        public object PostLoadSetControlForm(PostLoadSetControlFormDTO model)
        {
            Guid? dynamicFormId = model.DynamicFormId.ToGuidObjNull();
            DCSetControlModel designCode = null;
            if (!string.IsNullOrWhiteSpace(model.XmlB64Model))
            {
                designCode = DesignCodeUtility.GetObjectOfDesignCode<DCSetControlModel>(model.XmlB64Model.ToStringObj().FromBase64());
                designCode.IsOutputYes = model.IsOutputYes;
            }
            else
                designCode = new DCSetControlModel(Guid.NewGuid().ToString(), model.ShapeId.ToStringObj(),
                   model.ParentShapeId.ToStringObj(), string.Empty, DCBaseModel.e_ValueType.Static,
                    "", model.IsOutputYes, model.IsFirst.ToBoolObj(), null);
             
            using (DynamicFormService dynamicFormService = new DynamicFormService())
                return new
                {
                    DynamicFormId = dynamicFormId,
                    ProcessControls = dynamicFormId != Guid.Empty && dynamicFormId.HasValue ?
                    dynamicFormService.GetControls(dynamicFormService.GetInfo(dynamicFormId.Value)).Select(c => new QueryModel(c.Key, c.Value)).ToList() : new List<QueryModel>(),
                    Model = designCode
                };
        }

        [HttpPost]
        public object PostLoadWebServiceForm(PostLoadWebServiceFormDTO model)
        {
            Guid? dynamicFormId = model.DynamicFormId.ToGuidObjNull();
            DCWebServiceModel designCode = null;
            if (!string.IsNullOrWhiteSpace(model.XmlB64Model))
            {
                designCode = DesignCodeUtility.GetObjectOfDesignCode<DCWebServiceModel>(model.XmlB64Model.ToStringObj().FromBase64());
                designCode.IsOutputYes = model.IsOutputYes;
            }
            else
                designCode = new DCWebServiceModel(Guid.NewGuid().ToString(), "وب سرویس", string.Empty, string.Empty
                     , model.ShapeId.ToStringObj(), model.ParentShapeId.ToStringObj(),
                      model.IsOutputYes, null, model.IsFirst.ToBoolObj(), string.Empty);

            using (DynamicFormService dynamicFormService = new DynamicFormService())
                return new
                {
                    ListMethodTypes = EnumObjHelper.GetEnumList<DCWebServiceModel.e_MethodType>().Select(c => new QueryModel(c.Key.ToString(), c.Value)).ToList(),
                    ListContentTypes = EnumObjHelper.GetEnumList<DCWebServiceModel.e_ContentType>().Select(c => new QueryModel(c.Key.ToString(), c.Value)).ToList(),
                    DynamicFormId = dynamicFormId,
                    ProcessControls = dynamicFormId != Guid.Empty && dynamicFormId.HasValue ?
                    dynamicFormService.GetControls(dynamicFormService.GetInfo(dynamicFormId.Value)).Select(c => new QueryModel(c.Key, c.Value)).ToList() : new List<QueryModel>(),
                    Model = designCode
                };
        }

        [HttpPost]
        public object PostLoadEntityForm(PostLoadEntityFormDTO model)
        {
            Guid? dynamicFormId = model.DynamicFormId.ToGuidObjNull();
            DCEntityModel designCode = null;
            if (!string.IsNullOrWhiteSpace(model.XmlB64Model))
            {
                designCode = DesignCodeUtility.GetObjectOfDesignCode<DCEntityModel>(model.XmlB64Model.ToStringObj().FromBase64());
                designCode.IsOutputYes = model.IsOutputYes;
            }
            else
                designCode = new DCEntityModel(Guid.NewGuid().ToString(), string.Empty, model.ShapeId.ToStringObj(),
                    model.ParentShapeId.ToStringObj(), model.IsOutputYes.ToBoolObjNull(),
                    new List<DCEntityParametersModel>(), model.IsFirst.ToBoolObj(), model.DefaultMethodType);

            using (EntityDefService entityDefService = new EntityDefService())
            {
                var listENtities = entityDefService.GetList(null, null, true);
                if (designCode.EntityDefID != Guid.Empty)
                {
                    designCode.Rows.ForEach((item) =>
                     {
                         item.IsRequired = listENtities.FirstOrDefault(c => c.ID == designCode.EntityDefID).AllProperties.FirstOrDefault(d => d.Name == item.ParameterName).Required;
                     });
                }
                using (DynamicFormService dynamicFormService = new DynamicFormService())
                    return new
                    {
                        ListEntities = listENtities.Select(c => new EntityDefDTO(c)).ToList(),
                        DynamicFormId = dynamicFormId,
                        ProcessControls = dynamicFormId != Guid.Empty && dynamicFormId.HasValue ?
                        dynamicFormService.GetControls(dynamicFormService.GetInfo(dynamicFormId.Value)).Select(c => new QueryModel(c.Key, c.Value)).ToList() : new List<QueryModel>(),
                        Model = designCode
                    };
            }
        }

        [HttpGet]
        public List<string> GetOperationParameters(Guid operationId)
        {
            using (OperationService operationService = new OperationService())
            {
                return operationService.GetParameters(operationId);
            }
        }

        [HttpGet]
        public List<EntityPropertyModel> GetEntityProperties(Guid ID)
        {
            using (EntityDefService entityDefService = new EntityDefService())
            {
                sysBpmsEntityDef sysBpmsEntityDef1 = entityDefService.GetInfo(ID);
                return sysBpmsEntityDef1.AllProperties;
            }
        }

        /// <summary>
        /// it render the xml code then return it.
        /// </summary>
        /// <param name="xmlCode"><ArrayOfObjects>....</ArrayOfObjects></param>
        [HttpPost]
        public object DoRenderCode(DoRenderCodeDTO doRenderCode)
        {
            var data = DesignCodeUtility.RenderObjectsToCode(doRenderCode.XmlCode.ToStringObj(),
                new UnitOfWork(), doRenderCode.ProcessId, doRenderCode.ApplicationPageId, doRenderCode.OnlyConditional, doRenderCode.AddGotoLabel.ToBoolObjNull() ?? true);
            return Json(new
            {
                Code = data.Item1,
                Assemblies = data.Item2,
            });
        }

        /// <summary>
        /// it is called from _FormGenerator for filling all variable combotrees
        /// </summary>
        [HttpGet]
        public object GetVariableData()
        {
            return new { variable = new VariableService().GetVariableAsComboTree(base.ProcessId, base.ApplicationPageId, null).AsJson() };
        }

        #region .:: Private ::.

        private void InitialData(DesignCodeDTO codeDTO, Guid? dynamicFormId = null)
        {
            //codeDTO.DesignCodeData = (string.Join(",", codeDTO.CodeObjects.Select(c => string.Format("{{data:'{0}',shapeId:'{1}',id:'{2}'}}", (DesignCodeUtility.ConvertDesignCodeObjectToXml((c)).ToBase64()), ((DCBaseModel)c).ShapeID, ((DCBaseModel)c).ID))));
            codeDTO.DesignCodeData = codeDTO.CodeObjects.Select(c => new { data = DesignCodeUtility.ConvertDesignCodeObjectToXml((c)).ToBase64(), shapeId = ((DCBaseModel)c).ShapeID, id = ((DCBaseModel)c).ID }).ToList();
            using (VariableService variableService = new VariableService())
                codeDTO.ProcessVariables = variableService.GetVariableAsComboTree(base.ProcessId, base.ApplicationPageId, null, "{0}");
            codeDTO.AssembliesJson = Directory.Exists(BPMSResources.FilesRoot + BPMSResources.AssemblyRoot) ? new DirectoryInfo(BPMSResources.FilesRoot + BPMSResources.AssemblyRoot).GetFiles("*.dll").Select(c => new AssemblyDTO(c)).Select(c => new ComboTreeModel() { id = c.FileName, title = c.FileName }).ToList() : new List<ComboTreeModel>();
            using (DynamicFormService dynamicFormService = new DynamicFormService())
                codeDTO.ApplicationPages = dynamicFormService.GetList(null, null, true, string.Empty, null, null).Select(c => new QueryModel(c.ApplicationPageID.ToString(), c.Name)).ToList();

            using (LURowService luRowService = new LURowService())
                codeDTO.DepartmentRoles = luRowService.GetList("DepartmentRoleLU").Select(c => new LURowDTO(c)).ToList();
            using (DepartmentService departmentService = new DepartmentService())
                codeDTO.DepartmentList = departmentService.GetList(true, "", null).Select(c => new QueryModel($"new Guid(\"{ c.ID }\")", c.Name)).ToList();
            using (DocumentFolderService documentFolderService = new DocumentFolderService())
                codeDTO.DocumentFolders = documentFolderService.GetList(null, "", "", true).Select(c => new DocumentFolderDTO(c)).ToList();
            using (DynamicFormService dynamicFormService = new DynamicFormService())
            {
                if (dynamicFormId != Guid.Empty && dynamicFormId.HasValue)
                    codeDTO.ProcessControls = dynamicFormService.GetControls(dynamicFormService.GetInfo(dynamicFormId.Value)).Select(c => new QueryModel(c.Key, c.Value)).ToList();
                else
                      if (base.ApplicationPageId.HasValue)
                    codeDTO.ProcessControls = dynamicFormService.GetControls(dynamicFormService.GetInfoByPageID(base.ApplicationPageId.Value)).Select(c => new QueryModel(c.Key, c.Value)).ToList();
                else
                    codeDTO.ProcessControls = new List<QueryModel>();
            }
        }

        #endregion
    }
}