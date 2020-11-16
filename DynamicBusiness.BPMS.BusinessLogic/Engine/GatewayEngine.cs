using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class GatewayEngine : BaseEngine
    {
        public GatewayEngine(EngineSharedModel engineSharedModel, IUnitOfWork unitOfWork = null) : base(engineSharedModel, unitOfWork) { }

        public (List<sysBpmsSequenceFlow> flowsList, bool result) CheckGateway(sysBpmsGateway gateway, sysBpmsSequenceFlow joinFromSequenceFlow, Guid? threadTaskID)
        {
            SequenceFlowService sequenceFlowService = new SequenceFlowService(this.UnitOfWork);
            List<sysBpmsSequenceFlow> acceptedForkSequenceFlows = new List<sysBpmsSequenceFlow>();

            ConditionService conditionService = new ConditionService(this.UnitOfWork);

            switch ((sysBpmsGateway.e_TypeLU)gateway.TypeLU)
            {
                case sysBpmsGateway.e_TypeLU.ExclusiveGateWay:
                    {
                        List<sysBpmsCondition> listCondition = conditionService.GetList(gateway.ID, null, null);
                        foreach (sysBpmsCondition condition in listCondition)
                        {
                            if (new DynamicCodeEngine(base.EngineSharedModel, this.UnitOfWork).ExecuteBooleanCode(DesignCodeUtility.GetDesignCodeFromXml(condition.Code)))
                            {
                                acceptedForkSequenceFlows.Add(sequenceFlowService.GetInfo(condition.SequenceFlowID.Value));
                                break;
                            }
                        }
                    }
                    break;
                case sysBpmsGateway.e_TypeLU.InclusiveGateWay:
                    {
                        List<sysBpmsThreadTask> listRunningThreadTask = new ThreadTaskService(base.UnitOfWork).GetListRunning(base.EngineSharedModel.CurrentThreadID.ToGuidObj());
                        if (!listRunningThreadTask.Any(c => c.ID != threadTaskID && gateway.TraceToStart.ToStringObj().Split(',').Contains(c.sysBpmsTask.ElementID)))
                        {
                            List<sysBpmsCondition> listCondition = conditionService.GetList(gateway.ID, null, null);
                            foreach (sysBpmsCondition condition in listCondition)
                            {
                                if (new DynamicCodeEngine(base.EngineSharedModel, this.UnitOfWork).ExecuteBooleanCode(DesignCodeUtility.GetDesignCodeFromXml(condition.Code)))
                                {
                                    acceptedForkSequenceFlows.Add(sequenceFlowService.GetInfo(condition.SequenceFlowID.Value));
                                }
                            }
                        }
                        else
                            return (new List<sysBpmsSequenceFlow>(), true);
                    }
                    break;
                case sysBpmsGateway.e_TypeLU.ParallelGateWay:
                    {
                        List<sysBpmsSequenceFlow> listJoinSequenceFlow = sequenceFlowService.GetList(base.EngineSharedModel.CurrentProcessID.Value, gateway.ElementID, "", "");
                        //if all sequence flow were executed, run condition code and clear gateway Status Xml List
                        if (listJoinSequenceFlow.Where(c => c.ID != joinFromSequenceFlow.ID).All(c => base.EngineSharedModel.CurrentThread.GatewayStatus.Any(d => d.GatewayID == gateway.ID && d.List.Any(f => f.SequenceFlowID == c.ID && f.Done))) || base.EngineSharedModel.CurrentThread == null)
                        {
                            acceptedForkSequenceFlows = sequenceFlowService.GetList(base.EngineSharedModel.CurrentProcessID.Value, "", gateway.ElementID, "");
                            new ThreadService(base.UnitOfWork).ClearGatewayStatusXml(base.EngineSharedModel.CurrentThread, gateway.ID);
                        }
                        else
                        {
                            //add this sequence flow path to executed path in Thread.GatewayStatusXml
                            new ThreadService(base.UnitOfWork).AddGatewayStatusXml(base.EngineSharedModel.CurrentThread, gateway.ID, joinFromSequenceFlow.ID);
                            return (new List<sysBpmsSequenceFlow>(), true);
                        }
                    }
                    break;
            }

            if (!acceptedForkSequenceFlows.Any() && gateway.DefaultSequenceFlowID.HasValue)
                acceptedForkSequenceFlows.Add(sequenceFlowService.GetInfo(gateway.DefaultSequenceFlowID.Value));

            return (acceptedForkSequenceFlows, acceptedForkSequenceFlows.Any());
        }

        /// <summary>
        /// used for finding all tasks from an inclusive gateway element to start event.
        /// </summary>
        /// <param name="element">first time it is inclusive gateway</param>
        private void GetRecursiveTraceToStart(sysBpmsElement element, List<string> listTaskID,
            List<string> listEvaluatedID, List<sysBpmsElement> listElement,
            List<sysBpmsSequenceFlow> listSequenceFlow, List<string> listTracedItems)
        {
            if (listEvaluatedID.Contains(element.ID))
                return;
            if (element.TypeLU == (int)sysBpmsElement.e_TypeLU.Event)
            {
                sysBpmsEvent sysBpmsEvent = new EventService().GetInfo(element.ID, element.ProcessID);
                if (sysBpmsEvent.TypeLU != (int)sysBpmsEvent.e_TypeLU.IntermediateThrow || sysBpmsEvent.TypeLU != (int)sysBpmsEvent.e_TypeLU.IntermediateCatch)
                    return;
            }
            if (element.TypeLU == (int)sysBpmsElement.e_TypeLU.Task)
            {
                if (!listTaskID.Contains(element.ID))
                    listTaskID.Add(element.ID);
            }
            if (element.TypeLU == (int)sysBpmsElement.e_TypeLU.Event ||
                element.TypeLU == (int)sysBpmsElement.e_TypeLU.Gateway)
            {
                listTracedItems.Add(element.ID);
            }
            foreach (sysBpmsSequenceFlow item in listSequenceFlow.Where(c => c.SourceElementID != element.ID && c.TargetElementID == element.ID))
            {
                if (!listTracedItems.Any(c => c == item.SourceElementID))
                    this.GetRecursiveTraceToStart(listElement.FirstOrDefault(c => c.ID == item.SourceElementID), listTaskID, listEvaluatedID, listElement, listSequenceFlow, listTracedItems);
            }
        }

        public ResultOperation UpdateTraceToStart(sysBpmsProcess process)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                List<sysBpmsGateway> listGateways = new GatewayService(base.UnitOfWork).GetList(process.ID).ToList();
                if (listGateways.Any())
                {
                    List<sysBpmsElement> listElements = new ElementService(base.UnitOfWork).GetList(process.ID, null, "");
                    List<sysBpmsSequenceFlow> listSequenceFlow = new SequenceFlowService(base.UnitOfWork).GetList(process.ID, "", "", "");
                    this.BeginTransaction();
                    foreach (sysBpmsGateway item in listGateways)
                    {
                        if (resultOperation.IsSuccess)
                        {
                            List<string> listEvaluatedID = new List<string>();
                            List<string> listTaskID = new List<string>();
                            List<string> listTracedItems = new List<string>();
                            this.GetRecursiveTraceToStart(listElements.FirstOrDefault(c => c.ID == item.ElementID), listTaskID, listEvaluatedID, listElements, listSequenceFlow, listTracedItems);

                            item.TraceToStart = string.Join(",", listTaskID);
                            resultOperation = new GatewayService(base.UnitOfWork).Update(item);
                        }
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

    }
}
