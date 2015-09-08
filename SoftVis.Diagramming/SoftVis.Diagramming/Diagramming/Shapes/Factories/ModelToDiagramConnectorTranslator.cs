using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming.Shapes.Factories
{
    /// <summary>
    /// Translates a model relationship into a diagram connector.
    /// </summary>
    internal class ModelToDiagramConnectorTranslator : UmlModelVisitorBase<DiagramConnectorType>
    {
        internal static DiagramConnector Translate(UmlRelationship relationship, DiagramNode sourceNode, DiagramNode targetNode)
        {
            var visitor = new ModelToDiagramConnectorTranslator();
            var connectorType = visitor.Visit(relationship);
            return new DiagramConnector(relationship, sourceNode, targetNode, connectorType);
        }

        public override DiagramConnectorType Visit(UmlGeneralization item)
        {
            return DiagramConnectorType.Generalization;
        }

        public override DiagramConnectorType Visit(UmlDependency item)
        {
            return DiagramConnectorType.Dependency;
        }
    }
}
