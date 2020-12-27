using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class HtmlElementHelper
    {
        public static HtmlElementHelperModel MakeModel(
            EngineSharedModel engineSharedModel,
            IUnitOfWork unitOfWork,
            HtmlElementHelperModel.e_FormAction formAction, sysBpmsDynamicForm mainDynamicForm)
        {

            return new HtmlElementHelperModel(
                new DataManageEngine(engineSharedModel),
                unitOfWork, new ApplicationPageEngine(engineSharedModel, unitOfWork),
                formAction,
                new DynamicCodeEngine(engineSharedModel, unitOfWork),
                new DocumentEngine(engineSharedModel, unitOfWork),
                engineSharedModel?.BaseQueryModel,
                engineSharedModel?.ApiSessionID.ToStringObj(),
                mainDynamicForm.ConfigXmlModel.IsEncrypted);
        }
    }
}
