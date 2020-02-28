namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Tracks focus and assigns minibuttons to the focused diagram shape.
    /// </summary>
    public interface IMiniButtonManager
    {
        void Focus(IDiagramShapeUi diagramShapeUi);
        void Unfocus(IDiagramShapeUi diagramShapeUi);
        void UnfocusAll();
        void PinFocus();
        void UnpinFocus();
    }
}