using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util;

namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// Implement diagram-related operations.
    /// </summary>
    public class DiagramService : IDiagramService
    {
        protected IReadOnlyModelStore ModelStore { get; }
        public IDiagramStore DiagramStore { get; }
        protected IDiagramShapeFactory DiagramShapeFactory { get; }

        public DiagramService(IReadOnlyModelStore modelStore, IDiagramStore diagramStore, IDiagramShapeFactory diagramShapeFactory)
        {
            ModelStore = modelStore;
            DiagramStore = diagramStore;
            DiagramShapeFactory = diagramShapeFactory;
        }

        public IDiagramNode ShowModelNode(IModelNode modelNode)
        {
            var diagram = DiagramStore.CurrentDiagram;
            if (diagram.TryGetNodeById(modelNode.Id, out var existingDiagramNode))
                return existingDiagramNode;

            var diagramNode = DiagramShapeFactory.CreateDiagramNode(DiagramStore, modelNode);
            DiagramStore.AddNode(diagramNode);
            return diagramNode;
        }

        public IEnumerable<IDiagramNode> ShowModelNodes(IEnumerable<IModelNode> modelNodes, 
            CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            var diagramNodes = new List<IDiagramNode>();

            foreach (var modelNode in modelNodes)
            {
                if (cancellationToken.IsCancellationRequested)
                    return Enumerable.Empty<IDiagramNode>();

                var diagramNode = ShowModelNode(modelNode);
                diagramNodes.Add(diagramNode);

                progress.Report(1);
            }

            return diagramNodes;
        }

        public void HideModelNode(IModelNode modelNode)
        {
            var diagram = DiagramStore.CurrentDiagram;
            if (diagram.TryGetNodeById(modelNode.Id, out IDiagramNode diagramNode))
            {
                var diagramConnectors = diagram.GetConnectorsByNodeId(modelNode.Id);
                foreach (var diagramConnector in diagramConnectors)
                    DiagramStore.RemoveConnector(diagramConnector);

                DiagramStore.RemoveNode(diagramNode);
            }
        }

        public void ShowModelRelationship(IModelRelationship modelRelationship)
        {
            var diagram = DiagramStore.CurrentDiagram;
            if (diagram.ConnectorExistsById(modelRelationship.Id))
                return;

            var diagramConnector = DiagramShapeFactory.CreateDiagramConnector(DiagramStore, modelRelationship);
            DiagramStore.AddConnector(diagramConnector);
        }

        public void HideModelRelationship(IModelRelationship modelRelationship)
        {
            var diagram = DiagramStore.CurrentDiagram;
            if (diagram.TryGetConnectorById(modelRelationship.Id, out IDiagramConnector diagramConnector))
                DiagramStore.RemoveConnector(diagramConnector);
        }

        public void ClearDiagram() => DiagramStore.ClearDiagram();
    }
}
