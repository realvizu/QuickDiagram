using System.Collections.Generic;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Diagramming.Implementation.Layout;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UnitTests.Modeling;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout
{
    public sealed class DiagramLayoutAlgorithmTests
    {
        [NotNull] private readonly ModelBuilder _modelBuilder;
        [NotNull] private readonly TestLayoutAlgorithmSelectionStrategy _layoutAlgorithmSelectionStrategy;

        public DiagramLayoutAlgorithmTests()
        {
            _modelBuilder = new ModelBuilder();
            _layoutAlgorithmSelectionStrategy = new TestLayoutAlgorithmSelectionStrategy();
        }

        [Fact]
        public void Calculate_RootNodesOnly_Works()
        {
            var model = _modelBuilder.AddNodes("A", "B").AddRelationships("A->B").Model;
            var diagramBuilder = new DiagramBuilder(model).AddNodes(("A", 2, 3), ("B", 4, 5)).AddAllRelationships();

            var cannedLayoutInfo = new GroupLayoutInfo(new List<NodeLayoutInfo>
            {
                new NodeLayoutInfo(diagramBuilder.GetDiagramNodeByName("A"), new Point2D(0,0)),
                new NodeLayoutInfo(diagramBuilder.GetDiagramNodeByName("B"), new Point2D(1,1)),
            });
            var testLayoutAlgorithm = new TestLayoutAlgorithm(cannedLayoutInfo);
            _layoutAlgorithmSelectionStrategy.SetLayoutAlgorithmForRoot(testLayoutAlgorithm);

            var diagramLayoutInfo = CreateLayoutAlgorithm().Calculate(diagramBuilder.Diagram);

            diagramLayoutInfo.RootNodes.Should().BeEquivalentTo(cannedLayoutInfo.Nodes);
        }

        [NotNull]
        private IDiagramLayoutAlgorithm CreateLayoutAlgorithm()
        {
            return new DiagramLayoutAlgorithm(_layoutAlgorithmSelectionStrategy, childAreaMargin: 0);
        }

        private class TestLayoutAlgorithmSelectionStrategy : ILayoutAlgorithmSelectionStrategy
        {
            private IGroupLayoutAlgorithm _rootLayoutAlgorithm;
            private readonly IDictionary<ModelNodeId, IGroupLayoutAlgorithm> _nodeLayoutAlgorithms = new Dictionary<ModelNodeId, IGroupLayoutAlgorithm>();

            public void SetLayoutAlgorithmForRoot(IGroupLayoutAlgorithm layoutAlgorithm)
            {
                _rootLayoutAlgorithm = layoutAlgorithm;
            }

            public IGroupLayoutAlgorithm GetForRoot()
            {
                return _rootLayoutAlgorithm;
            }

            public void SetLayoutAlgorithmForNode(IDiagramNode node, IGroupLayoutAlgorithm layoutAlgorithm)
            {
                _nodeLayoutAlgorithms.Add(node.Id, layoutAlgorithm);
            }

            public IGroupLayoutAlgorithm GetForNode(IDiagramNode node)
            {
                return _nodeLayoutAlgorithms[node.Id];
            }
        }

        private class TestLayoutAlgorithm : IGroupLayoutAlgorithm
        {
            public GroupLayoutInfo GroupLayoutInfo { get; }

            public TestLayoutAlgorithm(GroupLayoutInfo groupLayoutInfo)
            {
                GroupLayoutInfo = groupLayoutInfo;
            }

            public GroupLayoutInfo Calculate(ILayoutGroup layoutGroup)
            {
                return GroupLayoutInfo;
            }
        }
    }
}