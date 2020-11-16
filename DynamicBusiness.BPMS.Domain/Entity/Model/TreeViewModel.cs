using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public class TreeViewModel
    {
        public TreeViewModel()
        {
            this.children = new List<TreeViewModel>();
        }
        public string id { get; set; }
        public string text { get; set; }
        public TreeNodeStateModel state { get; set; }
        public string icon { get; set; }//"icon" : "jstree-file"
        public List<TreeViewModel> children { get; set; }
    }
}
