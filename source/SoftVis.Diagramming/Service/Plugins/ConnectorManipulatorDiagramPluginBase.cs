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
            var sourceNode = diagram.GetNodeById(modelRelationship.Source.Id);
            var targetNode = diagram.GetNodeById(modelRelationship.Target.Id);
            return diagram.PathExists(sourceNode, targetNode);
        }

        protected void AddDiagramConnectorIfNotExists(IModelRelationship modelRelationship, IDiagram diagram)
        {
            var connector = diagram.GetConnectorById(modelRelationship.Id);
            if (connector == null)
            {
                var newConnector = DiagramShapeFactory.CreateDiagramConnector(modelRelationship);
                DiagramStore.AddConnector(newConnector);
            }
        }
    }
}