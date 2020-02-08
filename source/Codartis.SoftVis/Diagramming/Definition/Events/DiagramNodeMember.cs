namespace Codartis.SoftVis.Diagramming.Definition.Events
{
    public enum DiagramNodeMember
    {
        /// <summary>
        /// The absolute position on the canvas.
        /// </summary>
        AbsolutePosition,

        /// <summary>
        /// The position relative to the parent. If there's no parent then the same as the AbsolutePosition.
        /// </summary>
        RelativePosition,

        HeaderSize,

        ChildrenAreaSize,

        ModelNode
    }
}