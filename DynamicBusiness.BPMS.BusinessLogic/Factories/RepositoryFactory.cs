using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;
using Unity.Resolution;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public static class RepositoryFactory<TAnyType>
    {
        private static IUnityContainer container = null;
        public static TAnyType Create(Db_BPMSEntities context)
        {
            if (container == null)
            {
                container = new UnityContainer();
                container.RegisterType<IConditionRepository, ConditionRepository>();
                container.RegisterType<IDBConnectionRepository, DBConnectionRepository>();
                container.RegisterType<IDataBaseQueryRepository, DataBaseQueryRepository>();
                container.RegisterType<IDepartmentMemberRepository, DepartmentMemberRepository>();
                container.RegisterType<IDepartmentRepository, DepartmentRepository>();
                container.RegisterType<IDynamicFormRepository, DynamicFormRepository>();
                container.RegisterType<IElementRepository, ElementRepository>();
                container.RegisterType<IEntityDefRepository, EntityDefRepository>();
                container.RegisterType<IEventRepository, EventRepository>();
                container.RegisterType<IGatewayRepository, GatewayRepository>();
                container.RegisterType<ILURowRepository, LURowRepository>();
                container.RegisterType<ILUTableRepository, LUTableRepository>();
                container.RegisterType<IProcessRepository, ProcessRepository>();
                container.RegisterType<ISequenceFlowRepository, SequenceFlowRepository>();
                container.RegisterType<IStepRepository, StepRepository>();
                container.RegisterType<ITaskRepository, TaskRepository>();
                container.RegisterType<IThreadRepository, ThreadRepository>();
                container.RegisterType<IThreadTaskRepository, ThreadTaskRepository>();
                container.RegisterType<IThreadVariableRepository, ThreadVariableRepository>();
                container.RegisterType<IVariableRepository, VariableRepository>();
                container.RegisterType<ILaneRepository, LaneRepository>();
                container.RegisterType<IUserRepository, UserRepository>();
                container.RegisterType<IDocumentFolderRepository, DocumentFolderRepository>();
                container.RegisterType<IDocumentDefRepository, DocumentDefRepository>();
                container.RegisterType<IDocumentRepository, DocumentRepository>();
                container.RegisterType<IVariableDependencyRepository, VariableDependencyRepository>();
                container.RegisterType<IOperationRepository, OperationRepository>();
                container.RegisterType<IEmailAccountRepository, EmailAccountRepository>();
                container.RegisterType<IThreadEventRepository, ThreadEventRepository>();
                container.RegisterType<IApplicationPageRepository, ApplicationPageRepository>();
                container.RegisterType<IApplicationPageAccessRepository, ApplicationPageAccessRepository>();
                container.RegisterType<ISettingDefRepository, SettingDefRepository>();
                container.RegisterType<ISettingValueRepository, SettingValueRepository>();
                container.RegisterType<IAPIAccessRepository, APIAccessRepository>();
                container.RegisterType<IMessageTypeRepository, MessageTypeRepository>();
                container.RegisterType<IProcessGroupRepository, ProcessGroupRepository>();
            }
            return container.Resolve<TAnyType>(new ResolverOverride[] { new ParameterOverride("context", context) });
        }
    }
}
