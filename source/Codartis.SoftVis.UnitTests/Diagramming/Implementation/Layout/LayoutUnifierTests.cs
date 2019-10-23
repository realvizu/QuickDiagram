using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Geometry;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout
{
    public class LayoutUnifierTests
    {
        private const double Padding = 1;

        [Fact]
        public void CalculateAbsoluteLayout_RootNodeOnly_Works()
        {
            var relativeLayoutInfo = new GroupLayoutInfo(
                new[]
                {
                    new BoxLayoutInfo("A", payloadAreaSize: (9, 1), topLeft: (1, 1), childrenAreaSize: Size2D.Zero),
                    new BoxLayoutInfo("B", payloadAreaSize: (8, 2), topLeft: (2, 2), childrenAreaSize: Size2D.Zero),
                },
                new[]
                {
                    new LineLayoutInfo("A->B", new Route((1, 1), (2, 2)))
                }
            );

            var absoluteLayoutInfo = CreateLayoutUnifier().CalculateAbsoluteLayout(relativeLayoutInfo);

            absoluteLayoutInfo.Should().BeEquivalentTo(relativeLayoutInfo);
        }

        [Fact]
        public void CalculateAbsoluteLayout_WithChildNodes_Works()
        {
            var relativeLayoutInfo = new GroupLayoutInfo(
                new[]
                {
                    new BoxLayoutInfo("A", topLeft: (0, 0), payloadAreaSize: (1, 1), childrenAreaSize: Size2D.Zero),
                    new BoxLayoutInfo(
                        "B",
                        topLeft: (2, 0),
                        payloadAreaSize: (2, 2),
                        childrenAreaSize: (10, 7),
                        new GroupLayoutInfo(
                            new[]
                            {
                                new BoxLayoutInfo("C", topLeft: (0, 0), payloadAreaSize: (3, 3), childrenAreaSize: Size2D.Zero),
                                new BoxLayoutInfo("D", topLeft: (4, 1), payloadAreaSize: (4, 4), childrenAreaSize: Size2D.Zero),
                            },
                            new[]
                            {
                                new LineLayoutInfo("C->D", new Route((3, 1.5), (4, 3)))
                            }
                        )
                    ),
                },
                new[]
                {
                    new LineLayoutInfo("A->B", new Route((1, 1), (2, 2)))
                }
            );

            var absoluteLayoutInfo = CreateLayoutUnifier().CalculateAbsoluteLayout(relativeLayoutInfo);

            var expectedLayoutInfo = new GroupLayoutInfo(
                new[]
                {
                    new BoxLayoutInfo("A", topLeft: (0, 0), payloadAreaSize: (1, 1), childrenAreaSize: Size2D.Zero),
                    new BoxLayoutInfo(
                        "B",
                        topLeft: (2, 0),
                        payloadAreaSize: (2, 2),
                        childrenAreaSize: (10, 7),
                        new GroupLayoutInfo(
                            new[]
                            {
                                new BoxLayoutInfo("C", topLeft: (3, 3), payloadAreaSize: (3, 3), childrenAreaSize: Size2D.Zero),
                                new BoxLayoutInfo("D", topLeft: (7, 4), payloadAreaSize: (4, 4), childrenAreaSize: Size2D.Zero),
                            },
                            new[]
                            {
                                new LineLayoutInfo("C->D", new Route((6, 4.5), (7, 6)))
                            }
                        )
                    )
                },
                new[]
                {
                    new LineLayoutInfo("A->B", new Route((1, 1), (2, 2)))
                }
            );

            absoluteLayoutInfo.Should().BeEquivalentTo(expectedLayoutInfo);
        }

        [NotNull]
        public static LayoutUnifier CreateLayoutUnifier() => new LayoutUnifier(Padding);
    }
}