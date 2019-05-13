namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// A diagram action that represents removing all diagram shapes.
    /// </summary>
    internal sealed class ClearDiagramAction : DiagramAction
    {
        public override void Accept(IDiagramActionVisitor visitor) => visitor.Visit(this);
    }
}