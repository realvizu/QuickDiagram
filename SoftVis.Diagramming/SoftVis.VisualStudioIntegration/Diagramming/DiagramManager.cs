using System;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Shapes;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering;
using Codartis.SoftVis.VisualStudioIntegration.Events;
using Codartis.SoftVis.VisualStudioIntegration.ImageExport;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Encapsulates a diagram, its builder and its presenter window.
    /// </summary>
    internal class DiagramManager : IDiagramServices
    {
        private readonly Diagram _diagram;
        private readonly RoslynBasedDiagramBuilder _diagramBuilder;
        private readonly DiagramToolWindow _diagramToolWindow;

        public event EventHandler PackageEvent;

        public DiagramManager(DiagramToolWindow diagramToolWindow)
        {
            _diagram = new WpfDiagram();
            _diagram.ShapeSelected += OnShapeSelected;
            _diagram.ShapeActivated += OnShapeActivated;

            _diagramBuilder = new RoslynBasedDiagramBuilder(_diagram);
            _diagramToolWindow = diagramToolWindow;
            _diagramToolWindow.Initialize(_diagram);
        }

        public int FontSize
        {
            get { return _diagramToolWindow.FontSize; }
            set { _diagramToolWindow.FontSize = value; }
        }

        public Dpi ImageExportDpi
        {
            get { return _diagramToolWindow.ImageExportDpi; }
            set { _diagramToolWindow.ImageExportDpi = value; }
        }

        public void ShowDiagram()
        {
            _diagramToolWindow.Show();
        }

        public void FitDiagramToView()
        {
            _diagramToolWindow.FitDiagramToView();
        }

        public void ClearDiagram()
        {
            _diagram.Clear();
        }

        public BitmapSource GetDiagramAsBitmap()
        {
            return _diagramToolWindow.GetDiagramAsBitmap();
        }

        public void ShowModelEntity(IModelEntity modelEntity)
        {
            _diagramBuilder.ShowModelEntity(modelEntity);
        }

        private void OnShapeSelected(object sender, DiagramShape diagramShape)
        {
            // TODO
        }

        private void OnShapeActivated(object sender, DiagramShape diagramShape)
        {
            if (diagramShape is DiagramNode)
            {
                var eventArgs = new DiagramNodeActivatedEventArgs((DiagramNode) diagramShape);
                PackageEvent?.Invoke(sender, eventArgs);
            }
        }
    }
}
