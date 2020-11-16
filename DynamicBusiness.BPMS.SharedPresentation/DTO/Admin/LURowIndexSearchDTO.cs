using DynamicBusiness.BPMS.Domain;
using DynamicBusiness.BPMS.SharedPresentation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{    /// <summary>
     ///Controller -> LURow  Action -> Index
     /// </summary>
    [KnownType(typeof(LURowIndexSearchDTO))]
    [DataContract]
    public class LURowIndexSearchDTO : BaseSearchDTO<LURowDTO>
    {
        public void Update(IEnumerable<sysBpmsLUTable> luTables)
        {
            this.GetLUTables = luTables.Select(c => new LUTableDTO(c)).ToList();
        }
        public LURowIndexSearchDTO() : base() { }
        //search region. 
        [DataMember]
        public string Name { get; set; }
         
        [DataMember]
        public Guid? LUTableID { get; set; } 
        [DataMember]
        public IEnumerable<LUTableDTO> GetLUTables { get; set; }
    }
}