namespace Codartis.SoftVis.Diagramming.Layout.BaseActions
{
    /// <summary>
    /// Abstract base class for those classes that publish base layout action events.
    /// </summary>
    internal abstract class BaseLayoutActionEventSource : LayoutActionEventSource
    {
        protected LayoutAction RaiseLayoutAction(string action, double? amount = null,
            ILayoutAction causingAction = null)
        {
            var layoutAction = new LayoutAction(action, amount, causingAction);
            RaiseLayoutAction(layoutAction);
            return layoutAction;
        }

        protected ILayoutAction RaiseDiagramNodeLayoutAction(string action, DiagramNode diagramNode,
            ILayoutAction causingAction = null)
        {
            var layoutAction = new DiagramNodeLayoutAction(action, diagramNode, causingAction);
            RaiseLayoutAction(layoutAction);
            return layoutAction;
        }

        protected ILayoutAction RaiseDiagramConnectorLayoutAction(string action, DiagramConnector diagramConnector, 
            ILayoutAction causingAction = null)
        {
            var layoutAction = new DiagramConnectorAction(action, diagramConnector, causingAction);
            RaiseLayoutAction(layoutAction);
            return layoutAction;
        }
    }
}