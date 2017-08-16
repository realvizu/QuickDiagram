using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Specializes the diagram class for the VS integrated usage.
    /// </summary>
    internal class RoslynBasedDiagram : AutoArrangingDiagram, IDiagramServices
    {
        private readonly IModelServices _modelServices;

        public RoslynBasedDiagram(IModelServices modelServices)
            : base(modelServices, new RoslynDiagramBuilder(), new RoslynDiagramNodeFactory())
        {
            _modelServices = modelServices;
        }

        public IDiagramNode ShowModelNode(IRoslynModelNode modelNode)
        {
            return ShowModelItem(modelNode) as IDiagramNode;
        }

        public IReadOnlyList<IDiagramNode> ShowModelNodes(IEnumerable<IRoslynModelNode> modelNodes, CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            return ShowModelItems(modelNodes, cancellationToken, progress).OfType<IDiagramNode>().ToArray();
        }

        public IReadOnlyList<IDiagramNode> ShowModelNodeWithHierarchy(IRoslynModelNode modelNode, CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            var baseTypes = _modelServices.CurrentModel.GetRelatedNodes(modelNode.Id, DirectedRelationshipTypes.BaseType, recursive: true);
            var subtypes = _modelServices.CurrentModel.GetRelatedNodes(modelNode.Id, DirectedRelationshipTypes.Subtype, recursive: true);
            var entities = new[] { modelNode }.Union(baseTypes).Union(subtypes);

            return ShowModelItems(entities, cancellationToken, progress).OfType<IDiagramNode>().ToArray();
        }

        public void UpdateFromSource(CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            foreach (var diagramNode in Nodes)
            {
                _modelServices.ExtendModelWithRelatedEntities(diagramNode.ModelNode, cancellationToken: cancellationToken);
                progress?.Report(1);
            }
        }
    }
}
