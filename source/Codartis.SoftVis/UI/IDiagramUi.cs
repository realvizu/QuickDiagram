namespace Codartis.SoftVis.UI
{
    /// <summary>
    /// Displays the diagram and handles user interactions.
    /// </summary>
    public interface IDiagramUi
    {
        IDiagramViewportUi Viewport { get; }
    }
}
