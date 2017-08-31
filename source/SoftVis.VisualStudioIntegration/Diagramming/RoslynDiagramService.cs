using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Implements Roslyn-specific extensions to diagram operations.
    /// </summary>
    internal class RoslynDiagramService : DiagramService, IRoslynDiagramService
    {
        public RoslynDiagramService(IReadOnlyModelStore modelStore, IDiagramStore diagramStore, IDiagramShapeFactory diagramShapeFactory) 
            : base(modelStore, diagramStore, diagramShapeFactory)
        {
        }

        public IEnumerable<IDiagramNode> ShowModelNodeWithHierarchy(IRoslynModelNode modelNode, 
            CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            var model = ModelStore.CurrentModel;

            var baseTypes = model.GetRelatedNodes(modelNode, DirectedRelationshipTypes.BaseType, recursive: true);
            var subtypes = model.GetRelatedNodes(modelNode, DirectedRelationshipTypes.Subtype, recursive: true);
            var modelNodes = new[] { modelNode }.Union(baseTypes).Union(subtypes);

            return ShowModelNodes(modelNodes, cancellationToken, progress);
        }
    }
}
