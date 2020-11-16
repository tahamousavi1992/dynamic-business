using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicBusiness.BPMS.CodePanel
{
    public static class CodeUtility
    {
        public static string ToString(object obj)
        {
            string OutString = null;
            try
            {
                if (obj != null)
                    OutString = Convert.ToString(obj);
            }
            catch { }
            return OutString;
        }
        public static string ToStringObj(object obj)
        {
            string OutString = string.Empty;
            try
            {
                if (obj != null)
                    OutString = Convert.ToString(obj);
            }
            catch { }
            return OutString;
        }
        public static DateTime? ToDateTime(object obj)
        {
            try
            {
                if (obj != null)
                    return Convert.ToDateTime(obj);
            }
            catch { }
            return null;
        }
        public static DateTime ToDateTimeObj(object obj)
        {
            try
            {
                if (obj != null)
                    return Convert.ToDateTime(obj);
            }
            catch { }
            return DateTime.MinValue;
        }

        public static bool? ToBoolean(object obj)
        {
            bool? bResult = null;
            try
            {
                if (obj != null)
                    bResult = Convert.ToBoolean(obj);
            }
            catch { bResult = null; }
            return bResult;
        }
        public static bool ToBooleanObj(object obj)
        {
            bool? bResult = null;
            try
            {
                if (obj != null)
                    bResult = Convert.ToBoolean(obj);
            }
            catch { bResult = null; }
            return bResult ?? false;
        }

        public static int? ToInt(object obj)
        {
            int OutInt = 0;
            if (!string.IsNullOrEmpty(ToString(obj)))
            {
                if (!int.TryParse(CodeUtility.ToString(obj), out OutInt))
                    return null;
            }
            else
                return null;
            return OutInt;
        }
        public static int ToIntObj(object obj)
        {
            int OutInt = 0;
            if (!string.IsNullOrEmpty(ToString(obj)))
            {
                if (!int.TryParse(CodeUtility.ToString(obj), out OutInt))
                    return 0;
            }
            else
                return 0;
            return OutInt;
        }

        public static Guid? ToGuid(object obj)
        {
            Guid OutInt = Guid.Empty;
            if (!string.IsNullOrEmpty(ToString(obj)))
            {
                if (obj is Guid)
                    return (Guid)obj;
                if (!Guid.TryParse(CodeUtility.ToString(obj), out OutInt))
                    return null;
            }
            else
                return null;
            return OutInt;
        }
        public static Guid ToGuidObj(object obj)
        {
            Guid OutInt = Guid.Empty;
            if (!string.IsNullOrEmpty(ToString(obj)))
            {
                if (obj is Guid)
                    return (Guid)obj;
                if (!Guid.TryParse(CodeUtility.ToString(obj), out OutInt))
                    return Guid.Empty;
            }
            else
                return Guid.Empty;
            return OutInt;
        }

        public static decimal? ToDecimal(object obj)
        {
            decimal OutDecimal = 0;

            if (!string.IsNullOrEmpty(CodeUtility.ToString(obj)))
            {
                obj = obj.ToString().Replace(",", "").Replace("،", "").Replace("/", ".");
                System.Globalization.CultureInfo culInfo = new System.Globalization.CultureInfo("en-GB", true);
                if (!decimal.TryParse(CodeUtility.ToString(obj), System.Globalization.NumberStyles.AllowDecimalPoint, culInfo, out OutDecimal))
                    return 0;
            }
            else
            {
                return 0;
            }

            return OutDecimal;
        }
        public static decimal ToDecimalObj(object obj)
        {
            decimal OutDecimal = 0;

            if (!string.IsNullOrEmpty(CodeUtility.ToString(obj)))
            {
                obj = obj.ToString().Replace(",", "").Replace("،", "").Replace("/", ".");
                System.Globalization.CultureInfo culInfo = new System.Globalization.CultureInfo("en-GB", true);
                if (!decimal.TryParse(CodeUtility.ToString(obj), System.Globalization.NumberStyles.AllowDecimalPoint, culInfo, out OutDecimal))
                    return 0;
            }
            else
            {
                return 0;
            }

            return OutDecimal;
        }

        public static long? ToLong(object obj)
        {
            long OutInt = 0;
            if (!string.IsNullOrEmpty(ToString(obj)))
            {
                if (!long.TryParse(CodeUtility.ToString(obj), out OutInt))
                    return null;
            }
            else
                return null;
            return OutInt;
        }
        public static long ToLongObj(object obj)
        {
            long OutInt = 0;
            if (!string.IsNullOrEmpty(ToString(obj)))
            {
                if (!long.TryParse(CodeUtility.ToString(obj), out OutInt))
                    return 0;
            }
            else
                return 0;
            return OutInt;
        }
    }
}
