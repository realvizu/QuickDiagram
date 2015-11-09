using Codartis.SoftVis.Diagramming.Layout.ActionTracking;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.ActionTracking
{
    internal interface IRelativeLayoutAction : ILayoutAction
    {
        void AcceptVisitor(RelativeLayoutActionVisitorBase visitor);
    }
}
