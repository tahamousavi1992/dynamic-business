using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DynamicBusiness.BPMS.BusinessLogic
{
    public static class XMLExtensions
    {
        public static XElement XPathSelectElementByAttribute(this XNode node, string path, string attributeName, string attributeValue)
        {
            return node.XPathSelectElement(path + "[@" + attributeName + "=\"" + attributeValue + "\"]");
        }
        public static XElement XPathSelectElementByGuid(this XNode node, string path, Guid guid)
        {
            return node.XPathSelectElementByAttribute(path, "guid", guid.ToString());
        }
         
        public static string TryGetAttributeFromElement(this XElement element, string attrName)
        {
            var attrElement = element.Attribute(attrName);
            if (attrElement != null)
                return attrElement.Value;
            else
                return string.Empty;
        }
        public static void AddAttributeToElement(this XElement element, string attrName, object attrValue)
        {
            if (attrValue == null || (attrValue is string && string.IsNullOrWhiteSpace(attrValue.ToString()))) return;

            var attr = new XAttribute(attrName, attrValue);
            element.Add(attr);
        }
    }
}
