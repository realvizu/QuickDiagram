namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    /// <summary>
    /// A visitor for relative layout actions.
    /// </summary>
    internal abstract class RelativeLayoutActionVisitorBase : LayoutActionVisitorBase
    {
        public virtual void Visit(RelativeLocationAssignedLayoutAction layoutAction) { }
        public virtual void Visit(RelativeLocationChangedLayoutAction layoutAction) { }
    }
}
