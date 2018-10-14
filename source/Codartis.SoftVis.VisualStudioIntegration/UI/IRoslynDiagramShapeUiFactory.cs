using Codartis.SoftVis.UI;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Extends the diagram shape UI element factory with Roslyn-specific operations.
    /// </summary>
    internal interface IRoslynDiagramShapeUiFactory : IDiagramShapeUiFactory
    {
        bool IsDescriptionVisible { get; set; }
    }
}
