using DynamicBusiness.BPMS.Domain;
using Mono.CSharp.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ProcessService : ServiceBase
    {
        public ProcessService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(string Name, string Description, string userName, int? parallelCountPerUser, Guid processGroupId, int typeLU)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                sysBpmsProcess process = new sysBpmsProcess()
                {
                    CreateDate = DateTime.Now,
                    Name = Name,
                    Description = Description,
                    UpdateDate = DateTime.Now,
                    FormattedNumber = string.Empty,
                    Number = null,
                    StatusLU = (int)sysBpmsProcess.Enum_StatusLU.Draft,
                    ProcessVersion = (int)sysBpmsProcess.Enum_Version.startVersion,
                    WorkflowXML = string.Empty,
                    DiagramXML = this.GetDefaultXML(),
                    SourceCode = string.Empty,
                    CreatorUsername = userName,
                    BeginTasks = string.Empty,
                    ParallelCountPerUser = parallelCountPerUser,
                    ProcessGroupID = processGroupId,
                    TypeLU = typeLU,
                };
                process.Number = this.CalculateSerlialNumber(this.UnitOfWork.Repository<IProcessRepository>().MaxNumber() + 1);
                process.FormattedNumber = this.CalculateFormatNumber(process.Number.Value, DateTime.Now.Date);
                if (resultOperation.IsSuccess)
                {
                    this.BeginTransaction();

                    this.UnitOfWork.Repository<IProcessRepository>().Add(process);
                    this.UnitOfWork.Save();

                    //process.Code = process.ID.ToString();
                    this.UnitOfWork.Repository<IProcessRepository>().Update(process);
                    this.UnitOfWork.Save();

                    resultOperation.CurrentObject = process;
                }
            }
            catch (Exception ex)
            {
                return base.ExceptionHandler(ex);
            }
            base.FinalizeService(resultOperation);

            return resultOperation;
        }

        public ResultOperation Update(sysBpmsProcess process)
        {
            ResultOperation resultOperation;
            try
            {
                resultOperation = new ResultOperation();
                if (!process.AllowEdit() &&
                    process.WorkflowXML.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n", "").Replace("\r\n", "").Replace(" ", "") != this.GetInfo(process.ID).WorkflowXML.Replace(" ", ""))
                {
                    resultOperation.AddError(LangUtility.Get("NotAllowEdit.Text", nameof(sysBpmsProcess)));
                }
                if (resultOperation.IsSuccess)
                {
                    this.BeginTransaction();
                    if (process.StatusLU == (int)sysBpmsProcess.Enum_StatusLU.Published)
                        process.BeginTasks = string.Join(",", this.GetListBeginTaskElementID(process.ID));
                    process.UpdateDate = DateTime.Now;
                    this.UnitOfWork.Repository<IProcessRepository>().Update(process);
                    resultOperation = this.UpdateProcessDataBase(process);
                    if (resultOperation.IsSuccess)
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

        public ResultOperation UpdateInfo(sysBpmsProcess process)
        {
            ResultOperation resultOperation;
            try
            {
                resultOperation = new ResultOperation();
                if (resultOperation.IsSuccess)
                {
                    this.BeginTransaction();
                    this.UnitOfWork.Repository<IProcessRepository>().Update(process);
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

        public ResultOperation InActive(sysBpmsProcess process)
        {
            ResultOperation resultOperation;
            try
            {
                resultOperation = new ResultOperation();
                if (resultOperation.IsSuccess)
                {
                    if (process.StatusLU != (int)sysBpmsProcess.Enum_StatusLU.Published)
                        resultOperation.AddError(LangUtility.Get("InActiveError.Text", nameof(sysBpmsProcess)));
                    if (resultOperation.IsSuccess)
                    {
                        process.StatusLU = (int)sysBpmsProcess.Enum_StatusLU.Inactive;
                        this.UnitOfWork.Repository<IProcessRepository>().Update(process);
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

        public ResultOperation Active(sysBpmsProcess process)
        {
            ResultOperation resultOperation = null;
            try
            {
                resultOperation = new ResultOperation();
                if (resultOperation.IsSuccess)
                {
                    if (process.StatusLU != (int)sysBpmsProcess.Enum_StatusLU.Inactive)
                        resultOperation.AddError(LangUtility.Get("ActiveError.Text", nameof(sysBpmsProcess)));
                    if (resultOperation.IsSuccess)
                    {
                        process.StatusLU = (int)sysBpmsProcess.Enum_StatusLU.Published;
                        this.UnitOfWork.Repository<IProcessRepository>().Update(process);
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

        public ResultOperation NewVersion(sysBpmsProcess current, string userName)
        {
            ResultOperation resultOperation;
            try
            {
                resultOperation = new ResultOperation();
                if (resultOperation.IsSuccess)
                {
                    if (this.GetList((int)sysBpmsProcess.Enum_StatusLU.Draft, current.ParentProcessID ?? current.ID).Any())
                    {
                        resultOperation.AddError(LangUtility.Get("NewVersionError.Text", nameof(sysBpmsProcess)));
                        return resultOperation;
                    }
                    this.BeginTransaction();

                    //Create new process
                    sysBpmsProcess newProcess = this.GetInfo(current.ID);
                    newProcess.CreateDate = DateTime.Now;
                    newProcess.UpdateDate = DateTime.Now;
                    newProcess.StatusLU = (int)sysBpmsProcess.Enum_StatusLU.Draft;
                    newProcess.ProcessVersion = newProcess.ProcessVersion + 1;
                    newProcess.ParentProcessID = current.ParentProcessID.HasValue ? current.ParentProcessID.Value : current.ID;
                    newProcess.Number = this.CalculateSerlialNumber(this.UnitOfWork.Repository<IProcessRepository>().MaxNumber() + 1);
                    newProcess.FormattedNumber = this.CalculateFormatNumber(newProcess.Number.Value, DateTime.Now.Date);

                    this.UnitOfWork.Repository<IProcessRepository>().Add(newProcess);
                    this.UnitOfWork.Save();
                    //Copy Variable
                    Dictionary<Guid, Guid> variableConvertID = new Dictionary<Guid, Guid>();
                    VariableService variableService = new VariableService(base.UnitOfWork);
                    List<sysBpmsVariable> sysBpmsVariableList = variableService.GetList(current.ID, null, null, "", null, null);
                    if (resultOperation.IsSuccess)
                        foreach (sysBpmsVariable item in sysBpmsVariableList)
                        {
                            Guid oldGuidID = item.ID;
                            item.ProcessID = newProcess.ID;
                            resultOperation = variableService.Add(item, null);
                            variableConvertID.Add(oldGuidID, item.ID);
                            if (!resultOperation.IsSuccess)
                                break;
                        }
                    //Copy VariableDependency
                    VariableDependencyService variableDependencyService = new VariableDependencyService(base.UnitOfWork);
                    List<sysBpmsVariableDependency> sysBpmsVariableDependencyList = variableDependencyService.GetList(current.ID);
                    if (resultOperation.IsSuccess)
                        foreach (sysBpmsVariableDependency item in sysBpmsVariableDependencyList)
                        {
                            if (item.ToVariableID.HasValue)
                                item.ToVariableID = variableConvertID[item.ToVariableID.Value];
                            item.DependentVariableID = variableConvertID[item.DependentVariableID];
                            resultOperation = variableDependencyService.Add(item);
                            if (!resultOperation.IsSuccess)
                                break;
                        }
                    //Copy Element
                    ElementService elementService = new ElementService(base.UnitOfWork);
                    List<sysBpmsElement> sysBpmsElementList = elementService.GetList(current.ID, null, "");
                    if (resultOperation.IsSuccess)
                        foreach (sysBpmsElement item in sysBpmsElementList)
                        {
                            item.ProcessID = newProcess.ID;
                            item.sysBpmsProcess = null;
                            resultOperation = elementService.Add(item);
                            if (!resultOperation.IsSuccess)
                                break;
                        }
                    //Copy Lane
                    LaneService laneService = new LaneService(base.UnitOfWork);
                    List<sysBpmsLane> sysBpmsLaneList = laneService.GetList(current.ID);
                    if (resultOperation.IsSuccess)
                        foreach (sysBpmsLane item in sysBpmsLaneList)
                        {
                            item.ID = Guid.NewGuid();
                            item.ProcessID = newProcess.ID;
                            item.sysBpmsElement = null;
                            laneService.Add(item);
                        }
                    //Copy event
                    EventService eventService = new EventService(base.UnitOfWork);
                    List<sysBpmsEvent> sysBpmsEventList = eventService.GetList(null, current.ID, "", null, null);
                    if (resultOperation.IsSuccess)
                        foreach (sysBpmsEvent item in sysBpmsEventList)
                        {
                            item.ID = Guid.NewGuid();
                            item.ProcessID = newProcess.ID;
                            item.sysBpmsElement = null;
                            resultOperation = eventService.Add(item);
                            if (!resultOperation.IsSuccess)
                                break;
                        }
                    //Copy SequenceFlow
                    Dictionary<Guid, Guid> flowConvertID = new Dictionary<Guid, Guid>();
                    SequenceFlowService sequenceFlowService = new SequenceFlowService(base.UnitOfWork);
                    List<sysBpmsSequenceFlow> sysBpmsSequenceFlowList = sequenceFlowService.GetList(current.ID);
                    if (resultOperation.IsSuccess)
                        foreach (sysBpmsSequenceFlow item in sysBpmsSequenceFlowList)
                        {
                            Guid flowNewId = Guid.NewGuid();
                            flowConvertID.Add(item.ID, flowNewId);
                            item.ID = flowNewId;
                            item.ProcessID = newProcess.ID;
                            item.sysBpmsProcess = null;
                            item.sysBpmsElement = null;
                            resultOperation = sequenceFlowService.Add(item);
                            if (!resultOperation.IsSuccess)
                                break;
                        }
                    //Copy gateway
                    GatewayService gatewayService = new GatewayService(base.UnitOfWork);
                    ConditionService conditionService = new ConditionService(base.UnitOfWork);
                    List<sysBpmsGateway> sysBpmsGatewayList = gatewayService.GetList(current.ID);
                    if (resultOperation.IsSuccess)
                        foreach (sysBpmsGateway item in sysBpmsGatewayList)
                        {
                            List<sysBpmsCondition> sysBpmsConditionList = conditionService.GetList(item.ID, null, null);
                            item.ID = Guid.NewGuid();
                            item.ProcessID = newProcess.ID;
                            item.sysBpmsElement = null;
                            if (item.DefaultSequenceFlowID.HasValue)
                                item.DefaultSequenceFlowID = flowConvertID[item.DefaultSequenceFlowID.Value];
                            resultOperation = gatewayService.Add(item);
                            if (!resultOperation.IsSuccess)
                                break;
                            foreach (sysBpmsCondition condition in sysBpmsConditionList)
                            {
                                condition.GatewayID = item.ID;
                                if (condition.SequenceFlowID.HasValue)
                                    condition.SequenceFlowID = flowConvertID[condition.SequenceFlowID.Value];
                                condition.sysBpmsGateway = null;
                                condition.sysBpmsSequenceFlow = null;
                                resultOperation = conditionService.Add(condition);
                                if (!resultOperation.IsSuccess)
                                    break;
                            }
                        }
                    //Copy dynamicForm
                    Dictionary<Guid, Guid> formConvertID = new Dictionary<Guid, Guid>();
                    DynamicFormService dynamicFormService = new DynamicFormService(base.UnitOfWork);
                    ApplicationPageService applicationPageService = new ApplicationPageService(base.UnitOfWork);
                    List<sysBpmsDynamicForm> sysBpmsDynamicFormList = dynamicFormService.GetList(current.ID, null, null, "", null, null);
                    if (resultOperation.IsSuccess)
                        foreach (sysBpmsDynamicForm item in sysBpmsDynamicFormList)
                        {
                            Guid oldID = item.ID;
                            item.ProcessId = newProcess.ID;
                            //First change code id. 
                            dynamicFormService.UpdateBackendCodeID(item);
                            //Then update sourceCode value.
                            dynamicFormService.GetSourceCode(item);
                            item.sysBpmsProcess = null;
                            resultOperation = dynamicFormService.Add(item,
                                item.ApplicationPageID.HasValue ? applicationPageService.GetInfo(item.ApplicationPageID.Value) : null, userName);
                            formConvertID.Add(oldID, item.ID);
                            if (!resultOperation.IsSuccess)
                                break;
                        }
                    //Update formHtml control formid property
                    if (resultOperation.IsSuccess)
                        foreach (sysBpmsDynamicForm item in sysBpmsDynamicFormList)
                        {
                            if (item.DesignJson.Contains("formId"))
                            {
                                formConvertID.ToList().ForEach(c =>
                                {
                                    item.DesignJson = item.DesignJson.Replace(c.Key.ToString(), c.Value.ToString());
                                });
                                resultOperation = dynamicFormService.Update(item, userName);
                                if (!resultOperation.IsSuccess)
                                    break;
                            }
                        }
                    //Copy task
                    TaskService taskService = new TaskService(base.UnitOfWork);
                    StepService stepService = new StepService(base.UnitOfWork);
                    List<sysBpmsTask> sysBpmsTaskList = taskService.GetList(null, current.ID);
                    if (resultOperation.IsSuccess)
                        foreach (sysBpmsTask item in sysBpmsTaskList)
                        {
                            List<sysBpmsStep> sysBpmsStepList = stepService.GetList(item.ID, null);
                            item.ID = Guid.NewGuid();
                            item.ProcessID = newProcess.ID;
                            item.sysBpmsProcess = null;
                            item.sysBpmsElement = null;
                            resultOperation = taskService.Add(item);
                            if (!resultOperation.IsSuccess)
                                break;
                            foreach (sysBpmsStep step in sysBpmsStepList)
                            {
                                step.TaskID = item.ID;
                                if (step.DynamicFormID.HasValue)
                                    step.DynamicFormID = formConvertID[step.DynamicFormID.Value];
                                step.sysBpmsDynamicForm = null;
                                step.sysBpmsTask = null;
                                resultOperation = stepService.Add(step);
                                if (!resultOperation.IsSuccess)
                                    break;
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

        public ResultOperation Delete(Guid processId)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                this.BeginTransaction();
                sysBpmsProcess process = this.GetInfo(processId);
                if (this.GetList(null, process.ID).Any())
                {
                    resultOperation.AddError(LangUtility.Get("OldVersionDeleteError.Text", nameof(sysBpmsProcess)));
                }
                int countThread = new ThreadService().GetCount(process.ID);
                if (countThread > 0)
                {
                    resultOperation.AddError(string.Format(LangUtility.Get("DeleteProcessThreadError.Text", nameof(sysBpmsProcess)), countThread));
                }
                if (resultOperation.IsSuccess)
                {
                    //Delete Gateway
                    foreach (sysBpmsGateway gateway in new GatewayService(base.UnitOfWork).GetList(processId))
                    {
                        if (resultOperation.IsSuccess)
                            resultOperation = new GatewayService(base.UnitOfWork).Delete(gateway.ID);
                    }

                    //Delete Task
                    foreach (sysBpmsTask task in new TaskService(base.UnitOfWork).GetList(null, processId))
                    {
                        if (resultOperation.IsSuccess)
                            resultOperation = new TaskService(base.UnitOfWork).Delete(task.ID);
                    }

                    //Delete Event
                    foreach (sysBpmsEvent @event in new EventService(base.UnitOfWork).GetList(null, processId, "", null))
                    {
                        if (resultOperation.IsSuccess)
                            resultOperation = new EventService(base.UnitOfWork).Delete(@event.ID);
                    }

                    //Delete DynamicForm
                    foreach (sysBpmsDynamicForm dynamicForm in new DynamicFormService(base.UnitOfWork).GetList(processId, null, null, "", null, null))
                    {
                        if (resultOperation.IsSuccess)
                            resultOperation = new DynamicFormService(base.UnitOfWork).Delete(dynamicForm.ID);
                    }

                    //Delete Variable
                    foreach (sysBpmsVariable variable in new VariableService(base.UnitOfWork).GetList(processId, null, null, "", null, null))
                    {
                        if (resultOperation.IsSuccess)
                            resultOperation = new VariableService(base.UnitOfWork).Delete(variable.ID);
                    }

                    //Delete Process
                    if (resultOperation.IsSuccess)
                    {
                        this.UnitOfWork.Repository<IProcessRepository>().Delete(processId);
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

        public sysBpmsProcess GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<IProcessRepository>().GetInfo(ID);
        }

        public List<sysBpmsProcess> GetList(DateTime? startDate, DateTime? endDate, Guid? processGroupId, PagingProperties currentPaging)
        {
            return this.UnitOfWork.Repository<IProcessRepository>().GetList(startDate, endDate, processGroupId, currentPaging);
        }

        public List<sysBpmsProcess> GetList(int? statusLU, Guid? parentProcessId)
        {
            return this.UnitOfWork.Repository<IProcessRepository>().GetList(statusLU, parentProcessId);
        }

        public sysBpmsProcess GetLastActive(Guid parentProcessId)
        {
            return this.UnitOfWork.Repository<IProcessRepository>().GetLastActive(parentProcessId);
        }

        public ResultOperation Publish(Guid processID)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                sysBpmsProcess process = this.GetInfo(processID);
                if (process.StatusLU != (int)sysBpmsProcess.Enum_StatusLU.Draft &&
                    process.StatusLU != (int)sysBpmsProcess.Enum_StatusLU.Inactive)
                {
                    resultOperation.AddError(LangUtility.Get("WorkflowPublishError.Text", nameof(sysBpmsProcess)));
                }

                if (new EventService(base.UnitOfWork).GetList((int)sysBpmsEvent.e_TypeLU.StartEvent, processID, "", null).Count == 0)
                    resultOperation.AddError(LangUtility.Get("WorkflowPublishNoStartError.Text", nameof(sysBpmsProcess)));

                if (new EventService(base.UnitOfWork).GetList((int)sysBpmsEvent.e_TypeLU.EndEvent, processID, "", null).Count == 0)
                    resultOperation.AddError(LangUtility.Get("WorkflowPublishNoEndError.Text", nameof(sysBpmsProcess)));

                foreach (var item in new TaskService(base.UnitOfWork).GetList((int)sysBpmsTask.e_TypeLU.UserTask, processID))
                {
                    if (item.sysBpmsSteps.Count == 0)
                        resultOperation.AddError(LangUtility.Get("WorkflowPublishNoStepError.Text", nameof(sysBpmsProcess)));
                }
                if (resultOperation.IsSuccess)
                {
                    process.BeginTasks = string.Join(",", this.GetListBeginTaskElementID(process.ID));

                    this.BeginTransaction();
                    if (process.ParentProcessID.HasValue)
                    {
                        sysBpmsProcess parent = this.GetLastActive(process.ParentProcessID.Value);
                        //Make parent process as old version.
                        if (parent.StatusLU != (int)sysBpmsProcess.Enum_StatusLU.OldVersion)
                        {
                            parent.StatusLU = (int)sysBpmsProcess.Enum_StatusLU.OldVersion;
                            this.UnitOfWork.Repository<IProcessRepository>().Update(parent);
                        }
                    }
                    process.StatusLU = (int)sysBpmsProcess.Enum_StatusLU.Published;
                    process.PublishDate = DateTime.Now;
                    //Update sourceCode fired and Generate assembly
                    GetSourceCode(process);
                    //Update TraceToStartField of Gateway element.
                    resultOperation = new GatewayEngine(null, base.UnitOfWork).UpdateTraceToStart(process);

                    if (resultOperation.IsSuccess)
                    {
                        this.UnitOfWork.Repository<IProcessRepository>().Update(process);
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

        /// <summary>
        /// this method returns tasks that are first user task after start events by evaluating all outputs of elements.
        /// </summary>
        public List<string> GetListBeginTaskElementID(Guid ID)
        {
            return this.UnitOfWork.Repository<IProcessRepository>().GetListBeginTaskElementID(ID);
        }

        //It will GetSourceCode and generate assemblies
        public void GetSourceCode(sysBpmsProcess sysBpmsProcess)
        {

            string makeClass(DesignCodeModel designCode)
            {
                string code = string.Empty;
                if (designCode != null && !string.IsNullOrWhiteSpace(designCode.Code))
                {
                    code = DynamicCodeEngine.MakeClass(designCode.Code, designCode.ID);
                }
                return code;
            }
            string sourceCode = string.Empty;
            DynamicFormService dynamicFormService = new DynamicFormService(base.UnitOfWork);
            List<sysBpmsTask> sysBpmsTaskList = new TaskService(base.UnitOfWork).GetList(null, sysBpmsProcess.ID);
            List<sysBpmsCondition> sysBpmsConditionList = new ConditionService(base.UnitOfWork).GetList(null, null, sysBpmsProcess.ID);

            List<sysBpmsDynamicForm> sysBpmsDynamicFormList = dynamicFormService.GetList(sysBpmsProcess.ID, null, null, "", null, null);
            foreach (var item in sysBpmsDynamicFormList)
            {
                if (!string.IsNullOrWhiteSpace(item.SourceCode))
                {
                    sourceCode += item.SourceCode + Environment.NewLine;
                }
            }

            foreach (var item in sysBpmsTaskList)
            {
                //Service and script task
                if (!string.IsNullOrWhiteSpace(item.Code))
                    sourceCode += makeClass(DesignCodeUtility.GetDesignCodeFromXml(item.Code));
                //Rule
                if (!string.IsNullOrWhiteSpace(item.Rule))
                    sourceCode += makeClass(DesignCodeUtility.GetDesignCodeFromXml(item.Rule));
            }

            foreach (var item in sysBpmsConditionList)
            {
                if (!string.IsNullOrWhiteSpace(item.Code))
                    sourceCode += makeClass(DesignCodeUtility.GetDesignCodeFromXml(item.Code));
            }

            sysBpmsProcess.SourceCode = sourceCode;
            DynamicCodeEngine.GenerateProcessAssembly(sysBpmsProcess);
        }

        #region .:: Private Methods ::.

        private ResultOperation UpdateProcessDataBase(sysBpmsProcess Process)
        {
            ResultOperation resultOperation = new ResultOperation();
            WorkflowProcess _WorkflowProcess = new XmlWorkflowProcessConvertor().ConvertFromString(Process.WorkflowXML);
            //Lane
            new LaneService(this.UnitOfWork).Update(Process.ID, _WorkflowProcess);
            //Task
            resultOperation = new TaskService(this.UnitOfWork).Update(Process.ID, _WorkflowProcess);
            if (!resultOperation.IsSuccess)
                return resultOperation;
            //Event
            resultOperation = new EventService(this.UnitOfWork).Update(Process.ID, _WorkflowProcess);
            if (!resultOperation.IsSuccess)
                return resultOperation;
            //GateWay
            resultOperation = new GatewayService(this.UnitOfWork).Update(Process.ID, _WorkflowProcess);
            if (!resultOperation.IsSuccess)
                return resultOperation;
            //SequenceFlow : this must be at the end of the others 
            resultOperation = new SequenceFlowService(this.UnitOfWork).Update(Process.ID, _WorkflowProcess);
            if (!resultOperation.IsSuccess)
                return resultOperation;

            return resultOperation;
        }

        private string GetDefaultXML()
        {
            return "<definitions xmlns=\"http://www.omg.org/spec/BPMN/20100524/MODEL\" xmlns:bpmndi=\"http://www.omg.org/spec/BPMN/20100524/DI\" xmlns:omgdc=\"http://www.omg.org/spec/DD/20100524/DC\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" targetNamespace=\"\" xsi:schemaLocation=\"http://www.omg.org/spec/BPMN/20100524/MODEL http://www.omg.org/spec/BPMN/2.0/20100501/BPMN20.xsd\"><process id=\"Process_0avyb7c\" /><bpmndi:BPMNDiagram id=\"sid-74620812-92c4-44e5-949c-aa47393d3830\"><bpmndi:BPMNPlane id=\"sid-cdcae759-2af7-4a6d-bd02-53f3352a731d\" bpmnElement=\"Process_0avyb7c\" /></bpmndi:BPMNDiagram></definitions>";
        }

        private int CalculateSerlialNumber(int NumberOf)
        {
            int StartPoint = new ConfigurationService(base.UnitOfWork).GetValue(sysBpmsConfiguration.e_NameType.ProcessStartPointSerlialNumber.ToString()).ToIntObj();
            if (NumberOf < StartPoint)
                NumberOf += StartPoint - NumberOf;
            return NumberOf;
        }

        private string CalculateFormatNumber(int NumberOf, DateTime OrderDateOf)
        {
            string SerlialNumberFormat = new ConfigurationService(base.UnitOfWork).GetValue(sysBpmsConfiguration.e_NameType.ProcessFormatSerlialNumber.ToString());
            string NumberFormat = string.Empty;
            string Formated = string.Empty;

            if (SerlialNumberFormat.IndexOf("#") >= 0)
            {
                var Ar = SerlialNumberFormat.ToArray();
                foreach (var A in Ar)
                {
                    if (A == '#')
                    {
                        NumberFormat += "0";
                        SerlialNumberFormat = SerlialNumberFormat.Remove(SerlialNumberFormat.IndexOf("#"), 1);
                    }
                    else
                        if (NumberFormat.Length > 0) break;
                }
            }

            if (NumberFormat.Length > 0)
            {
                if (NumberFormat.Length > NumberOf.ToString().Length)
                {
                    Formated = NumberFormat.Substring(0, NumberFormat.Length - NumberOf.ToString().Length);
                }
            }

            Formated = Formated.ToString() + NumberOf.ToString();
            if (SerlialNumberFormat.Contains("yyyy"))
                SerlialNumberFormat = SerlialNumberFormat.Replace("yyyy", OrderDateOf.ToString("yyyy"));
            else
                if (SerlialNumberFormat.Contains("yy"))
                SerlialNumberFormat = SerlialNumberFormat.Replace("yy", OrderDateOf.ToString("yy"));
            if (SerlialNumberFormat.Contains("mm"))
                SerlialNumberFormat = SerlialNumberFormat.Replace("mm", OrderDateOf.ToString("MM"));
            else
                if (SerlialNumberFormat.Contains("m"))
                SerlialNumberFormat = SerlialNumberFormat.Replace("m", OrderDateOf.ToString("M"));
            if (SerlialNumberFormat.Contains("dd"))
                SerlialNumberFormat = SerlialNumberFormat.Replace("dd", OrderDateOf.ToString("dd"));
            else
                if (SerlialNumberFormat.Contains("d"))
                SerlialNumberFormat = SerlialNumberFormat.Replace("d", OrderDateOf.ToString("d"));

            Formated = SerlialNumberFormat + Formated;
            return Formated;
        }

        #endregion
    }
}
