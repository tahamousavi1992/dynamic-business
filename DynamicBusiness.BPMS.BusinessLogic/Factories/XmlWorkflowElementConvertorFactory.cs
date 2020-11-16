using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public static class XmlWorkflowElementConvertorFactory<TAnyType> where TAnyType : IWorkflowElement
    {

        private static IUnityContainer container = null;

        public static IWorkflowElementConvertor<TAnyType> Create()
        {
            if (container == null)
            {
                container = new UnityContainer();
                container.RegisterType<IWorkflowElementConvertor<WorkflowProcess>, XmlWorkflowProcessConvertor>(typeof(WorkflowProcess).Name, new InjectionConstructor());
                container.RegisterType<IWorkflowElementConvertor<WorkflowUserTask>, XmlWorkflowUserTaskConvertor>(typeof(WorkflowUserTask).Name, new InjectionConstructor());
                container.RegisterType<IWorkflowElementConvertor<WorkflowSequenceFlow>, XmlWorkflowSequenceFlowConvertor>(typeof(WorkflowSequenceFlow).Name, new InjectionConstructor());
                container.RegisterType<IWorkflowElementConvertor<WorkflowStartEvent>, XmlWorkflowStartEventConvertor>(typeof(WorkflowStartEvent).Name, new InjectionConstructor());
                container.RegisterType<IWorkflowElementConvertor<WorkflowIntermediateThrowEvent>, XmlWorkflowIntermediateThrowEventConvertor>(typeof(WorkflowIntermediateThrowEvent).Name, new InjectionConstructor());
                container.RegisterType<IWorkflowElementConvertor<WorkflowIntermediateCatchEvent>, XmlWorkflowIntermediateCatchEventConvertor>(typeof(WorkflowIntermediateCatchEvent).Name, new InjectionConstructor());
                container.RegisterType<IWorkflowElementConvertor<WorkflowBoundaryEvent>, XmlWorkflowBoundaryEventConvertor>(typeof(WorkflowBoundaryEvent).Name, new InjectionConstructor());
                container.RegisterType<IWorkflowElementConvertor<WorkflowEndEvent>, XmlWorkflowEndEventConvertor>(typeof(WorkflowEndEvent).Name, new InjectionConstructor());
                container.RegisterType<IWorkflowElementConvertor<WorkflowExclusiveGateway>, XmlWorkflowExclusiveGatewayConvertor>(typeof(WorkflowExclusiveGateway).Name, new InjectionConstructor());
                container.RegisterType<IWorkflowElementConvertor<WorkflowChildLaneSet>, XmlWorkflowChildLaneSetConvertor>(typeof(WorkflowChildLaneSet).Name, new InjectionConstructor());
                container.RegisterType<IWorkflowElementConvertor<WorkflowLaneSet>, XmlWorkflowLaneSetConvertor>(typeof(WorkflowLaneSet).Name, new InjectionConstructor());
                container.RegisterType<IWorkflowElementConvertor<WorkflowLane>, XmlWorkflowLaneConvertor>(typeof(WorkflowLane).Name, new InjectionConstructor());
                container.RegisterType<IWorkflowElementConvertor<WorkflowServiceTask>, XmlWorkflowServiceTaskConvertor>(typeof(WorkflowServiceTask).Name, new InjectionConstructor());
                container.RegisterType<IWorkflowElementConvertor<WorkflowInclusiveGateway>, XmlWorkflowInclusiveGatewayConvertor>(typeof(WorkflowInclusiveGateway).Name, new InjectionConstructor());
                container.RegisterType<IWorkflowElementConvertor<WorkflowParallelGateway>, XmlWorkflowParallelGatewayConvertor>(typeof(WorkflowParallelGateway).Name, new InjectionConstructor());
                container.RegisterType<IWorkflowElementConvertor<WorkflowScriptTask>, XmlWorkflowScriptTaskConvertor>(typeof(WorkflowScriptTask).Name, new InjectionConstructor());
                container.RegisterType<IWorkflowElementConvertor<WorkflowTask>, XmlWorkflowTaskConvertor>(typeof(WorkflowTask).Name, new InjectionConstructor());
            }
            return container.Resolve<IWorkflowElementConvertor<TAnyType>>(typeof(TAnyType).Name);
        }
    }
}
