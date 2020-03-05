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

        /// <summary>
        /// The size of the whole node.
        /// </summary>
        Size,

        /// <summary>
        /// The children area's top left corner's position relative to the diagram node's top left corner.
        /// </summary>
        ChildrenAreaTopLeft,

        /// <summary>
        /// The size of the children area.
        /// </summary>
        ChildrenAreaSize,

        ModelNode,
        ParentNode
    }
}