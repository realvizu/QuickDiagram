namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    /// <summary>
    /// A layout action of the relative layout phase.
    /// </summary>
    internal interface IRelativeLayoutAction : ILayoutAction
    {
        void AcceptVisitor(RelativeLayoutActionVisitorBase visitor);
    }
}
