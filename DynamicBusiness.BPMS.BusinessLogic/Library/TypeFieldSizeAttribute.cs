using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public class TypeFieldSizeAttribute : Attribute
    {
        internal TypeFieldSizeAttribute(int size)
        {
            this.Size = size;
        }
        internal TypeFieldSizeAttribute(int precision, int scale)
        {
            this.Precision = precision;
            this.Scale = scale;
        }

        public int? Size { get; set; }
        public int? Precision { get; set; }
        public int? Scale { get; set; }
    }
}
