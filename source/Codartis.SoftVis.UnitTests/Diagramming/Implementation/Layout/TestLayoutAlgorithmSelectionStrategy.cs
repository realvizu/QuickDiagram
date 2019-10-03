using System.Collections.Generic;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Layout;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.UnitTests.Diagramming.Implementation.Layout
{
    public class TestLayoutAlgorithmSelectionStrategy : ILayoutAlgorithmSelectionStrategy
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
}