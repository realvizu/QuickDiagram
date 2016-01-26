using System;
using System.Collections.ObjectModel;
using System.Windows;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.UI.Common;
using Codartis.SoftVis.UI.Extensibility;
using Codartis.SoftVis.UI.Wpf.Common.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Tracks the change events of a diagram and creates/modifies diagram shape viewmodels accordingly.
    /// </summary>
    public class DiagramViewportViewModel : ViewModelBase
    {
        private readonly Diagram _diagram;
        private readonly Map<DiagramShape, DiagramShapeViewModelBase> _diagramShapeToViewModelMap;
        private readonly DiagramShapeViewModelFactory _diagramShapeViewModelFactory;
        private readonly DiagramButtonCollectionViewModel _diagramButtonCollectionViewModel;
        private Rect _diagramContentRect;

        public ObservableCollection<DiagramShapeViewModelBase> DiagramShapeViewModels { get; }
        public ViewportViewModel ViewportViewModel { get; }

        public DiagramViewportViewModel(Diagram diagram, IDiagramBehaviourProvider diagramBehaviourProvider,
            double minZoom, double maxZoom, double initialZoom)
        {
            _diagram = diagram;
            _diagramShapeToViewModelMap = new Map<DiagramShape, DiagramShapeViewModelBase>();
            _diagramShapeViewModelFactory = new DiagramShapeViewModelFactory(diagram.ConnectorTypeResolver);
            _diagramButtonCollectionViewModel = new DiagramButtonCollectionViewModel(diagramBehaviourProvider);
            _diagramContentRect = Rect.Empty;

            DiagramShapeViewModels = new ObservableCollection<DiagramShapeViewModelBase>();
            ViewportViewModel = new ViewportViewModel(minZoom, maxZoom, initialZoom);

            diagram.ShapeAdded += OnShapeAdded;
            diagram.ShapeMoved += OnShapeMoved;
            diagram.ShapeRemoved += OnShapeRemoved;
            diagram.Cleared += OnDiagramCleared;

            AddDiagram(diagram);
        }

        public ObservableCollection<DiagramButtonViewModelBase> DiagramButtonViewModels
            => _diagramButtonCollectionViewModel.DiagramButtonViewModels;

        public void ZoomToContent() => ViewportViewModel.ZoomToContent(TransitionSpeed.Slow);

        //public void Resize(Size sizeInScreenSpace) => ViewportViewModel.Resize(sizeInScreenSpace);
        //public void ZoomTo(double newLinearZoom) => ViewportViewModel.ZoomTo(newLinearZoom);
        //public void Pan(Vector panVectorInScreenSpace) => ViewportViewModel.Pan(panVectorInScreenSpace);
        //public void ZoomWithCenterTo(double newLinearZoom, Point zoomCenterInScreenSpace)
        //    => ViewportViewModel.ZoomWithCenterTo(newLinearZoom, zoomCenterInScreenSpace);

        private void AddDiagram(Diagram diagram)
        {
            foreach (var diagramNode in diagram.Nodes)
                OnShapeAdded(null, diagramNode);

            foreach (var diagramConnector in diagram.Connectors)
                OnShapeAdded(null, diagramConnector);

            UpdateDiagramContentRect();
        }

        private void OnShapeAdded(object sender, DiagramShape diagramShape)
        {
            var diagramShapeViewModel = _diagramShapeViewModelFactory.CreateViewModel(diagramShape);
            diagramShapeViewModel.GotFocus += OnShapeFocused;
            diagramShapeViewModel.LostFocus += OnShapeUnfocused;
            diagramShapeViewModel.RemoveRequested += OnShapeRemoveRequested;

            DiagramShapeViewModels.Add(diagramShapeViewModel);
            _diagramShapeToViewModelMap.Set(diagramShape, diagramShapeViewModel);

            UpdateDiagramContentRect();
        }

        private void OnShapeMoved(object sender, DiagramShape diagramShape)
        {
            var diagramShapeViewModel = _diagramShapeToViewModelMap.Get(diagramShape);
            diagramShapeViewModel.UpdateState();

            UpdateDiagramContentRect();
        }

        private void OnShapeRemoved(object sender, DiagramShape diagramShape)
        {
            var diagramShapeViewModel = _diagramShapeToViewModelMap.Get(diagramShape);
            OnShapeUnfocused(diagramShapeViewModel);
            diagramShapeViewModel.GotFocus -= OnShapeFocused;
            diagramShapeViewModel.LostFocus -= OnShapeUnfocused;
            diagramShapeViewModel.RemoveRequested -= OnShapeRemoveRequested;

            DiagramShapeViewModels.Remove(diagramShapeViewModel);
            _diagramShapeToViewModelMap.Remove(diagramShape);

            UpdateDiagramContentRect();
        }

        private void OnDiagramCleared(object sender, EventArgs e)
        {
            DiagramShapeViewModels.Clear();
            _diagramShapeToViewModelMap.Clear();

            UpdateDiagramContentRect();
        }

        private void OnShapeRemoveRequested(DiagramShape diagramShape)
        {
            OnShapeRemoved(this, diagramShape);
        }

        private void OnShapeFocused(FocusableViewModelBase focusableViewModel)
        {
            var diagramShapeViewModel = focusableViewModel as DiagramShapeViewModelBase;
            if (diagramShapeViewModel == null)
                throw new ArgumentException("DiagramShapeViewModelBase expected");

           _diagramButtonCollectionViewModel.AssignButtonsTo(diagramShapeViewModel);
        }

        private void OnShapeUnfocused(FocusableViewModelBase focusableViewModel)
        {
            var diagramShapeViewModel = focusableViewModel as DiagramShapeViewModelBase;
            if (diagramShapeViewModel == null)
                throw new ArgumentException("DiagramShapeViewModelBase expected");

            if (_diagramButtonCollectionViewModel.AreButtonsAssignedTo(diagramShapeViewModel))
                _diagramButtonCollectionViewModel.HideButtons();
        }

        private void UpdateDiagramContentRect()
        {
            _diagramContentRect = _diagram.ContentRect.ToWpf();
            ViewportViewModel.UpdateContentRect(_diagramContentRect);
        }
    }
}
