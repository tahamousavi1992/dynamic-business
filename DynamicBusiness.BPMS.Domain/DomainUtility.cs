using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DynamicBusiness.BPMS.Domain
{
    public static class DomainUtility
    {
        public enum ReportExportType
        {
            PDF,
            Excel,
        }
        public static string GetXAttributeValue(XElement element, string attrName)
        {
            var attrElement = element.Attribute(attrName);
            if (attrElement != null)
                return attrElement.Value;
            else
                return string.Empty;
            //throw new Exception("There isn't any " + attrTagName + " for element");
        }

        public static T To<T>(this IConvertible obj)
        {
            return (T)Convert.ChangeType(obj, typeof(T));
        }

        public static string toString(object obj)
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
        public static bool toBool(object obj)
        {
            try
            {
                if (obj != null)
                    return obj.ToBoolObj();
            }
            catch { }
            return false;
        }
        // ------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------
        public static int toInt(object obj)
        {
            int OutInt = 0;
            if (!string.IsNullOrEmpty(DomainUtility.toString(obj)))
                if (!int.TryParse(DomainUtility.enNumbers(DomainUtility.toString(obj)), out OutInt))
                    OutInt = 0;
            return OutInt;
        }

        public static string enNumbers(string faNumbers)
        {
            return faNumbers
                .Replace("۰", "0")
                .Replace("۱", "1")
                .Replace("۲", "2")
                .Replace("۳", "3")
                .Replace("۴", "4")
                .Replace("۵", "5")
                .Replace("۶", "6")
                .Replace("۷", "7")
                .Replace("۸", "8")
                .Replace("۹", "9");
        }

        public static decimal toDecimal(object obj)
        {
            decimal OutDecimal = 0;

            if (!string.IsNullOrEmpty(DomainUtility.toString(obj)))
            {
                obj = obj.ToString().Replace(",", "").Replace("،", "").Replace("/", ".");
                System.Globalization.CultureInfo culInfo = new System.Globalization.CultureInfo("en-GB", true);
                if (!decimal.TryParse(DomainUtility.enNumbers(DomainUtility.toString(obj)), NumberStyles.AllowDecimalPoint, culInfo, out OutDecimal))
                    return 0;
            }
            else
            {
                return 0;
            }

            return OutDecimal;
        }

        public static string GetCulture()
        {
            string strLang = System.Globalization.CultureInfo.CurrentCulture.ToStringObj().ToLower().Replace("en-us", "");
            if (strLang.ToLower() == "fa-ir" || System.Web.HttpContext.Current?.Request?.QueryString["language"].ToStringObj().ToLower() == "fa-ir")
                strLang = "fa-IR";
            if (string.IsNullOrEmpty(strLang))
                return string.Empty;
            return strLang;
        }

        public static bool IsTestEnvironment()
        {
            return false;
        }

        public static string[] GetRegularValue(string Start, string End, string Value)
        {
            Value = Value ?? string.Empty;
            var pattern = new Regex(Regex.Escape(Start) + "(.*?)" + Regex.Escape(End)).ToString();
            if (Regex.Matches(Value, pattern).Cast<Match>().Select(match => match.Groups[1].Value).Any())
                return Regex.Matches(Value, pattern).Cast<Match>().Select(match => match.Groups[1].Value).ToArray();
            else
                return new string[] { };
        }
        //replace NewVal with start+getValue+End
        public static string ReplaceRegularValue(string Start, string End, string newVal, string value, bool includeStartEnd = false)
        {
            string pattern = new Regex(Regex.Escape(Start) + "(.*?)" + Regex.Escape(End)).ToString();
            if (includeStartEnd)
            {
                GetRegularValue(Start, End, value).ToList().ForEach(c => value = value.Replace(Start + c + End, newVal));
                return value;
            }
            return Regex.Replace(value, Start + "(.*?)" + End, newVal);
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName)?.GetValue(src, null);
        }

        public static List<string> GetParameters(string sqlQuery)
        {
            return Regex.Matches(sqlQuery, @"\@[\w.]+").Cast<Match>().Select(m => m.Value).ToList();
        }

        public static string CreateApiSessionID(string sessionId, string ipAddress)
        {
            //it may change and use ipAddress as well
            return sessionId;
        }

        public static string CartableHomeUr { get; set; }
        public static string SingleActionHomeUr { get; set; }
        public static string AdminHomeUr { get; set; }


        private static string ConnectionString { get; set; }
        public static string GetConnectionName()
        {
            //if (DomainUtility.IsTestEnvironment())
            //    return "Db_BPMSEntities";
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;
                System.Data.SqlClient.SqlConnectionStringBuilder scsb = new System.Data.SqlClient.SqlConnectionStringBuilder(connection);

                EntityConnectionStringBuilder ecb = new EntityConnectionStringBuilder();
                ecb.Metadata = "res://*/dmBPMS.csdl|res://*/dmBPMS.ssdl|res://*/dmBPMS.msl";
                ecb.Provider = "System.Data.SqlClient";
                ecb.ProviderConnectionString = scsb.ConnectionString;
                ConnectionString = ecb.ConnectionString;
            }
            return ConnectionString;
        }
    }
}
