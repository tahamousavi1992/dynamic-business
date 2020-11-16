using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public class TreeNodeStateModel
    {
        public bool? selected { get; set; }
        public bool? disabled { get; set; }
        public bool? opened { get; set; }
    }
}
