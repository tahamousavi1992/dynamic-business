using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public class ComboTreeModel
    {
        public ComboTreeModel()
        {
            subs = new List<ComboTreeModel>();
        }
        public string id { get; set; }
        public string title { get; set; }
        public string state { get; set; }
        public List<ComboTreeModel> subs { get; set; }
    }
}
