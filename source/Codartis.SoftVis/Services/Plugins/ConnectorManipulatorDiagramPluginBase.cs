using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.Services.Plugins
{
    public abstract class ConnectorManipulatorDiagramPluginBase : DiagramPluginBase
    {
        protected ConnectorManipulatorDiagramPluginBase(
            [NotNull] IModelService modelService,
            [NotNull] IDiagramService diagramService)
            : base(modelService, diagramService)
        {
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
                diagram.NodeExists(modelRelationship.Source) &&
                diagram.NodeExists(modelRelationship.Target) &&
                !DiagramConnectorWouldBeRedundant(modelRelationship, diagram);

            if (shouldShowModelRelationship)
                AddDiagramConnectorIfNotExists(modelRelationship, diagram);
        }

        protected static bool DiagramConnectorWouldBeRedundant(IModelRelationship modelRelationship, IDiagram diagram)
        {
            return diagram.PathExists(
                diagram.TryGetNode(modelRelationship.Source).Select(i => i.Id),
                diagram.TryGetNode(modelRelationship.Target).Select(i => i.Id));
        }

        protected void AddDiagramConnectorIfNotExists(IModelRelationship modelRelationship, IDiagram diagram)
        {
            if (diagram.ConnectorExists(modelRelationship.Id))
                return;

            DiagramService.AddConnector(modelRelationship.Id);
        }
    }
}