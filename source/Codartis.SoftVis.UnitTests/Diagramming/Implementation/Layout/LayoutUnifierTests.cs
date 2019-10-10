using System.Diagnostics.CodeAnalysis;
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
                    new BoxLayoutInfo(new TestBoxShape("A", payloadAreaSize: (9, 1)), topLeft: (1, 1)),
                    new BoxLayoutInfo(new TestBoxShape("B", payloadAreaSize: (8, 2)), topLeft: (2, 2)),
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
                        new TestBoxShape("A", payloadAreaSize: (9, 1)),
                        topLeft: (1, 1),
                        new GroupLayoutInfo(
                            new[]
                            {
                                new BoxLayoutInfo(new TestBoxShape("A1", payloadAreaSize: (1, 1)), topLeft: (1, 1)),
                                new BoxLayoutInfo(new TestBoxShape("A2", payloadAreaSize: (2, 2)), topLeft: (2, 2)),
                            }
                        )),
                    new BoxLayoutInfo(new TestBoxShape("B", payloadAreaSize: (8, 2)), topLeft: (2, 2)),
                }
                // TODO: connectors
            );

            var absoluteLayoutInfo = CreateLayoutUnifier().CalculateAbsoluteLayout(relativeLayoutInfo);

            absoluteLayoutInfo.Should().BeEquivalentTo(
                new GroupLayoutInfo(
                    new[]
                    {
                        new BoxLayoutInfo(
                            new TestBoxShape("A", payloadAreaSize: (9, 1)),
                            topLeft: (1, 1),
                            new GroupLayoutInfo(
                                new[]
                                {
                                    new BoxLayoutInfo(new TestBoxShape("A1", payloadAreaSize: (1, 1)), topLeft: (2, 3)),
                                    new BoxLayoutInfo(new TestBoxShape("A2", payloadAreaSize: (2, 2)), topLeft: (3, 4)),
                                }
                            )),
                        new BoxLayoutInfo(new TestBoxShape("B", payloadAreaSize: (8, 2)), topLeft: (2, 2)),
                    }
                )
            );
        }

        [NotNull]
        public static LayoutUnifier CreateLayoutUnifier() => new LayoutUnifier(Padding);

        [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
        private class TestBoxShape : IBoxShape
        {
            public string ShapeId { get; }
            public Size2D PayloadAreaSize { get; }

            public Rect2D Rect { get; }
            public string Name { get; }
            public Size2D ChildrenAreaSize { get; }

            public TestBoxShape(string shapeId, Size2D payloadAreaSize)
            {
                ShapeId = shapeId;
                PayloadAreaSize = payloadAreaSize;
            }
        }
    }
}