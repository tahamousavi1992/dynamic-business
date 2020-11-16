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
    [KnownType(typeof(DbManagerIndexSearchDTO))]
    [DataContract]
    public class DbManagerIndexSearchDTO : BaseSearchDTO<DBConnectionDTO>
    {
        public DbManagerIndexSearchDTO() : base() { }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string AdvName { get; set; }
        [DataMember]
        public string AdvDataSource { get; set; }
    }
}