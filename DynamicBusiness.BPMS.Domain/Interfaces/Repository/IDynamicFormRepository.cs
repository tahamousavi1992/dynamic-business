using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public interface IDynamicFormRepository
    {
        void Add(sysBpmsDynamicForm dynamicForm);
        void Update(sysBpmsDynamicForm dynamicForm);
        void Delete(Guid dynamicFormId);
        sysBpmsDynamicForm GetInfo(Guid ID);
        sysBpmsDynamicForm GetInfoByPageID(Guid ApplicationPageID);
        sysBpmsDynamicForm GetInfoByStepID(Guid StepID);
        List<sysBpmsDynamicForm> GetList(Guid? processId, Guid? applicationPageID, bool? GetPages, string Name, bool? showInOverview, PagingProperties currentPaging);
    }
}
