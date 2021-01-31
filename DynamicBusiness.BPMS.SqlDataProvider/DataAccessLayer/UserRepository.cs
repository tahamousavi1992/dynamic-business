using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DynamicBusiness.BPMS.Domain;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class UserRepository : IUserRepository
    {
        private Db_BPMSEntities Context;
        public UserRepository(Db_BPMSEntities context)
        {
            this.Context = context;
        }

        public void Add(sysBpmsUser user)
        {
            user.ID = Guid.NewGuid();
            this.Context.sysBpmsUsers.Add(user);
        }

        public void Update(sysBpmsUser user)
        {
            sysBpmsUser retVal = (from p in this.Context.sysBpmsUsers
                              where p.ID == user.ID
                              select p).FirstOrDefault();
            retVal.Load(user);
        }

        public void Delete(Guid UserId)
        {
            sysBpmsUser user = this.Context.sysBpmsUsers.FirstOrDefault(d => d.ID == UserId);
            if (user != null)
            {
                this.Context.sysBpmsUsers.Remove(user);
            }
        }

        public sysBpmsUser GetInfo(Guid ID)
        {
            return (from P in this.Context.sysBpmsUsers
                    where P.ID == ID
                    select P).AsNoTracking().FirstOrDefault();

        }

        public sysBpmsUser GetInfo(string Username)
        {
            return (from P in this.Context.sysBpmsUsers
                    where P.Username == Username
                    select P).AsNoTracking().FirstOrDefault();

        }

        public sysBpmsUser GetInfoByEmail(string email)
        {
            return (from P in this.Context.sysBpmsUsers
                    where P.Email == email
                    select P).AsNoTracking().FirstOrDefault();

        }

        public List<sysBpmsUser> GetList(string name, PagingProperties currentPaging)
        {
            name = DomainUtility.toString(name);
            List<sysBpmsUser> rettVal = null;
            var query = this.Context.sysBpmsUsers.Where(d =>
              (name == string.Empty || (d.FirstName + " " + d.LastName).Contains(name)));
            if (currentPaging != null)
            {
                currentPaging.RowsCount = query.Count();
                if (Math.Ceiling(Convert.ToDecimal(currentPaging.RowsCount) / Convert.ToDecimal(currentPaging.PageSize)) < currentPaging.PageIndex)
                    currentPaging.PageIndex = 1;

                rettVal = query.OrderByDescending(p => p.FirstName).Skip((currentPaging.PageIndex - 1) * currentPaging.PageSize).Take(currentPaging.PageSize).AsNoTracking().ToList();
            }
            else rettVal = query.OrderByDescending(p => p.FirstName).ToList();

            return rettVal;
        }

        public List<sysBpmsUser> GetList(string name, int? roleCode, Guid? departmentId, PagingProperties currentPaging)
        {
            name = name.ToStringObj().Trim();
            List<sysBpmsUser> rettVal = null;
            var query = this.Context.sysBpmsUsers.Where(d =>
              (!roleCode.HasValue || d.DepartmentMembers.Count(c => c.RoleLU == roleCode) > 0) &&
              (!departmentId.HasValue || d.DepartmentMembers.Count(c => c.DepartmentID == departmentId) > 0) &&
              (name == string.Empty || d.Username.Contains(name) || (d.FirstName + " " + d.LastName).Contains(name)));
            if (currentPaging != null)
            {
                currentPaging.RowsCount = query.Count();
                if (Math.Ceiling(Convert.ToDecimal(currentPaging.RowsCount) / Convert.ToDecimal(currentPaging.PageSize)) < currentPaging.PageIndex)
                    currentPaging.PageIndex = 1;

                rettVal = query.OrderByDescending(p => p.FirstName).Skip((currentPaging.PageIndex - 1) * currentPaging.PageSize).Take(currentPaging.PageSize).AsNoTracking().ToList();
            }
            else rettVal = query.OrderByDescending(p => p.FirstName).ToList();

            return rettVal;
        }
    }
}
