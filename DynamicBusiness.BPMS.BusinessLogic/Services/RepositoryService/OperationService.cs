using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class OperationService : ServiceBase
    {
        public OperationService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsOperation Operation)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IOperationRepository>().Add(Operation);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsOperation Operation)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IOperationRepository>().Update(Operation);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(Guid OperationId)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IOperationRepository>().Delete(OperationId);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public sysBpmsOperation GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<IOperationRepository>().GetInfo(ID);
        }

        public List<sysBpmsOperation> GetList(int? GroupLU, PagingProperties currentPaging)
        {
            return this.UnitOfWork.Repository<IOperationRepository>().GetList(GroupLU, currentPaging);
        }

        public List<string> GetParameters(Guid operationId)
        {
            sysBpmsOperation operation = this.GetInfo(operationId);
            List<string> parameters = new List<string>();
            if (!string.IsNullOrWhiteSpace(operation.SqlCommand))
            {
                List<string> queryParams = Regex.Matches(operation.SqlCommand, @"\@\w+").Cast<Match>().Select(m => m.Value).ToList();
                foreach (string parameter in queryParams)
                {
                    if (!parameters.Contains(parameter.TrimStart('@')) && Regex.Matches(operation.SqlCommand, "declare(\\s*?)" + parameter).Count == 0)
                        parameters.Add(parameter.TrimStart('@'));
                }
            }
            return parameters;
        }
 
    }
}
