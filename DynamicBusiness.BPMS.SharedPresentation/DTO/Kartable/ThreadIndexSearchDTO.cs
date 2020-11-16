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
    [KnownType(typeof(ThreadIndexSearchDTO))]
    [DataContract]
    public class ThreadIndexSearchDTO : BaseSearchDTO<ThreadDTO>
    {
        public ThreadIndexSearchDTO() : base() { }
        public void Update(IEnumerable<sysBpmsProcess> process, List<ThreadDTO> list)
        {
            this.GetProcess = process?.Select(c => new QueryModel(c.ID.ToString(), c.Name)).ToList();
            this.GetList = list;
        }

        //search region.
        [DataMember]
        public Guid? ProcessID { get; set; }

        [DataMember]
        public Guid? AdvProcessID { get; set; }
        [DataMember]
        public IEnumerable<QueryModel> GetProcess { get; set; }

        [DataMember]
        public DateTime? AdvStartDateFrom { get; set; }

        [DataMember]
        public DateTime? AdvStartDateTo { get; set; }

        [DataMember]
        public DateTime? AdvEndDateFrom { get; set; }

        [DataMember]
        public DateTime? AdvEndDateTo { get; set; }

        //table region.
        [DataMember]
        public override List<ThreadDTO> GetList { get; set; }
    }
}