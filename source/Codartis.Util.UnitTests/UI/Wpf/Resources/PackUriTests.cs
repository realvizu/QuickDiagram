using System;
using Codartis.Util.UI.Wpf.Resources;
using FluentAssertions;
using Xunit;

namespace Codartis.Util.UnitTests.UI.Wpf.Resources
{
    public class PackUriTests
    {
        [Fact]
        public void RelativePartReplacement_Works()
        {
            new PackUri(
                    @"pack://application:,,,/QuickDiagramTool;component/UI/StereotypeStyleMaps.xaml",
                    @"/UI/Images/Class_16x.xaml")
                .ToString()
                .Should().Be(@"pack://application:,,,/QuickDiagramTool;component/UI/Images/Class_16x.xaml");
        }

        [Fact]
        public void ImplicitConversionToUri_Works()
        {
            Uri uri = new PackUri(null, "pack://host/Path");
            uri.ToString().Should().Be(@"pack://host/Path");
        }
    }
}