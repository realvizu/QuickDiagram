namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// Describes a batch diagram action.
    /// </summary>
    internal class DiagramBatchAction : DiagramAction
    {
        public BatchActionType Type { get; }

        public DiagramBatchAction(BatchActionType type)
        {
            Type = type;
        }

        public override void Accept(IDiagramActionVisitor visitor) => visitor.Visit(this);
    }
}
