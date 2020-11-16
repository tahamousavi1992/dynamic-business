using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IApplicationPageEngine
    {
        List<sysBpmsApplicationPage> GetAvailable(Guid? userID, string UserName, bool? ShowInMenu);
        bool CheckUserAccessByForm(Guid dynamicFormID, ElementBase.e_AccessType e_AccessType);
        bool CheckUserAccessByApplicationID(Guid applicationID, ElementBase.e_AccessType e_AccessType);
    }
}
