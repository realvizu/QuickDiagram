using Codartis.SoftVis.Geometry;
using Codartis.Util;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Geometry
{
    public class RectExtensionsTests
    {
        [NotNull]
        public static TheoryData<double[], Rect2D> TestData
            => new TheoryData<double[], Rect2D>
            {
                { new double[] { }, Rect2D.Undefined },
                { new double[] { 1, 2 }, new Rect2D(1, 2, 1, 2) },
                { new double[] { 1, 2, 3, 4 }, new Rect2D(1, 2, 3, 4) },
                { new double[] { 1, 2, 3, 4, -1, -1 }, new Rect2D(-1, -1, 3, 4) },
            };

        [Theory]
        [MemberData(nameof(TestData))]
        public void ToRect_Works(double[] coordinates, Rect2D expectedRect)
        {
            var route = new Route(coordinates.SelectPairs((x, y) => new Point2D(x, y)));
            route.ToRect().Should().Be(expectedRect);
        }
    }
}