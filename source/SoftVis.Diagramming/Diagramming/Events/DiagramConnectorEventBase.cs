namespace Codartis.SoftVis.Diagramming.Events
{
    public abstract class DiagramConnectorEventBase : DiagramEventBase
    {
        public IDiagramConnector DiagramConnector { get; }

        protected DiagramConnectorEventBase(IDiagram newDiagram, IDiagramConnector diagramConnector) 
            : base(newDiagram)
        {
            DiagramConnector = diagramConnector;
        }
    }
}