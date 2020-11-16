using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class GatewayService : ServiceBase
    {
        public GatewayService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsGateway Gateway)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IGatewayRepository>().Add(Gateway);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsGateway Gateway)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IGatewayRepository>().Update(Gateway);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(Guid GatewayId)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                if (resultOperation.IsSuccess)
                {
                    sysBpmsGateway sysBpmsGateway = this.GetInfo(GatewayId);
                    this.BeginTransaction();
                    foreach (sysBpmsCondition Condition in new ConditionService(this.UnitOfWork).GetList(GatewayId, null, null))
                    {
                        resultOperation = new ConditionService(this.UnitOfWork).Delete(Condition.ID);
                        if (!resultOperation.IsSuccess)
                            break;
                    }
                    if (resultOperation.IsSuccess)
                    {
                        this.UnitOfWork.Repository<IGatewayRepository>().Delete(GatewayId);
                        resultOperation = new ElementService(this.UnitOfWork).Delete(sysBpmsGateway.ElementID, sysBpmsGateway.ProcessID);
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

        public sysBpmsGateway GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<IGatewayRepository>().GetInfo(ID);
        }

        public sysBpmsGateway GetInfo(string elementId, Guid processId)
        {
            return this.UnitOfWork.Repository<IGatewayRepository>().GetInfo(elementId, processId);
        }

        public List<sysBpmsGateway> GetList(Guid processID)
        {
            return this.UnitOfWork.Repository<IGatewayRepository>().GetList(processID);
        }

        public List<sysBpmsGateway> GetListByDefaultSequence(Guid defaultSequenceFlowID)
        {
            return this.UnitOfWork.Repository<IGatewayRepository>().GetList(defaultSequenceFlowID);
        }

        public ResultOperation Update(Guid processID, WorkflowProcess _WorkflowProcess)
        {
            ResultOperation resultOperation = new ResultOperation();
            List<sysBpmsGateway> listGateway = this.GetList(processID);
            ElementService elementService = new ElementService(this.UnitOfWork);
            List<Guid> listDeleted = new List<Guid>();
            //Delete gateways that are not in diagram xml element.
            foreach (sysBpmsGateway item in listGateway.Where(c =>
            !_WorkflowProcess.ExclusiveGateways.Any(d => d.ID == c.ElementID) &&
            !_WorkflowProcess.InclusiveGateways.Any(d => d.ID == c.ElementID) &&
            !_WorkflowProcess.ParallelGateways.Any(d => d.ID == c.ElementID)))
            {
                resultOperation = this.Delete(item.ID);
                listDeleted.Add(item.ID);
                if (!resultOperation.IsSuccess)
                    return resultOperation;
            }
            listGateway = listGateway.Where(c => !listDeleted.Contains(c.ID)).ToList();
            //ExclusiveGateway
            foreach (WorkflowExclusiveGateway item in _WorkflowProcess.ExclusiveGateways)
            {
                sysBpmsGateway gateway = listGateway.FirstOrDefault(c => c.ElementID == item.ID);
                sysBpmsSequenceFlow newSequenceFlow = null;
                //if SequenceFlow did not add before and item.Default is not empty add it 
                if (!string.IsNullOrWhiteSpace(item.Default))
                {
                    sysBpmsSequenceFlow currentDefualt = new SequenceFlowService(this.UnitOfWork).GetInfo(item.Default, processID);
                    if (currentDefualt == null)
                    {
                        WorkflowSequenceFlow _WorkflowSequenceFlow = _WorkflowProcess.SequenceFlows.FirstOrDefault(c => c.ID == item.Default);
                        newSequenceFlow = new sysBpmsSequenceFlow()
                        {
                            TargetElementID = _WorkflowSequenceFlow.TargetRef,
                            SourceElementID = _WorkflowSequenceFlow.SourceRef,
                            Name = _WorkflowSequenceFlow.Name,
                            ElementID = _WorkflowSequenceFlow.ID,
                            ID = Guid.NewGuid(),
                            ProcessID = processID,
                        };
                    }
                    else if (gateway != null) gateway.DefaultSequenceFlowID = currentDefualt.ID;
                }
                if (gateway != null)
                {
                    if (newSequenceFlow != null)
                        gateway.sysBpmsSequenceFlow = newSequenceFlow;
                    gateway.TypeLU = (int)sysBpmsGateway.e_TypeLU.ExclusiveGateWay;
                    resultOperation = this.Update(gateway);
                    if (!resultOperation.IsSuccess)
                        return resultOperation;
                    gateway.sysBpmsElement.Name = item.Name;
                    elementService.Update(gateway.sysBpmsElement);
                }
                else
                {
                    gateway = new sysBpmsGateway()
                    {
                        ID = Guid.NewGuid(),
                        ElementID = item.ID,
                        ProcessID = processID,
                        sysBpmsSequenceFlow = newSequenceFlow,
                        TypeLU = (int)sysBpmsGateway.e_TypeLU.ExclusiveGateWay,
                        sysBpmsElement = new sysBpmsElement()
                        {
                            ID = item.ID,
                            Name = item.Name,
                            ProcessID = processID,
                            TypeLU = (int)sysBpmsElement.e_TypeLU.Gateway,
                        }
                    };
                    resultOperation = this.Add(gateway);
                }
                if (!resultOperation.IsSuccess)
                    return resultOperation;
            }

            //InclusiveGateway
            foreach (WorkflowInclusiveGateway item in _WorkflowProcess.InclusiveGateways)
            {
                sysBpmsGateway gateway = listGateway.FirstOrDefault(c => c.ElementID == item.ID);

                sysBpmsSequenceFlow newSequenceFlow = null;
                //if SequenceFlow did not add before and item.Default is not empty add it 
                if (!string.IsNullOrWhiteSpace(item.Default))
                {
                    sysBpmsSequenceFlow currentDefualt = new SequenceFlowService(this.UnitOfWork).GetInfo(item.Default, processID);
                    if (currentDefualt == null)
                    {
                        WorkflowSequenceFlow _WorkflowSequenceFlow = _WorkflowProcess.SequenceFlows.FirstOrDefault(c => c.ID == item.Default);
                        newSequenceFlow = new sysBpmsSequenceFlow()
                        {
                            TargetElementID = _WorkflowSequenceFlow.TargetRef,
                            SourceElementID = _WorkflowSequenceFlow.SourceRef,
                            Name = _WorkflowSequenceFlow.Name,
                            ElementID = _WorkflowSequenceFlow.ID,
                            ID = Guid.NewGuid(),
                            ProcessID = processID,
                        };
                    }
                    else if (gateway != null) gateway.DefaultSequenceFlowID = currentDefualt.ID;
                }

                if (gateway != null)
                {

                    if (newSequenceFlow != null)
                        gateway.sysBpmsSequenceFlow = newSequenceFlow;
                    gateway.TypeLU = (int)sysBpmsGateway.e_TypeLU.InclusiveGateWay;
                    resultOperation = this.Update(gateway);
                    if (!resultOperation.IsSuccess)
                        return resultOperation;
                    gateway.sysBpmsElement.Name = item.Name;
                    elementService.Update(gateway.sysBpmsElement);
                }
                else
                {
                    gateway = new sysBpmsGateway()
                    {
                        ID = Guid.NewGuid(),
                        ElementID = item.ID,
                        ProcessID = processID,
                        sysBpmsSequenceFlow = newSequenceFlow,
                        TypeLU = (int)sysBpmsGateway.e_TypeLU.InclusiveGateWay,
                        sysBpmsElement = new sysBpmsElement()
                        {
                            ID = item.ID,
                            Name = item.Name,
                            ProcessID = processID,
                            TypeLU = (int)sysBpmsElement.e_TypeLU.Gateway,
                        }
                    };
                    resultOperation = this.Add(gateway);
                }
                if (!resultOperation.IsSuccess)
                    return resultOperation;
            }

            //ParallelGateway
            foreach (WorkflowParallelGateway item in _WorkflowProcess.ParallelGateways)
            {
                sysBpmsGateway gateway = listGateway.FirstOrDefault(c => c.ElementID == item.ID);
                if (gateway != null)
                {
                    gateway.TypeLU = (int)sysBpmsGateway.e_TypeLU.ParallelGateWay;
                    resultOperation = this.Update(gateway);
                    if (!resultOperation.IsSuccess)
                        return resultOperation;
                    gateway.sysBpmsElement.Name = item.Name;
                    elementService.Update(gateway.sysBpmsElement);
                }
                else
                {
                    gateway = new sysBpmsGateway()
                    {
                        ID = Guid.NewGuid(),
                        ElementID = item.ID,
                        ProcessID = processID,
                        TypeLU = (int)sysBpmsGateway.e_TypeLU.ParallelGateWay,
                        sysBpmsElement = new sysBpmsElement()
                        {
                            ID = item.ID,
                            Name = item.Name,
                            ProcessID = processID,
                            TypeLU = (int)sysBpmsElement.e_TypeLU.Gateway,
                        }
                    };
                    resultOperation = this.Add(gateway);
                }
                if (!resultOperation.IsSuccess)
                    return resultOperation;
            }

            return resultOperation;
        }

    }
}
