using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class ElementService : ServiceBase
    {
        public ElementService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsElement Element)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IElementRepository>().Add(Element);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsElement element)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                element.Name = element.Name.ToStringObj().Trim();
                this.UnitOfWork.Repository<IElementRepository>().Update(element);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(string elementId, Guid proccessId)
        {
            ResultOperation resultOperation = null;
            try
            {
                resultOperation = new ResultOperation();
                if (resultOperation.IsSuccess)
                {
                    this.BeginTransaction();
                    SequenceFlowService SequenceFlowService = new SequenceFlowService(this.UnitOfWork);
                    foreach (sysBpmsSequenceFlow item in SequenceFlowService.GetList(proccessId, elementId, "", ""))
                    {
                        resultOperation = SequenceFlowService.Delete(item.ID);
                        if (!resultOperation.IsSuccess) break;
                    }
                    if (resultOperation.IsSuccess)
                    {
                        foreach (sysBpmsSequenceFlow item in SequenceFlowService.GetList(proccessId, "", elementId, ""))
                        {
                            resultOperation = SequenceFlowService.Delete(item.ID);
                            if (!resultOperation.IsSuccess) break;
                        }
                        if (resultOperation.IsSuccess)
                            this.UnitOfWork.Repository<IElementRepository>().Delete(elementId, proccessId);
                    }
                }
            }
            catch (Exception ex)
            {
                return base.ExceptionHandler(ex);
            }
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Save();
                this.CommitTransaction();
            }
            else base.RollbackTransaction();
            return resultOperation;
        }

        public sysBpmsElement GetInfo(string ID, Guid proccessId)
        {
            return this.UnitOfWork.Repository<IElementRepository>().GetInfo(ID, proccessId);
        }

        public List<sysBpmsElement> GetList(Guid ProcessID, int? TypeLU, string Name)
        {
            return this.UnitOfWork.Repository<IElementRepository>().GetList(ProcessID, TypeLU, Name);
        }

        public List<sysBpmsElement> GetList(string[] ElementID, Guid proccessId)
        {
            return this.UnitOfWork.Repository<IElementRepository>().GetList(ElementID, proccessId);
        }

    }
}
