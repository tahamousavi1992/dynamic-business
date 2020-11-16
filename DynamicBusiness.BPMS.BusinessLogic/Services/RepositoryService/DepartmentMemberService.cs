using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class DepartmentMemberService : ServiceBase
    {
        public DepartmentMemberService(IUnitOfWork unitOfWork = null) : base(unitOfWork) { }

        public ResultOperation Add(sysBpmsDepartmentMember departmentMember)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (this.GetList(departmentMember.DepartmentID, departmentMember.RoleLU, departmentMember.UserID).Any(c => c.ID != departmentMember.ID))
                resultOperation.AddError(LangUtility.Get("SameUser.Error", nameof(sysBpmsDepartmentMember)));

            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IDepartmentMemberRepository>().Add(departmentMember);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Update(sysBpmsDepartmentMember departmentMember)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (this.GetList(departmentMember.DepartmentID, departmentMember.RoleLU, departmentMember.UserID).Any(c => c.ID != departmentMember.ID))
                resultOperation.AddError(LangUtility.Get("SameUser.Error", nameof(sysBpmsDepartmentMember)));

            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IDepartmentMemberRepository>().Update(departmentMember);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public ResultOperation Delete(Guid DepartmentMemberId)
        {
            ResultOperation resultOperation = new ResultOperation();
            if (resultOperation.IsSuccess)
            {
                this.UnitOfWork.Repository<IDepartmentMemberRepository>().Delete(DepartmentMemberId);
                this.UnitOfWork.Save();
            }
            return resultOperation;
        }

        public sysBpmsDepartmentMember GetInfo(Guid ID)
        {
            return this.UnitOfWork.Repository<IDepartmentMemberRepository>().GetInfo(ID);
        }

        public List<sysBpmsDepartmentMember> GetList(Guid? departmentID, int? roleLU, Guid? userID)
        {
            return this.UnitOfWork.Repository<IDepartmentMemberRepository>().GetList(departmentID, roleLU, userID);
        }

        public List<sysBpmsDepartmentMember> GetList(Guid? departmentID, int? roleLU, Guid? userID, PagingProperties currentPaging)
        {
            return this.UnitOfWork.Repository<IDepartmentMemberRepository>().GetList(departmentID, roleLU, userID, currentPaging);
        }

        public List<sysBpmsDepartmentMember> GetList(Guid[] listDepartmentID, int? roleLU)
        {
            return this.UnitOfWork.Repository<IDepartmentMemberRepository>().GetList(listDepartmentID, roleLU);
        }

        public List<sysBpmsDepartmentMember> GetListHierarchy(Guid[] listDepartmentID, int roleLU, bool goUpDepartment = true)
        {
            return this.UnitOfWork.Repository<IDepartmentMemberRepository>().GetListHierarchy(listDepartmentID, roleLU, goUpDepartment);
        }

    }
}
