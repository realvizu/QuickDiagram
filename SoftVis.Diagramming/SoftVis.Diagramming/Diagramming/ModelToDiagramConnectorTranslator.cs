using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    class ModelToDiagramConnectorTranslator : UmlModelVisitorBase<DiagramConnectorType>
    {
        internal static DiagramConnector Translate(DiagramGraph graph, UmlRelationship relationship)
        {
            var sourceNode = graph.FindNode(relationship.SourceElement);
            var targetNode = graph.FindNode(relationship.TargetElement);

            var visitor = new ModelToDiagramConnectorTranslator();
            var connectorType = visitor.Visit(relationship);

            var connector = new DiagramConnector(relationship, sourceNode, targetNode, connectorType);
            return connector;
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
