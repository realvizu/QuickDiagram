using FluentAssertions;
using Xunit;

namespace Codartis.Util.UnitTests
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData(@"Prefix_", "Prefix_A", "A")]
        [InlineData(@"Prefix_", "prefix_A", "prefix_A")]
        [InlineData(@"Prefix_", "Prefix", "Prefix")]
        [InlineData(@"Prefix_", "Prefix_", "")]
        public void RemovePrefix(string prefix, string input, string expected)
        {
            input.RemovePrefix(prefix).Should().Be(expected);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("  a   ", " a ")]
        [InlineData(" \ta", " a")]
        [InlineData(" \n\ta", " a")]
        public void ToSingleWhitespaces_Works(string input, string expected)
        {
            input.ToSingleWhitespaces().Should().Be(expected);
        }
    }
}
