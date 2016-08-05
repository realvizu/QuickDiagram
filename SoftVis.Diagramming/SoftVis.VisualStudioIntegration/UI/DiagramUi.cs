using System;
using System.Reflection;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Wpf.View;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util.UI.Wpf;
using Codartis.SoftVis.VisualStudioIntegration.ImageExport;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Provides diagram UI services. Bundles the diagram control and its view model together.
    /// </summary>
    public sealed class DiagramUi : IUiServices
    {
        private const string DiagramStylesXaml = "UI/DiagramStyles.xaml";

        private readonly DiagramControl _diagramControl;
        private readonly DiagramViewModel _diagramViewModel;

        public Dpi ImageExportDpi { get; set; }

        public DiagramUi(IReadOnlyModel model, IDiagram diagram)
        {
            ImageExportDpi = Dpi.Default;
            
            var diagramBehaviourProvider = new CustomDiagramBehaviourProvider();
            _diagramViewModel = new DiagramViewModel(model, diagram, diagramBehaviourProvider, minZoom: .1, maxZoom: 10, initialZoom: 1);

            var resourceDictionary = ResourceHelpers.GetResourceDictionary(DiagramStylesXaml, Assembly.GetExecutingAssembly());
            _diagramControl = new DiagramControl(resourceDictionary) {DataContext = _diagramViewModel};
        }

        public object ContentControl => _diagramControl;

        public int FontSize
        {
            get { return (int)_diagramControl.FontSize; }
            set { _diagramControl.FontSize = value; }
        }

        public void FitDiagramToView() 
            => _diagramViewModel.ZoomToContent();

        public void GetDiagramImage(Action<BitmapSource> imageCreatedCallback) 
            => _diagramViewModel.GetDiagramImage(ImageExportDpi.Value, imageCreatedCallback);
    }
}
