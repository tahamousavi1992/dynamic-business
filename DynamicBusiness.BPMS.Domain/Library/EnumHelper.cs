using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.Domain
{
    public static class EnumObjHelper
    {
        public static string GetDescription(this Enum e)
        {
            if (e == null) return "";
            try
            {
                var type = e.GetType();
                var memInfo = type.GetMember(e.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                var description = ((System.ComponentModel.DescriptionAttribute)attributes[0]).Description;

                //for resx and multilanguages
                string resxName = LangUtility.Get(e.GetType().Name + "." + description + ".Text", e.GetType().FullName.Split('+')[0].Split('.').LastOrDefault());
                return string.IsNullOrWhiteSpace(resxName) ? description : resxName;
            }
            catch
            {

            }
            return "";
        }
        /// <summary>
        /// it is suitable for enums having integer values
        /// </summary>
        public static Dictionary<int, string> GetEnumList<T>()
        {
            return Enum.GetValues(typeof(T))
                       .Cast<int>()
                       .ToDictionary(e => e, c => ((Enum)Enum.Parse(typeof(T), c.ToString())).GetDescription());
        }
    }
}
