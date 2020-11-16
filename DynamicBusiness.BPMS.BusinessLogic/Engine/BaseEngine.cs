using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class BaseEngine : ServiceBase
    {
        public EngineSharedModel EngineSharedModel { get; set; }
        /// <summary>
        ///this method union BaseQueryModel and additionalQueryModel
        /// </summary>
        public List<QueryModel> GetAllQueryModels(List<QueryModel> additionalQueryModel)
        {
            if (additionalQueryModel == null)
                return this.EngineSharedModel?.BaseQueryModel;
            return this.EngineSharedModel?.BaseQueryModel?.Union(additionalQueryModel).GroupBy(c => c.Key).Select(c => c.FirstOrDefault()).ToList();
        }
        public BaseEngine(EngineSharedModel engineSharedModel, IUnitOfWork unitOfWork = null) : base(unitOfWork)
        {
            this.EngineSharedModel = engineSharedModel;
        }
    }
}
