using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DynamicBusiness.BPMS.Domain
{
    public static class ConvertorExt
    {
        public static string FromBase64(this string template)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(template))
                    return template;
                return System.Text.UTF8Encoding.UTF8.GetString(System.Convert.FromBase64String(template));
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string ToBase64(this string template)
        {
            try
            {
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(template);
                return System.Convert.ToBase64String(plainTextBytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static string ToStringObjNull(this object obj)
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

        public static string ToStringObj(this object obj)
        {
            string OutString = string.Empty;
            try
            {
                if (obj != null)
                    OutString = Convert.ToString(obj);
            }
            catch { OutString = string.Empty; }
            return OutString;
        }

        /// ------------------------------------------------------------------------------------------------
        /// ------------------------------------------------------------------------------------------------
        public static bool ToBoolObj(this object obj)
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
        public static bool? ToBoolObjNull(this object obj)
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
        // ------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------
        public static int ToIntObj(this object obj)
        {
            int OutInt = 0;
            if (!string.IsNullOrEmpty(obj.ToStringObj()))
                if (!int.TryParse(DomainUtility.enNumbers(DomainUtility.toString(obj)), out OutInt))
                    OutInt = 0;
            return OutInt;
        }

        public static Guid ToGuidObj(this object obj)
        {
            Guid OutInt = Guid.Empty;
            if (!string.IsNullOrEmpty(obj.ToStringObj()))
            {
                if (!Guid.TryParse(DomainUtility.enNumbers(DomainUtility.toString(obj)), out OutInt))
                    return Guid.Empty;
            }
            else
                return Guid.Empty;
            return OutInt;
        }

        public static bool IsNullOrEmpty(this object obj)
        {
            return obj.ToGuidObj() == Guid.Empty;
        }

        public static int? ToIntObjNull(this object obj)
        {
            int OutInt = 0;
            if (!string.IsNullOrEmpty(obj.ToStringObj()))
            {
                if (!int.TryParse(DomainUtility.enNumbers(DomainUtility.toString(obj)), out OutInt))
                    return null;
            }
            else
                return null;
            return OutInt;
        }

        public static Guid? ToGuidObjNull(this object obj)
        {
            Guid OutInt = Guid.Empty;
            if (!string.IsNullOrEmpty(obj.ToStringObj()))
            {
                if (!Guid.TryParse(DomainUtility.enNumbers(DomainUtility.toString(obj)), out OutInt))
                    return null;
            }
            else
                return null;
            return OutInt;
        }

        public static string TrimStringEnd(this string text, string removeThis)
        {
            return System.Text.RegularExpressions.Regex.Replace(text.ToStringObj(), removeThis + "$", "");
        }

        public static string TrimStringStart(this string target, string trimString)
        {
            if (string.IsNullOrEmpty(trimString)) return target;
            string result = target;
            while (result.StartsWith(trimString))
            {
                result = result.Substring(trimString.Length);
            }
            return result;
        }

        public static DateTime? ToDateTimeObjNull(this object obj)
        {
            try
            {
                if (obj != null)
                    return Convert.ToDateTime(obj);
            }
            catch { }
            return null;
        }

        public static DateTime ToDateTimeObj(this object obj)
        {
            try
            {
                if (obj != null)
                    return Convert.ToDateTime(obj);
            }
            catch { }
            return DateTime.MinValue;
        }

        public static decimal ToDecimalObj(this object obj)
        {
            decimal OutDecimal;
            if (!string.IsNullOrEmpty(obj.ToStringObjNull()))
            {
                obj = obj.ToString().Replace(",", "").Replace("،", "").Replace("/", ".");
                System.Globalization.CultureInfo culInfo = new System.Globalization.CultureInfo("en-GB", true);
                if (!decimal.TryParse(obj.ToStringObjNull(), System.Globalization.NumberStyles.AllowDecimalPoint, culInfo, out OutDecimal))
                    return 0;
            }
            else
            {
                return 0;
            }

            return OutDecimal;
        }

        public static decimal? ToDecimalObjNull(this object obj)
        {
            decimal OutDecimal;
            if (!string.IsNullOrEmpty(obj.ToStringObjNull()))
            {
                obj = obj.ToString().Replace(",", "").Replace("،", "").Replace("/", ".");
                System.Globalization.CultureInfo culInfo = new System.Globalization.CultureInfo("en-GB", true);
                if (!decimal.TryParse(obj.ToStringObjNull(), System.Globalization.NumberStyles.AllowDecimalPoint, culInfo, out OutDecimal))
                    return null;
            }
            else
            {
                return null;
            }
            return OutDecimal;
        }

        public static long? ToLongObjNull(this object obj)
        {
            long OutInt = 0;
            if (!string.IsNullOrEmpty(obj.ToStringObjNull()))
            {
                if (!long.TryParse(obj.ToStringObjNull(), out OutInt))
                    return null;
            }
            else
                return null;
            return OutInt;
        }
        public static long ToLongObj(this object obj)
        {
            long OutInt = 0;
            if (!string.IsNullOrEmpty(obj.ToStringObjNull()))
            {
                if (!long.TryParse(obj.ToStringObjNull(), out OutInt))
                    return 0;
            }
            else
                return 0;
            return OutInt;
        }


        /// <summary>
        /// this is used to format a object using specific format like 'dd/MM/yyyy' ,"#0"
        /// </summary>
        public static string ToFormat(this object value, string template)
        {
            if (template.Split(new string[] { "::" }, StringSplitOptions.None).Count() == 2)
            {
                //if has format
                string format = template.Split(new string[] { "::" }, StringSplitOptions.None)[1];
                if (value == null || string.IsNullOrWhiteSpace(value.ToStringObj()))
                    return value.ToStringObj();
                if (value is DateTime)
                    return Convert.ToDateTime(value).ToString(format);
                if (value is decimal)
                    return value.ToDecimalObj().ToString(format);
                if (value is int)
                    return value.ToIntObj().ToString(format);
                if (value is double)
                    return double.Parse(value.ToStringObj()).ToString(format);
                if (value is float)
                    return float.Parse(value.ToStringObj()).ToString(format);
            }
            return value.ToStringObj();
        }

        /// <summary>
        /// this will return a list of QueryModel according to form and query string parameter and whether it is encrypted or not.
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="isEncrypted"></param>
        /// <param name="apiSessionId"></param>
        /// <returns></returns>
        public static IEnumerable<QueryModel> GetList(this HttpRequestBase httpRequest, bool isEncrypted, string apiSessionId)
        {
            if (httpRequest != null)
            {
                foreach (string key in httpRequest.QueryString.AllKeys.Where(c => c != null && !QueryModel.ForbidenList.Contains(c.ToLower())))
                    yield return new QueryModel() { Key = key.TrimStart('@'), Value = StringCipher.DecryptFormValues(httpRequest.QueryString[key], apiSessionId, isEncrypted) };
                foreach (string key in httpRequest.Form.AllKeys.Where(c => !QueryModel.ForbidenList.Contains(c.ToLower())))
                    yield return new QueryModel() { Key = key.TrimStart('@'), Value = StringCipher.DecryptFormValues(httpRequest.Form[key], apiSessionId, isEncrypted) };
                foreach (string key in httpRequest.Files.AllKeys.Where(c => !QueryModel.ForbidenList.Contains(c.ToLower())))
                    yield return new QueryModel() { Key = key.TrimStart('@'), Value = httpRequest.Files[key] };
            }
        }

        public static IEnumerable<QueryModel> GetList(this HttpRequest httpRequest, bool isEncrypted, string apiSessionId)
        {
            if (httpRequest != null)
            {
                foreach (string key in httpRequest.QueryString.AllKeys.Where(c => c != null && !QueryModel.ForbidenList.Contains(c.ToLower())))
                    yield return new QueryModel() { Key = key.TrimStart('@'), Value = StringCipher.DecryptFormValues(httpRequest.QueryString[key], apiSessionId, isEncrypted) };
                foreach (string key in httpRequest.Form.AllKeys.Where(c => c != null && !QueryModel.ForbidenList.Contains(c.ToLower())))
                    yield return new QueryModel() { Key = key.TrimStart('@'), Value = StringCipher.DecryptFormValues(httpRequest.Form[key], apiSessionId, isEncrypted) };
                foreach (string key in httpRequest.Files.AllKeys.Where(c => c != null && !QueryModel.ForbidenList.Contains(c.ToLower())))
                    yield return new QueryModel() { Key = key.TrimStart('@'), Value = httpRequest.Files[key] };
            }
        }

        public static string GetValue(this System.Xml.Linq.XElement Xelement, string name)
        {
            return Xelement.Element(name) != null ? Xelement.Element(name).Value : "";
        }
    }
}
