using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    /// <summary>
    ///Controller -> DbManager  Action -> Index
    /// </summary>
    [KnownType(typeof(UserIndexSearchDTO))]
    [DataContract]
    public class UserIndexSearchDTO : BaseSearchDTO<UserDTO>
    {
        public UserIndexSearchDTO() : base() { }
        public void Update(IEnumerable<sysBpmsDepartment> getDepartmentList,
          IEnumerable<sysBpmsLURow> getRoleList)
        {
            this.GetDepartmentList = getDepartmentList.Select(c => new DepartmentDTO(c));
            this.GetRoleList = getRoleList.Select(c => new LURowDTO(c));
        }

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string AdvName { get; set; }
        [DataMember]
        public int? AdvRoleCode { get; set; }
        [DataMember]
        public Guid? AdvDepartmentID { get; set; }
        [DataMember]
        public IEnumerable<DepartmentDTO> GetDepartmentList { get; set; }
        [DataMember]
        public IEnumerable<LURowDTO> GetRoleList { get; set; }
    }
}