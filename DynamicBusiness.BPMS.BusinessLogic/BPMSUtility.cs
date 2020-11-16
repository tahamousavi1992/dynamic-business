using DotNetNuke.Entities.Users;
using DotNetNuke.Web.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using DynamicBusiness.BPMS.Domain;
namespace DynamicBusiness.BPMS.BusinessLogic
{

    public static class BPMSUtility
    {

        public static string GetXAttributeValue(XElement element, string attrName)
        {
            var attrElement = element.Attribute(attrName);
            if (attrElement != null)
                return attrElement.Value;
            else
                return string.Empty;
            //throw new Exception("There isn't any " + attrTagName + " for element");
        }

        public static Expression<Func<TEntity, bool>> CreateExpression<TEntity>(string propertyName, string valueToCompare)
        {
            // get the type of entity
            var entityType = typeof(TEntity);
            // get the type of the value object
            var valueType = valueToCompare.GetType();
            var entityProperty = entityType.GetProperty(propertyName);
            var propertyType = entityProperty.PropertyType;


            // Expression: "entity"
            var item = Expression.Parameter(entityType, "item");

            // check if the property type is a value type
            // only value types work 
            if (propertyType.IsValueType || propertyType.Equals(typeof(string)))
            {
                // Expression: entity.Property == value
                var equal = Expression.Equal(
                    Expression.Property(item, entityProperty),
                    Expression.Constant(valueToCompare)
                );
                var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, item);
                return lambda;
            }
            // if not, then use the key
            else
            {
                // get the key property
                var keyProperty = propertyType.GetProperties().FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Length > 0);

