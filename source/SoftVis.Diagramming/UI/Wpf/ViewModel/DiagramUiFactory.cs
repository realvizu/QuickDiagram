using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Creates the main view model of the WPF UI.
    /// </summary>
    public class DiagramUiFactory : IDiagramUiFactory
    {
        public IDiagramUi Create(IReadOnlyModelStore modelStore, IReadOnlyDiagramStore diagramStore,
            IDiagramShapeUiFactory diagramShapeUiFactory, double minZoom, double maxZoom, double initialZoom)
        {
            return new DiagramViewModel(modelStore, diagramStore, diagramShapeUiFactory, minZoom, maxZoom, initialZoom);
        }
    }
}
