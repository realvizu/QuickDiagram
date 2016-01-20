using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.UI.Wpf.Common.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Tracks the change events of a diagram and creates/modifies diagram shape viewmodels accordingly.
    /// </summary>
    public class DiagramViewportViewModel : ViewModelBase
    {
        private readonly Diagram _diagram;
        private readonly DiagramShapeViewModelFactory _viewModelFactory;
        private readonly Map<DiagramShape, DiagramShapeViewModelBase> _diagramShapeToViewModelMap;

        private Rect _diagramContentRect;
        public ObservableCollection<DiagramShapeViewModelBase> DiagramShapeViewModels { get; }
        public ObservableCollection<MiniButtonViewModelBase> MiniButtonViewModels { get; }

        public DiagramViewportViewModel(Diagram diagram)
        {
            _diagram = diagram;
            _viewModelFactory = new DiagramShapeViewModelFactory(diagram.ConnectorTypeResolver);
            _diagramShapeToViewModelMap = new Map<DiagramShape, DiagramShapeViewModelBase>();

            _diagramContentRect = Rect.Empty;
            DiagramShapeViewModels = new ObservableCollection<DiagramShapeViewModelBase>();
            MiniButtonViewModels = new ObservableCollection<MiniButtonViewModelBase>();
            CreateMiniButtonViewModels();

            diagram.ShapeAdded += OnShapeAdded;
            diagram.ShapeMoved += OnShapeMoved;
            diagram.ShapeRemoved += OnShapeRemoved;
            diagram.Cleared += OnDiagramCleared;

            AddDiagram(diagram);
        }

        public Rect DiagramContentRect
        {
            get { return _diagramContentRect; }
            set
            {
                if (_diagramContentRect != value)
                {
                    _diagramContentRect = value;
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateDiagramContentRect()
        {
            DiagramContentRect = _diagram.ContentRect.ToWpf();
        }

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
            var diagramShapeViewModel = _viewModelFactory.CreateViewModel(diagramShape);
            diagramShapeViewModel.GotFocus += OnShapeFocused;
            diagramShapeViewModel.LostFocus += OnShapeUnfocused;

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

            DiagramShapeViewModels.Remove(diagramShapeViewModel);
            _diagramShapeToViewModelMap.Remove(diagramShape);

            UpdateDiagramContentRect();
        }

        private void OnShapeFocused(FocusableViewModelBase focusableViewModel)
        {
            var diagramShapeViewModel = focusableViewModel as DiagramShapeViewModelBase;
            if (diagramShapeViewModel==null)
                throw new ArgumentException($"DiagramShapeViewModelBase expected");

            AssignMiniButtonsTo(diagramShapeViewModel);
        }

        private void OnShapeUnfocused(FocusableViewModelBase focusableViewModel)
        {
            var diagramShapeViewModel = focusableViewModel as DiagramShapeViewModelBase;
            if (diagramShapeViewModel == null)
                throw new ArgumentException($"DiagramShapeViewModelBase expected");

            if (MiniButtonsAssignedTo(diagramShapeViewModel))
                HideMiniButtons();
        }

        private bool MiniButtonsAssignedTo(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            return MiniButtonViewModels.Any(i => i.AssociatedDiagramShapeViewModel == diagramShapeViewModel);
        }

        private void OnDiagramCleared(object sender, EventArgs e)
        {
            DiagramShapeViewModels.Clear();
            _diagramShapeToViewModelMap.Clear();

            UpdateDiagramContentRect();
        }

        private void CreateMiniButtonViewModels()
        {
            MiniButtonViewModels.Add(CreateCloseMiniButtonViewModel());
            MiniButtonViewModels.Add(CreateShowRelatedEntityMiniButtonViewModel());
        }

        private static ShowRelatedEntityMiniButtonViewModel CreateShowRelatedEntityMiniButtonViewModel()
        {
            return new ShowRelatedEntityMiniButtonViewModel(DiagramDefaults.MiniButtonRadius);
        }

        private static CloseMiniButtonViewModel CreateCloseMiniButtonViewModel()
        {
            return new CloseMiniButtonViewModel(DiagramDefaults.MiniButtonRadius);
        }

        private void AssignMiniButtonsTo(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            foreach (var miniButtonViewModel in MiniButtonViewModels)
                miniButtonViewModel.AssociateWith(diagramShapeViewModel);
        }

        private void HideMiniButtons()
        {
            foreach (var miniButtonViewModel in MiniButtonViewModels)
                miniButtonViewModel.Hide();
        }
    }
}
