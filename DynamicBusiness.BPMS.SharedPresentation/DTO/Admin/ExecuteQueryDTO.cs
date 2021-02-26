using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DynamicBusiness.BPMS.SharedPresentation
{
    public class ExecuteQueryDTO
    {
        public string Query { get; set; }
        public Guid EntityId { get; set; }
    }
}