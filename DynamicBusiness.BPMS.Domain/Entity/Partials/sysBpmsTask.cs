using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    [Serializable]
    public partial class sysBpmsTask
    {
        public void Load(sysBpmsTask Task)
        {
            this.ID = Task.ID;
            this.ElementID = Task.ElementID;
            this.ProcessID = Task.ProcessID;
            this.TypeLU = Task.TypeLU;
            this.Code = Task.Code;
            this.RoleName = DomainUtility.toString(Task.RoleName);
            this.OwnerTypeLU = Task.OwnerTypeLU;
            this.UserID = Task.UserID;
            this.Rule = Task.Rule;
            this.MarkerTypeLU = Task.MarkerTypeLU;
        }

        public ResultOperation Update(string roleName, Guid? specificDepartmentId, int? ownerTypeLU, string userID, string rule)
        {
            ResultOperation resultOperation = new ResultOperation();
            this.OwnerTypeLU = ownerTypeLU;
            this.UserID = userID;
            this.RoleName = roleName;
            this.Rule = rule;
            if (this.OwnerTypeLU.HasValue)
            {
                switch ((e_OwnerTypeLU)this.OwnerTypeLU)
                {
                    case e_OwnerTypeLU.User:
                        switch ((UserTaskRuleModel.e_UserAccessType)this.UserTaskRuleModel.AccessType)
                        {
                            case UserTaskRuleModel.e_UserAccessType.Static:
                                if (string.IsNullOrWhiteSpace(this.UserID))
                                    resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsTask.UserID), nameof(sysBpmsTask)));
                                break;
                            case UserTaskRuleModel.e_UserAccessType.Variable:
                                if (string.IsNullOrWhiteSpace(this.UserTaskRuleModel.Variable))
                                    resultOperation.AddError(SharedLang.GetReuired(nameof(UserTaskRuleModel.Variable), nameof(sysBpmsTask)));
                                this.UserID = null;
                                break;
                        }
                        this.RoleName = string.Empty;
                        break;
                    case e_OwnerTypeLU.Role:
                        switch ((UserTaskRuleModel.e_RoleAccessType)this.UserTaskRuleModel.AccessType)
                        {
                            case UserTaskRuleModel.e_RoleAccessType.Static:
                                if (string.IsNullOrWhiteSpace(this.RoleName))
                                    resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsTask.RoleName), nameof(sysBpmsTask)));
                                this.SetRoleDepartment(this.RoleName, specificDepartmentId);
                                break;
                            case UserTaskRuleModel.e_RoleAccessType.Variable:
                                this.RoleName = string.Empty;
                                if (string.IsNullOrWhiteSpace(this.UserTaskRuleModel.Variable))
                                    resultOperation.AddError(SharedLang.GetReuired(nameof(UserTaskRuleModel.Variable), nameof(sysBpmsTask)));
                                break;
                            case UserTaskRuleModel.e_RoleAccessType.CorrespondentRole:
                                this.RoleName = string.Empty;
                                if (string.IsNullOrWhiteSpace(this.UserTaskRuleModel.RoleCode))
                                    resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsTask.RoleName), nameof(sysBpmsTask)));
                                if (!this.UserTaskRuleModel.UserType.HasValue)
                                    resultOperation.AddError(SharedLang.GetReuired(nameof(sysBpmsTask.UserID), nameof(sysBpmsTask)));
                                break;
                        }
                        this.UserID = null;
                        break;
                }
            }
            return resultOperation;
        }

        public void UpdateCode(string Code)
        {
            this.Code = Code;
        }

        public void SetRoleDepartment(string roleName, Guid? specificDepartmentId)
        {
            this.RoleName = "," + string.Join(",", roleName.ToStringObj().Split(',')
                              .Where(c => !string.IsNullOrWhiteSpace(c))
                              .Select(c => ((!specificDepartmentId.HasValue || c.ToIntObj() == (int)sysBpmsDepartmentMember.e_RoleLU.Requester) ? "0" : specificDepartmentId.ToStringObj()) + ":" + c)) + ",";
        }

        public enum e_TypeLU
        {
            [Description("User Task")]
            UserTask = 1,
            [Description("Service Task")]
            ServiceTask = 2,
            [Description("Script Task")]
            ScriptTask = 3,
            [Description("Task")]
            Task = 4,
        }
        public enum e_OwnerTypeLU
        {
            User = 1,
            Role = 2,
        }
        public enum e_MarkerTypeLU
        {
            [Description("Sequential")]
            Sequential = 1,
            [Description("NonSequential")]
            NonSequential = 2,
            [Description("Loop")]
            Loop = 3,
        }
        public UserTaskRuleModel UserTaskRuleModel
        {
            get
            {
                return this.Rule.ParseXML<UserTaskRuleModel>();
            }
            private set { }
        }
        /// <param name="roleLU">DepartmentMember.e_RoleLU</param>
        public bool IsInRole(Guid? departmentId, int roleLU)
        {
            return this.GetDepartmentRoles.Any(c => (!departmentId.HasValue || c.Item1 == departmentId) && c.Item2.ToIntObj() == roleLU);
        }

        public List<Tuple<Guid?, string>> GetDepartmentRoles
        {
            get
            {
                return this.RoleName.Split(',').Where(c => !string.IsNullOrWhiteSpace(c)).Select(c =>
                new Tuple<Guid?, string>(c.Split(':').FirstOrDefault().ToGuidObjNull(), c.Split(':').LastOrDefault())).ToList();
            }
        }
    }
}
