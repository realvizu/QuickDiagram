using Codartis.SoftVis.Geometry;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Geometry
{
    public class RouteTests
    {
        [Fact]
        public void Equals_Works()
        {
            var emptyRoute = Route.Empty;

            (Route.Empty == emptyRoute).Should().BeTrue();
            (Route.Empty == new Route()).Should().BeTrue();
            (Route.Empty == new Route(new Point2D[0])).Should().BeTrue();

            var route1 = new Route(new[] { new Point2D(1, 1), new Point2D(2, 2) });

            (route1 == new Route(new[] { new Point2D(1, 1), new Point2D(2, 2) })).Should().BeTrue();
            (route1 == new Route(new[] { new Point2D(1, 1), new Point2D(3, 3) })).Should().BeFalse();
            (route1 == new Route(new[] { new Point2D(1, 1) })).Should().BeFalse();
            (route1 == Route.Empty).Should().BeFalse();
        }

        [Fact]
        public void Normalize_Works()
        {
            var route1 = new Route(new[]
            {
                new Point2D(1, 1),
                new Point2D(1, 1),
                new Point2D(2, 2),
                new Point2D(2, 2),
                new Point2D(1, 1),
            });

            (route1 == new Route(new[] { new Point2D(1, 1), new Point2D(2, 2), new Point2D(1, 1) })).Should().BeTrue();
        }
    }
}