using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Codartis.SoftVis.VisualStudioIntegration.Util;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.VisualStudioIntegration.UnitTests.Util
{
    public class XmlDocumentationTests
    {
        [Fact]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void Create_NonXmlInput_Exception()
        {
            Action a = () => new XmlDocumentation("kutya");
            a.Should().Throw<XmlException>();
        }

        [Fact]
        public void GetSummary_NullInput_Works()
        {
            new XmlDocumentation(null).Summary.Should().BeNull();
        }

        [Fact]
        public void GetSummary_EmptyInput_Works()
        {
            new XmlDocumentation(string.Empty).Summary.Should().BeNull();
        }

        [Fact]
        public void GetSummary_NonEmptyInput_Works()
        {
            const string xmlString = "<doc><summary>UnitTest</summary></doc>";
            new XmlDocumentation(xmlString).Summary.Should().BeEquivalentTo("UnitTest");
        }

        [Fact]
        public void GetSummary_XmlWithTyperefs_Works()
        {
            const string xmlString = @"<doc><summary>Unit <see cref=""T:System.IO.FileInfo""/> test</summary></doc>";
            new XmlDocumentation(xmlString).Summary.Should().BeEquivalentTo("Unit System.IO.FileInfo test");
        }

        [Fact]
        public void GetSummary_MultipleWhitespacesReplacedWithSingleSpace()
        {
            const string xmlString = "<doc><summary>Unit  \n\r\t  \t \n \rtest</summary></doc>";
            new XmlDocumentation(xmlString).Summary.Should().BeEquivalentTo("Unit test");
        }
    }
}
