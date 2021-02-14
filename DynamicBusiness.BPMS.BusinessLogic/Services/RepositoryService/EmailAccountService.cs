using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class EmailAccountService : ServiceBase
    {
        public EmailAccountService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation AddOverwrite(sysBpmsEmailAccount emailAccount)
        {
            sysBpmsEmailAccount current = emailAccount.ObjectID == null ? null :
                this.GetList(emailAccount.ObjectTypeLU, emailAccount.ObjectID, null).LastOrDefault();
            if (!string.IsNullOrWhiteSpace(emailAccount.Email + emailAccount.MailPassword + emailAccount.SMTP))
            {
                if (current == null)
                    return this.Add(emailAccount);
                else
                {
                    emailAccount.ID = current.ID;
                    return this.Update(emailAccount);
                }
            }
            else
            {
                if (emailAccount != null && current != null)
                    return this.Delete(current.ID);
                else
                    return new ResultOperation();
            }
        }

        public ResultOperation Add(sysBpmsEmailAccount emailAccount)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IEmailAccountRepository>().Add(emailAccount);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsEmailAccount emailAccount)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IEmailAccountRepository>().Update(emailAccount);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(Guid emailAccountId)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IEmailAccountRepository>().Delete(emailAccountId);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public sysBpmsEmailAccount GetInfo(Guid ID)
        {
            if (ID == Guid.Empty) return new sysBpmsEmailAccount();
            return this.UnitOfWork.Repository<IEmailAccountRepository>().GetInfo(ID);
        }

        public List<sysBpmsEmailAccount> GetList(int? ObjectTypeLU, Guid? ObjectID, PagingProperties currentPaging)
        {
            return this.UnitOfWork.Repository<IEmailAccountRepository>().GetList(ObjectTypeLU, ObjectID, currentPaging);
        }
    }
}
