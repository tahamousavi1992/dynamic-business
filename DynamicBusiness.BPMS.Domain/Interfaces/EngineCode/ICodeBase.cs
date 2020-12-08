using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface ICodeBase
    {
        Guid? GetProcessID { get; }
        Guid? GetApplicationPageID { get; }
        IUnitOfWork GetUnitOfWork { get; }
        IVariableCodeHelper VariableHelper { get; }
        IAccessCodeHelper AccessHelper { get; }
        IControlCodeHelper ControlHelper { get; }
        IProcessCodeHelper ProcessHelper { get; }
        IMessageCodeHelper MessageHelper { get; }
        IUserCodeHelper UserHelper { get; }
        IUrlCodeHelper UrlHelper { get; }
        IDocumentCodeHelper DocumentHelper { get; }
        IQueryCodeHelper QueryHelper { get; }
        IEntityCodeHelper EntityHelper { get; }
        IWebServiceCodeHelper WebServiceHelper { get; }

        Guid? GetCurrentUserID { get; }
        string GetCurrentUserName { get; }
        Guid? GetThreadUserID { get; }
        Guid? GetThreadID { get; }
    }
}
