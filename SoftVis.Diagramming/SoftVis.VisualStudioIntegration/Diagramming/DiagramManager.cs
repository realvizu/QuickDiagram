using System;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Events;
using Codartis.SoftVis.VisualStudioIntegration.ImageExport;
using Codartis.SoftVis.VisualStudioIntegration.UI;

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

        public DiagramManager(IModel model, DiagramToolWindow diagramToolWindow)
        {
            var connectorTypeResolver = new RoslynBasedConnectorTypeResolver();
            _diagram = new RoslynBasedWpfDiagram(connectorTypeResolver);
            _diagram.ShapeSelected += OnShapeSelected;
            _diagram.ShapeActivated += OnShapeActivated;

            _diagramBuilder = new RoslynBasedDiagramBuilder(_diagram);

            _diagramToolWindow = diagramToolWindow;
            _diagramToolWindow.Initialize(model, _diagram);
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

        public void GetDiagramImage(Action<BitmapSource> imageCreatedCallback)
        {
            _diagramToolWindow.GetDiagramImage(imageCreatedCallback);
        }

        public void ShowModelEntity(IModelEntity modelEntity)
        {
            _diagramBuilder.ShowModelEntity(modelEntity);
        }

        public void ShowModelEntityWithRelatedEntities(IModelEntity modelEntity)
        {
            _diagramBuilder.ShowModelEntityWithRelatedEntities(modelEntity);
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
