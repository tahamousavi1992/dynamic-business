using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class SequenceFlowService : ServiceBase
    {
        public SequenceFlowService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsSequenceFlow SequenceFlow)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<ISequenceFlowRepository>().Add(SequenceFlow);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsSequenceFlow SequenceFlow)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<ISequenceFlowRepository>().Update(SequenceFlow);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(Guid SequenceFlowId)
        {
            ResultOperation resultOperation = null;
            try
            {
                resultOperation = new ResultOperation();
                if (resultOperation.IsSuccess)
                {
                    this.BeginTransaction();
                    sysBpmsSequenceFlow sysBpmsSequenceFlow = this.GetInfo(SequenceFlowId);
                    foreach (sysBpmsCondition Condition in new ConditionService(this.UnitOfWork).GetList(null, SequenceFlowId, null))
                    {
                        resultOperation = new ConditionService(this.UnitOfWork).Delete(Condition.ID);
                        if (!resultOperation.IsSuccess)
                            break;
                    }
                    //remove DefaultSequence
                    foreach (sysBpmsGateway gateway in new GatewayService(base.UnitOfWork).GetListByDefaultSequence(SequenceFlowId))
                    {
                        gateway.DefaultSequenceFlowID = null;
                        resultOperation = new GatewayService(base.UnitOfWork).Update(gateway);
                        if (!resultOperation.IsSuccess)
                            break;
                    }

                    if (resultOperation.IsSuccess)
                    {
                        this.UnitOfWork.Repository<ISequenceFlowRepository>().Delete(SequenceFlowId);
                        this.UnitOfWork.Save();
                        new ElementService(this.UnitOfWork).Delete(sysBpmsSequenceFlow.ElementID, sysBpmsSequenceFlow.ProcessID);
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

        public sysBpmsSequenceFlow GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<ISequenceFlowRepository>().GetInfo(ID);
        }
        public sysBpmsSequenceFlow GetInfo(string elementId, Guid processId)
        {
            return this.UnitOfWork.Repository<ISequenceFlowRepository>().GetInfo(elementId, processId);
        }
        public List<sysBpmsSequenceFlow> GetList(Guid ProcessID, string TargetElementID, string SourceElementID, string Name)
        {
            return this.UnitOfWork.Repository<ISequenceFlowRepository>().GetList(ProcessID, TargetElementID, SourceElementID, Name);
        }

        public List<sysBpmsSequenceFlow> GetList(Guid ProcessID)
        {
            return this.UnitOfWork.Repository<ISequenceFlowRepository>().GetList(ProcessID);
        }

        public ResultOperation Update(Guid processID, WorkflowProcess _WorkflowProcess)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                this.BeginTransaction();
                List<sysBpmsSequenceFlow> SequenceFlows = this.GetList(processID);
                foreach (sysBpmsSequenceFlow item in SequenceFlows.Where(c => !_WorkflowProcess.SequenceFlows.Any(d => d.ID == c.ElementID)))
                {
                    resultOperation = this.Delete(item.ID);
                    if (!resultOperation.IsSuccess)
                        return resultOperation;
                }

                //StartEvents
                foreach (WorkflowSequenceFlow item in _WorkflowProcess.SequenceFlows)
                {
                    sysBpmsSequenceFlow _SequenceFlow = SequenceFlows.FirstOrDefault(c => c.ElementID == item.ID);
                    if (_SequenceFlow != null)
                    {
                        _SequenceFlow.Name = item.Name;
                        _SequenceFlow.SourceElementID = item.SourceRef;
                        _SequenceFlow.TargetElementID = item.TargetRef;
                        resultOperation = this.Update(_SequenceFlow);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                        //Element
                        _SequenceFlow.sysBpmsElement.Name = item.Name;
                        resultOperation = new ElementService(this.UnitOfWork).Update(_SequenceFlow.sysBpmsElement);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                    }
                    else
                    {
                        _SequenceFlow = new sysBpmsSequenceFlow()
                        {
                            ID = Guid.NewGuid(),
                            ElementID = item.ID,
                            ProcessID = processID,
                            Name = item.Name,
                            SourceElementID = item.SourceRef,
                            TargetElementID = item.TargetRef,
                            //Element
                            sysBpmsElement = new sysBpmsElement()
                            {
                                ID = item.ID,
                                Name = item.Name,
                                ProcessID = processID,
                                TypeLU = (int)sysBpmsElement.e_TypeLU.Sequence,
                            }
                        };
                        resultOperation = this.Add(_SequenceFlow);
                        if (!resultOperation.IsSuccess)
                            return resultOperation;
                    }
                };
            }
            catch (Exception ex)
            {
                return base.ExceptionHandler(ex);
            }
            if (resultOperation.IsSuccess)
                this.UnitOfWork.Save();
            base.FinalizeService(resultOperation);

            return resultOperation;
        }

    }
}
