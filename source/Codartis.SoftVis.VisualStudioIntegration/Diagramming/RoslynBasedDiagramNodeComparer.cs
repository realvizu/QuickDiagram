using System.Collections.Generic;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    public sealed class RoslynBasedDiagramNodeComparer : IComparer<IDiagramNode>
    {
        [NotNull] private readonly IComparer<IModelNode> _modelNodeComparer;

        public RoslynBasedDiagramNodeComparer([NotNull] IComparer<IModelNode> modelNodeComparer)
        {
            _modelNodeComparer = modelNodeComparer;
        }

        public int Compare(IDiagramNode x, IDiagramNode y) => _modelNodeComparer.Compare(x.ModelNode, y.ModelNode);
    }
}