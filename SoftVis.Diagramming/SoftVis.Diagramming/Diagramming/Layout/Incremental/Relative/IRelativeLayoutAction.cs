namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative
{
    internal interface IRelativeLayoutAction : ILayoutAction
    {
        void AcceptVisitor(RelativeLayoutActionVisitorBase visitor);
    }
}
