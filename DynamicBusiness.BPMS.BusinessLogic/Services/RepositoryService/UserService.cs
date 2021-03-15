using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class UserService : ServiceBase
    {
        public UserService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsUser user, sysBpmsEmailAccount emailAccount)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                this.BeginTransaction();
                if (resultOperation.IsSuccess)
                {

                    this.UnitOfWork.Repository<IUserRepository>().Add(user);
                    this.UnitOfWork.Save();
                    if (emailAccount != null)
                    {
                        emailAccount.ObjectID = user.ID;
                        resultOperation = new EmailAccountService(base.UnitOfWork).AddOverwrite(emailAccount);
                    }
                }
            }
            catch (Exception ex)
            {
                return base.ExceptionHandler(ex);
            }
            base.FinalizeService(resultOperation);
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsUser user, sysBpmsEmailAccount emailAccount)
        {
            ResultOperation resultOperation = new ResultOperation();
            try
            {
                this.BeginTransaction();
                if (resultOperation.IsSuccess)
                {
                    this.UnitOfWork.Repository<IUserRepository>().Update(user);
                    this.UnitOfWork.Save();
                    if (emailAccount != null)
                    {
                        emailAccount.ObjectID = user.ID;
                        resultOperation = new EmailAccountService(base.UnitOfWork).AddOverwrite(emailAccount);
                    }
                }
            }
            catch (Exception ex)
            {
                return base.ExceptionHandler(ex);
            }
            base.FinalizeService(resultOperation);
            return resultOperation;
        }

        public ResultOperation Delete(Guid userId)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                try
                {
                    this.BeginTransaction();
                    //delete email accounts
                    EmailAccountService emailAccountService = new EmailAccountService(base.UnitOfWork);
                    var emailList = emailAccountService.GetList((int)sysBpmsEmailAccount.e_ObjectTypeLU.User, userId, null);
                    foreach (var item in emailList)
                    {
                        emailAccountService.Delete(item.ID);
                    }

                    //delete department member.
                    DepartmentMemberService departmentMemberService = new DepartmentMemberService(base.UnitOfWork);
                    var members = departmentMemberService.GetList(null, null, userId);
                    foreach (var item in members)
                    {
                        departmentMemberService.Delete(item.ID);
                    }

                    this.UnitOfWork.Repository<IUserRepository>().Delete(userId);
                    this.UnitOfWork.Save();
                }
                catch (Exception ex)
                {
                    resultOperation.AddError(LangUtility.Get("UserUsedError.Text", nameof(sysBpmsUser)));
                    return base.ExceptionHandler(new Exception(LangUtility.Get("UserUsedError.Text", nameof(sysBpmsUser))));
                }
            }
            base.FinalizeService(resultOperation);
            return resultOperation;
        }

        public sysBpmsUser GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<IUserRepository>().GetInfo(ID);
        }

        public sysBpmsUser GetInfoByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;
            return this.UnitOfWork.Repository<IUserRepository>().GetInfo(email);
        }

        public sysBpmsUser GetInfo(string Username)
        {
            if (string.IsNullOrWhiteSpace(Username))
                return null;
            return this.UnitOfWork.Repository<IUserRepository>().GetInfo(Username);
        }

        public List<sysBpmsUser> GetList(string name, PagingProperties currentPaging)
        {
            return this.UnitOfWork.Repository<IUserRepository>().GetList(name, currentPaging);
        }
        public List<sysBpmsUser> GetList(string name, int? roleCode, Guid? departmentId, PagingProperties currentPaging)
        {
            return this.UnitOfWork.Repository<IUserRepository>().GetList(name, roleCode, departmentId, currentPaging);
        }
    }
}
