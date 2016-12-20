namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// Interface for double-dispatching diagram actions to type-specific operations.
    /// </summary>
    internal interface IDiagramActionVisitor
    {
        void Visit(AddDiagramNodeAction action);
        void Visit(RemoveDiagramNodeAction action);
        void Visit(ResizeDiagramNodeAction action);
        void Visit(AddDiagramConnectorAction action);
        void Visit(RemoveDiagramConnectorAction action);
    }
}
