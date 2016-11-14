namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// Interface for double-dispatching diagram actions to type-specific operations.
    /// </summary>
    internal interface IDiagramActionVisitor
    {
        void Visit(DiagramNodeAction diagramNodeAction);
        void Visit(DiagramConnectorAction diagramConnectorAction);
    }
}
