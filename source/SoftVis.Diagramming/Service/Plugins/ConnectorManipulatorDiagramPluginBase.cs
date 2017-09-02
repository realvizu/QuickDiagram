using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Service.Plugins
{
    public class ConnectorManipulatorDiagramPluginBase : DiagramPluginBase
    {
        protected readonly IDiagramShapeFactory DiagramShapeFactory;

        public ConnectorManipulatorDiagramPluginBase(IDiagramShapeFactory diagramShapeFactory)
        {
            DiagramShapeFactory = diagramShapeFactory;
        }

        protected void ShowModelRelationshipsIfBothEndsAreVisible(IModelNode modelNode, IModel model, IDiagram diagram)
        {
            foreach (var modelRelationship in model.GetRelationships(modelNode))
                ShowModelRelationshipIfBothEndsAreVisible(modelRelationship, diagram);
        }

        protected void ShowModelRelationshipIfBothEndsAreVisible(IModelRelationship modelRelationship, IDiagram diagram)
        {
            var shouldShowModelRelationship =
                diagram.NodeExistsById(modelRelationship.Source.Id) &&
                diagram.NodeExistsById(modelRelationship.Target.Id) &&
                !DiagramConnectorWouldBeRedundant(modelRelationship, diagram);

            if (shouldShowModelRelationship)
                AddDiagramConnectorIfNotExists(modelRelationship, diagram);
        }

        protected static bool DiagramConnectorWouldBeRedundant(IModelRelationship modelRelationship, IDiagram diagram)
        {
            if (diagram.TryGetNodeById(modelRelationship.Source.Id, out IDiagramNode sourceNode) &&
                diagram.TryGetNodeById(modelRelationship.Target.Id, out IDiagramNode targetNode))
                return diagram.PathExistsById(sourceNode.Id, targetNode.Id);

            return false;
        }

        protected void AddDiagramConnectorIfNotExists(IModelRelationship modelRelationship, IDiagram diagram)
        {
            if (diagram.ConnectorExistsById(modelRelationship.Id))
                return;

            var newConnector = DiagramShapeFactory.CreateDiagramConnector(DiagramService, modelRelationship);
            DiagramService.AddConnector(newConnector);
        }
    }
}