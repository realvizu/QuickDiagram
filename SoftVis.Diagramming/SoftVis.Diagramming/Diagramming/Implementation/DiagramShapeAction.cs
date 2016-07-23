namespace Codartis.SoftVis.Diagramming.Implementation
{
    /// <summary>
    /// A diagram action that desribes the addition or removal of a diagram shape.
    /// </summary>
    internal abstract class DiagramShapeAction
    {
        public ShapeActionType ActionType { get; }

        protected DiagramShapeAction(ShapeActionType actionType)
        {
            ActionType = actionType;
        }
    }
}
