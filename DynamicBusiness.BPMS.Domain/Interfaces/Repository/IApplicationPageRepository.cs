using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IApplicationPageRepository
    {
        void Add(sysBpmsApplicationPage applicationPage);
        void Update(sysBpmsApplicationPage applicationPage);
        void Delete(Guid applicationPageId);
        sysBpmsApplicationPage GetInfo(Guid ID);
        List<sysBpmsApplicationPage> GetList(Guid? dynamicFormID, int? groupLU);
        List<sysBpmsApplicationPage> GetAvailable(Guid? userID, bool? ShowInMenu);
    }
}
