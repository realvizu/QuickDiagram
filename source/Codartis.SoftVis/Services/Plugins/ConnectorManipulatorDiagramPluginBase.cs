using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.Util;

namespace Codartis.SoftVis.Services.Plugins
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
            foreach (var modelRelationship in model.GetRelationships(modelNode.Id))
                ShowModelRelationshipIfBothEndsAreVisible(modelRelationship, diagram);
        }

        protected void ShowModelRelationshipIfBothEndsAreVisible(IModelRelationship modelRelationship, IDiagram diagram)
        {
            var shouldShowModelRelationship =
                modelRelationship.Stereotype != ModelRelationshipStereotype.Containment &&
                diagram.NodeExists(modelRelationship.Source.Id) &&
                diagram.NodeExists(modelRelationship.Target.Id) &&
                !DiagramConnectorWouldBeRedundant(modelRelationship, diagram);

            if (shouldShowModelRelationship)
                AddDiagramConnectorIfNotExists(modelRelationship, diagram);
        }

        protected static bool DiagramConnectorWouldBeRedundant(IModelRelationship modelRelationship, IDiagram diagram)
        {
            return diagram.PathExists(
                diagram.TryGetNode(modelRelationship.Source.Id).Select(i => i.Id),
                diagram.TryGetNode(modelRelationship.Target.Id).Select(i => i.Id));
        }

        protected void AddDiagramConnectorIfNotExists(IModelRelationship modelRelationship, IDiagram diagram)
        {
            if (diagram.ConnectorExists(modelRelationship.Id))
                return;

            DiagramService.ShowModelRelationship(modelRelationship);
        }
    }
}