                // Expression: entity.Property.Key == value.Key
                var equal = Expression.Equal(
                    Expression.Property(
                        Expression.Property(item, entityProperty),
                        keyProperty
                    ),
                    Expression.Constant(
                        keyProperty.GetValue(valueToCompare),
                        keyProperty.PropertyType
                    )
                );
                var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, item);
                return lambda;
            }
        }

        public static T To<T>(this IConvertible obj)
        {
            return (T)Convert.ChangeType(obj, typeof(T));
        }

        public static T1 ConvertTo<T1>(object obj2)
        {
            var instance = Activator.CreateInstance(typeof(T1));
            if (obj2 == null) return default(T1);
            foreach (string Ig in obj2.GetType().GetProperties().Where(c => c.PropertyType.IsSerializable).Select(c => c.Name))
            {
                if (instance.GetType().GetProperty(Ig) != null)
                    instance.GetType().GetProperty(Ig).SetValue(instance, obj2.GetType().GetProperty(Ig).GetValue(obj2));
            }

            return (T1)instance;
        }

        

        /// <summary>
        /// Creates a connection string by SqlConnectionStringBuilder and returns it.
        /// </summary>
        /// <returns></returns>
        public static string GetSqlConnectionString(sysBpmsDBConnection DBConnection)
        {

            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder()
            {
                DataSource = DBConnection.DataSource,
                InitialCatalog = DBConnection.InitialCatalog,
                MultipleActiveResultSets = true,
            };

            // Check current Windows account credentials are used for authentication
            if (DBConnection.IntegratedSecurity)
                connectionString.IntegratedSecurity = true;
            else
            {
                connectionString.Password = DBConnection.Password;
                connectionString.UserID = DBConnection.UserID;
            }

            return connectionString.ConnectionString;
        }

        /// <summary>
        /// Tests connection string by opening it
        /// </summary>
        /// <returns></returns>
        public static bool TestConnection(sysBpmsDBConnection DBConnection)
        {
            try
            {
                string connectionString = GetSqlConnectionString(DBConnection);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open(); // throws if invalid
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CheckDuplicateTable(string TableName)
        {
            using (Db_BPMSEntities db = new Db_BPMSEntities())
            {
                bool exists = db.Database
                      .SqlQuery<int?>(@"
                         SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES
                         WHERE TABLE_NAME = '" + TableName + "'")
                      .FirstOrDefault() > 0;

                return exists;
            }
        }

        public static string GetXMLAsString(XmlNode xml)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);
            xml.WriteTo(tx);
            return sw.ToString();
        }

        public static XmlDocument StringToXml(string xmlString)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(xmlString);
            return xdoc;
        }

        public static string SafeFarsiStr(object input)
        {
            if (input == null)
                return null;
            string Result = BPMSUtility.toString(input);
            if (string.IsNullOrWhiteSpace(Result))
                return Result;
            return Result.Replace("ي", "ی").Replace("ك", "ک");
        }

        public static void SafeFarsiStrObject(object Entity)
        {
            try
            {
                Entity.GetType().GetProperties().Where(c => c.PropertyType == typeof(string) && c.CanWrite && c.CanRead).ToList().ForEach(c => c.SetValue(Entity, BPMSUtility.SafeFarsiStr(c.GetValue(Entity, null))));
            }
            catch
            {

            }
        }

        /// ------------------------------------------------------------------------------------------------
        /// ------------------------------------------------------------------------------------------------
        public static string BusinessObjectAsXml<T>(T BusinessObject, string[] IgnoreArray)
        {
            MemoryStream Stream = null;
            TextWriter Writer = null;
            try
            {
                System.Xml.Serialization.XmlAttributeOverrides xOver = new System.Xml.Serialization.XmlAttributeOverrides();
                System.Xml.Serialization.XmlAttributes attrs = new System.Xml.Serialization.XmlAttributes();

                foreach (var Ig in typeof(T).GetProperties().Where(c => !c.PropertyType.IsSerializable || c.PropertyType.FullName.StartsWith("DynamicBusiness.BPMS.BusinessLogic")).Select(c => c.Name))
                {
                    attrs = new System.Xml.Serialization.XmlAttributes();
                    attrs.XmlIgnore = true;
                    xOver.Add(typeof(T), Ig, attrs);
                }

                Stream = new MemoryStream();
                Writer = new StreamWriter(Stream, Encoding.Unicode);

                XmlSerializer Serializer = new XmlSerializer(typeof(T), xOver);
                Serializer.Serialize(Writer, BusinessObject);

                int Count = (int)Stream.Length;
                byte[] BytesArr = new byte[Count];

                Stream.Seek(0, SeekOrigin.Begin);
                Stream.Read(BytesArr, 0, Count);

                UnicodeEncoding utf = new UnicodeEncoding();
                return utf.GetString(BytesArr).Trim();
            }
            catch (Exception exc)
            {
                DotNetNuke.Services.Exceptions.Exceptions.LogException(exc);
                return string.Empty;
            }
            finally
            {
                if (Stream != null) Stream.Close();
                if (Writer != null) Writer.Close();
            }
        }
        /// ------------------------------------------------------------------------------------------------
        /// ------------------------------------------------------------------------------------------------
        ///

        public static T XmlAsBusinessObject<T>(string XmlData, string[] IgnoreArray)
        {
            StringReader Stream = null;
            XmlTextReader Reader = null;
            try
            {
                System.Xml.Serialization.XmlAttributeOverrides xOver = new System.Xml.Serialization.XmlAttributeOverrides();
                System.Xml.Serialization.XmlAttributes attrs = new System.Xml.Serialization.XmlAttributes();

                foreach (var Ig in typeof(T).GetProperties().Where(c => !c.PropertyType.IsSerializable || c.PropertyType.FullName.StartsWith("DynamicBusiness.BPMS.BusinessLogic")).Select(c => c.Name))
                {
                    attrs = new System.Xml.Serialization.XmlAttributes();
                    attrs.XmlIgnore = true;
                    xOver.Add(typeof(T), Ig, attrs);
                }

                XmlSerializer Serializer = new XmlSerializer(typeof(T), xOver);
                Stream = new StringReader(XmlData);
                Reader = new XmlTextReader(Stream);

                // covert reader to object 
                return (T)Serializer.Deserialize(Reader);
            }
            catch
            {
                return default(T);
            }
            finally
            {
                if (Stream != null) Stream.Close();
                if (Reader != null) Reader.Close();
            }
        }
        /// ------------------------------------------------------------------------------------------------
        /// ------------------------------------------------------------------------------------------------
        /// 

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

        // ------------------------------------------------------------------------------------------------
        // ------------------------------------------------------------------------------------------------
        public static int toInt(object obj)
        {
            int OutInt = 0;
            if (!string.IsNullOrEmpty(BPMSUtility.toString(obj)))
                if (!int.TryParse(BPMSUtility.enNumbers(BPMSUtility.toString(obj)), out OutInt))
                    OutInt = 0;
            return OutInt;
        }
        public static Guid toGuid(object obj)
        {
            Guid OutInt = Guid.Empty;
            if (!string.IsNullOrEmpty(BPMSUtility.toString(obj)))
                if (!Guid.TryParse(BPMSUtility.enNumbers(BPMSUtility.toString(obj)), out OutInt))
                    OutInt = Guid.Empty;
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

            if (!string.IsNullOrEmpty(BPMSUtility.toString(obj)))
            {
                obj = obj.ToString().Replace(",", "").Replace("،", "").Replace("/", ".");
                System.Globalization.CultureInfo culInfo = new System.Globalization.CultureInfo("en-GB", true);
                if (!decimal.TryParse(BPMSUtility.enNumbers(BPMSUtility.toString(obj)), NumberStyles.AllowDecimalPoint, culInfo, out OutDecimal))
                    return 0;
            }
            else
            {
                return 0;
            }

            return OutDecimal;
        }

        public static string RemoveController(string name)
        {
            return name.Replace("Controller", "");
        }
    }
}
