namespace Codartis.SoftVis.Diagramming.Layout
{
    /// <summary>
    /// A diagram action that desribes an action on a diagram shape.
    /// </summary>
    internal abstract class DiagramShapeAction : DiagramAction
    {
        public ShapeActionType ActionType { get; }

        protected DiagramShapeAction(ShapeActionType actionType)
        {
            ActionType = actionType;
        }
    }
}
