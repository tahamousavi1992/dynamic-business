using DynamicBusiness.BPMS.BusinessLogic;
using DynamicBusiness.BPMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    [DataContract]
    public class BaseSearchDTO<T>
    {
        public BaseSearchDTO()
        {
            this.GetPagingProperties = new PagingProperties();
        }
        public void Update(List<T> list)
        {
            this.GetList = list;
        }
        [DataMember]
        public PagingProperties GetPagingProperties { get; set; }
        [DataMember]
        public bool IsAdvSearch { get; set; }
        [DataMember]
        public virtual List<T> GetList { get; set; }
    }
}