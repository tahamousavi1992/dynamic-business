using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DynamicBusiness.BPMS.Domain
{
    public static class ParseHelpers
    {
        public static Stream ToStream(this string @this)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(@this);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static T ParseXML<T>(this string @this) where T : class
        {
            if (string.IsNullOrWhiteSpace(@this))
                return (T)Activator.CreateInstance(typeof(T));
           
            if (!@this.StartsWith("<?xml version=\"1.0\" encoding=\"utf-16\"?>"))
                @this = "<?xml version=\"1.0\" encoding=\"utf-16\"?>" + @this;

            string kUtf8BOM = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetPreamble());
            if (@this.StartsWith(kUtf8BOM)) @this = @this.Remove(0, kUtf8BOM.Length);
            if (@this.StartsWith("?xm")) @this = "<" + @this;

            StringReader Stream = null;
            XmlTextReader Reader = null;
            try
            {
                System.Xml.Serialization.XmlAttributeOverrides xOver = new System.Xml.Serialization.XmlAttributeOverrides();
                System.Xml.Serialization.XmlAttributes attrs = new System.Xml.Serialization.XmlAttributes();
                foreach (var Ig in typeof(T).GetProperties().Where(c => !c.PropertyType.IsSerializable || c.PropertyType.FullName.StartsWith("DynamicBusiness.BPMS.BusinessLogic") || c.PropertyType.FullName.StartsWith("DynamicBusiness.BPMS.Engine")).Select(c => c.Name))
                {
                    attrs = new System.Xml.Serialization.XmlAttributes();
                    attrs.XmlIgnore = true;
                    xOver.Add(typeof(T), Ig, attrs);
                }

                XmlSerializer Serializer = new XmlSerializer(typeof(T), xOver);
                Stream = new StringReader(@this);
                Reader = new XmlTextReader(Stream);

                // covert reader to object 
                return (T)Serializer.Deserialize(Reader);
            }
            catch (Exception ex)
            {
                return default(T);
            }
            finally
            {
                if (Stream != null) Stream.Close();
                if (Reader != null) Reader.Close();
            }
        }

        public static string BuildXml<T>(this T BusinessObject) where T : class
        {
            MemoryStream Stream = null;
            TextWriter Writer = null;
            try
            {
                System.Xml.Serialization.XmlAttributeOverrides xOver = new System.Xml.Serialization.XmlAttributeOverrides();
                System.Xml.Serialization.XmlAttributes attrs = new System.Xml.Serialization.XmlAttributes();

                foreach (var Ig in typeof(T).GetProperties().Where(c =>
                !c.PropertyType.IsSerializable ||
                c.PropertyType.FullName.StartsWith("DynamicBusiness.BPMS.BusinessLogic")).Select(c => c.Name))
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
                return System.Web.HttpUtility.HtmlDecode(utf.GetString(BytesArr).Trim());

                //return utf.GetString(BytesArr).Trim();
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
    }
}
