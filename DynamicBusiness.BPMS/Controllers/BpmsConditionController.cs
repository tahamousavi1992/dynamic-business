using DynamicBusiness.BPMS.BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;

namespace DynamicBusiness.BPMS.Controllers
{
    public class BpmsConditionController : BpmsAdminApiControlBase
    {
        public object GetConditions(string ElementId, Guid ProcessId)
        { 
            using (GatewayService gatewayService = new GatewayService())
            {
                sysBpmsGateway sysBpmsGateway = gatewayService.GetInfo(ElementId, ProcessId);

                using (SequenceFlowService sequenceFlowService = new SequenceFlowService())
                {
                    using (ElementService elementService = new ElementService())
                    {
                        using (ConditionService conditionService = new ConditionService())
                        {
                            List<sysBpmsSequenceFlow> SequenceFlows = sequenceFlowService.GetList(ProcessId, "", ElementId, "");
                            List<sysBpmsElement> Elements = elementService.GetList(SequenceFlows.Select(c => c.TargetElementID).ToArray(), ProcessId);

                            using (ProcessService processService = new ProcessService())
                                return new
                                {
                                    SequenceFlows = SequenceFlows.Select(c => new
                                    {
                                        c.ID,
                                        Elements.FirstOrDefault(d => d.ID == c.TargetElementID).Name,
                                    }).ToList(),
                                    AllowEdit = processService.GetInfo(ProcessId).AllowEdit(),
                                    GatewayID = sysBpmsGateway.ID,
                                    GetList = conditionService.GetList(sysBpmsGateway.ID, null, null).Select(c => new ConditionDTO(c)).ToList(),
                                };
                        }
                    }
                }
            }

        }

        [HttpPost]
        public object PostAddEdit(PostAddEditGatewayDTO model)
        {
            using (ProcessService processService = new ProcessService())
            {
                if (!processService.GetInfo(model.ProcessId).AllowEdit())
                    return new PostMethodMessage(LangUtility.Get("NotAllowEdit.Text", nameof(sysBpmsProcess)), DisplayMessageType.error);
            }
            using (ConditionService conditionService = new ConditionService())
            {
                model.ListConditions = model.ListConditions ?? new List<ConditionDTO>();
                ResultOperation resultOperation = null;
                List<sysBpmsCondition> CurrentConditions = conditionService.GetList(model.GatewayID, null, null);
                foreach (sysBpmsCondition item in CurrentConditions.Where(c => !model.ListConditions.Any(d => d.ID == c.ID)))
                {
                    resultOperation = conditionService.Delete(item.ID);
                    if (!resultOperation.IsSuccess)
                    {
                        return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
                    }
                }
                foreach (ConditionDTO item in model.ListConditions)
                {
                    sysBpmsCondition condition = item.ID != Guid.Empty ? conditionService.GetInfo(item.ID) : new sysBpmsCondition();
                    resultOperation = condition.Update(model.GatewayID, item.SequenceFlowID, item.Code);
                    if (!resultOperation.IsSuccess)
                        break;
                    if (item.ID != Guid.Empty)
                        resultOperation = conditionService.Update(condition);
                    else resultOperation = conditionService.Add(condition);
                    if (!resultOperation.IsSuccess)
                        break;
                }
                if (resultOperation.IsSuccess)
                    return new PostMethodMessage(SharedLang.Get("Success.Text"), DisplayMessageType.success);
                else
                    return new PostMethodMessage(resultOperation.GetErrors(), DisplayMessageType.error);
            }
        }
    }
}