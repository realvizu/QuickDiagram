using System.Linq;
using System.Xml.Linq;
using Codartis.Util;

namespace Codartis.SoftVis.VisualStudioIntegration.Util
{
    /// <summary>
    /// Extracts information from xml comment documentation as returned by the Roslyn API.
    /// </summary>
    public class XmlDocumentation
    {
        private readonly XDocument _xml;

        public XmlDocumentation(string xmlAsString)
        {
            _xml = string.IsNullOrWhiteSpace(xmlAsString)
                ? null
                : XDocument.Parse(xmlAsString);
        }

        public string Summary => GetSummary(_xml);

        private static string GetSummary(XDocument xml)
        {
            var stringParts = xml?.Descendants("summary").FirstOrDefault()?.DescendantNodes().Select(GetDocumentationText);
            var result = stringParts == null ? null : string.Join("", stringParts);
            return result.ToSingleWhitespaces();
        }

        private static string GetDocumentationText(XNode xNode)
        {
            if (xNode == null)
                return string.Empty;

            var xElement = xNode as XElement;
            if (xElement == null)
                return xNode.ToString();

            return xElement.Name == "see"
                    ? xElement.Attribute("cref")?.Value.RemovePrefix("T:")
                    : string.Empty;
        }
    }
}
