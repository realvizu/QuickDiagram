using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Service;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    internal class RoslynVisualizationService : VisualizationService, IRoslynVisualizationService
    {
        private readonly IRoslynModelProvider _roslynModelProvider;

        public RoslynVisualizationService(
            IModelStoreFactory modelStoreFactory, 
            IDiagramStoreFactory diagramStoreFactory, 
            IDiagramShapeFactory diagramShapeFactory, 
            IDiagramUiFactory diagramUiFactory, 
            IDiagramShapeUiFactory diagramShapeUiFactory, 
            IDiagramPluginFactory diagramPluginFactory, 
            IEnumerable<DiagramPluginId> diagramPluginIds,
            IRoslynModelProvider roslynModelProvider) 
            : base(modelStoreFactory, 
                  diagramStoreFactory, 
                  diagramShapeFactory, 
                  diagramUiFactory, 
                  diagramShapeUiFactory, 
                  diagramPluginFactory, 
                  diagramPluginIds)
        {
            _roslynModelProvider = roslynModelProvider;
        }

        public Task<bool> IsCurrentSymbolAvailableAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IRoslynModelNode> AddCurrentSymbolAsync()
        {
            throw new NotImplementedException();
        }

        public void ExtendModelWithRelatedNodes(IRoslynModelNode modelNode, DirectedModelRelationshipType? directedModelRelationshipType = null, CancellationToken cancellationToken = new CancellationToken(), IIncrementalProgress progress = null, bool recursive = false)
        {
            throw new NotImplementedException();
        }

        public void ShowModelNodeWithHierarchy(DiagramId diagramId, IRoslynModelNode modelNode, CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            var baseTypes = ModelStore.CurrentModel.GetRelatedNodes(modelNode, DirectedRelationshipTypes.BaseType, recursive: true);
            var subtypes = ModelStore.CurrentModel.GetRelatedNodes(modelNode, DirectedRelationshipTypes.Subtype, recursive: true);
            var modelNodes = new[] { modelNode }.Union(baseTypes).Union(subtypes);

            ShowModelNodes(diagramId, modelNodes, cancellationToken, progress);
        }

        public bool HasSource(IRoslynModelNode modelNode) 
            => _roslynModelProvider.HasSource(modelNode.RoslynSymbol);

        public void ShowSource(IRoslynModelNode modelNode) 
            => _roslynModelProvider.ShowSource(modelNode.RoslynSymbol);
    }
}
