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
                }
                // TODO: connectors
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
                    new BoxLayoutInfo(
                        "A",
                        topLeft: (1, 1),
                        payloadAreaSize: (9, 1),
                        childrenAreaSize: (3, 3),
                        new GroupLayoutInfo(
                            new[]
                            {
                                new BoxLayoutInfo("A1", topLeft: (5, 5), payloadAreaSize: (1, 1), childrenAreaSize: Size2D.Zero),
                                new BoxLayoutInfo("A2", topLeft: (6, 6), payloadAreaSize: (2, 2), childrenAreaSize: Size2D.Zero),
                            }
                        )),
                    new BoxLayoutInfo("B", topLeft: (2, 2), payloadAreaSize: (8, 2), childrenAreaSize: Size2D.Zero),
                }
                // TODO: connectors
            );

            var absoluteLayoutInfo = CreateLayoutUnifier().CalculateAbsoluteLayout(relativeLayoutInfo);

            var expectedLayoutInfo = new GroupLayoutInfo(
                new[]
                {
                    new BoxLayoutInfo(
                        "A",
                        topLeft: (1, 1),
                        payloadAreaSize: (9, 1),
                        childrenAreaSize: (5, 5),
                        new GroupLayoutInfo(
                            new[]
                            {
                                new BoxLayoutInfo("A1", topLeft: (2, 3), payloadAreaSize: (1, 1), childrenAreaSize: Size2D.Zero),
                                new BoxLayoutInfo("A2", topLeft: (3, 4), payloadAreaSize: (2, 2), childrenAreaSize: Size2D.Zero),
                            }
                        )),
                    new BoxLayoutInfo("B", topLeft: (2, 2), payloadAreaSize: (8, 2), childrenAreaSize: Size2D.Zero),
                }
            );

            absoluteLayoutInfo.Should().BeEquivalentTo(expectedLayoutInfo);
        }

        [NotNull]
        public static LayoutUnifier CreateLayoutUnifier() => new LayoutUnifier(Padding);
    }
}