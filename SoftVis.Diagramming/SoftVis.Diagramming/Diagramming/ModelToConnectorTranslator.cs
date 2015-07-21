using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    class ModelToConnectorTranslator : UmlModelVisitorBase<DiagramConnectorType>
    {
        internal static DiagramConnector Translate(DiagramGraph graph, UmlRelationship relationship)
        {
            var sourceNode = graph.FindNode(relationship.SourceElement);
            var targetNode = graph.FindNode(relationship.TargetElement);

            var visitor = new ModelToConnectorTranslator();
            var connectorType = visitor.Visit(relationship);

            var connector = new DiagramConnector(relationship, sourceNode, targetNode, connectorType);
            return connector;
        }

        public override DiagramConnectorType Visit(UmlGeneralization item)
        {
            return DiagramConnectorType.Generalization;
        }
    }
}
