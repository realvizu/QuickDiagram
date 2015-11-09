namespace Codartis.SoftVis.Diagramming.Layout.ActionTracking
{
    /// <summary>
    /// An action performed by the layout logic.
    /// </summary>
    public interface ILayoutAction
    {
        ILayoutAction CausingLayoutAction { get; }

        void AcceptVisitor(LayoutActionVisitorBase visitor);
    }
}