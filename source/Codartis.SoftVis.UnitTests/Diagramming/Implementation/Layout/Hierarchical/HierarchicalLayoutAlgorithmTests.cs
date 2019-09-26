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

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout.Hierarchical
{
    public sealed class HierarchicalLayoutAlgorithmTests
    {
        [NotNull] private readonly ModelBuilder _modelBuilder;
        [NotNull] private readonly TestLayoutAlgorithmSelectionStrategy _layoutAlgorithmSelectionStrategy;

        public HierarchicalLayoutAlgorithmTests()
        {
            _modelBuilder = new ModelBuilder();
            _layoutAlgorithmSelectionStrategy = new TestLayoutAlgorithmSelectionStrategy();
        }

        [Fact]
        public void Calculate_RootNodesOnly_Works()
        {
            var model = _modelBuilder.AddNodes("A", "B").AddRelationships("A->B").Model;
            var diagram = new DiagramBuilder(model).AddNodes(("A", 2, 3), ("B", 4, 5)).AddAllRelationships().Diagram;

            var cannedLayoutSpecification = new DiagramLayoutInfo();
            var testLayoutAlgorithm = new TestLayoutAlgorithm(cannedLayoutSpecification);
            _layoutAlgorithmSelectionStrategy.SetLayoutAlgorithmForRoot(testLayoutAlgorithm);

            var layoutSpecification = CreateLayoutAlgorithm().Calculate(diagram);

            // TODO: check diagramLayout for root

            layoutSpecification.NodeTopLeftPositions.Should().BeEquivalentTo(
                GetNodePosition("A", new Point2D(0, 0)),
                GetNodePosition("B", new Point2D(0, 3)));
        }

        private KeyValuePair<ModelNodeId, Point2D> GetNodePosition(string nodeName, Point2D topLeft)
            => new KeyValuePair<ModelNodeId, Point2D>(_modelBuilder.GetNodeIdByName(nodeName), topLeft);

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
            public DiagramLayoutInfo DiagramLayoutInfo { get; }

            public TestLayoutAlgorithm(DiagramLayoutInfo diagramLayoutInfo)
            {
                DiagramLayoutInfo = diagramLayoutInfo;
            }

            public DiagramLayoutInfo Calculate(ILayoutGroup layoutGroup)
            {
                return DiagramLayoutInfo;
            }
        }
    }
}