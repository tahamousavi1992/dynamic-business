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
    [KnownType(typeof(DepartmentMemberIndexSearchDTO))]
    [DataContract]
    public class DepartmentMemberIndexSearchDTO : BaseSearchDTO<DepartmentMemberListDTO>
    {
        public DepartmentMemberIndexSearchDTO() : base() { }
        [DataMember] 
        public Guid? DepartmentID { get; set; }
    }
}