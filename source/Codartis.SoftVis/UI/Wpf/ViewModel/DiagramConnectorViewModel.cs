using System.Linq;
using System.Windows;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Diagramming.Definition.Events;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Defines the visible properties of diagram connectors.
    /// </summary>
    public sealed class DiagramConnectorViewModel : DiagramShapeViewModelBase, IDiagramConnectorUi
    {
        // This member cannot be static because it will be bound to UI elements created on different threads.
        private readonly DoubleCollection _dashPattern = new DoubleCollection(new[] { 5d, 5d });

        private Point[] _routePoints;

        public DiagramNodeViewModel SourceNodeViewModel { get; }
        public DiagramNodeViewModel TargetNodeViewModel { get; }

        public DiagramConnectorViewModel(IModelService modelService, IDiagramService diagramService, 
            IDiagramConnector diagramConnector, DiagramNodeViewModel sourceNodeViewModel, DiagramNodeViewModel targetNodeViewModel)
            : base(modelService, diagramService, diagramConnector)
        {
            _routePoints = diagramConnector.Route.ToWpf();

            SourceNodeViewModel = sourceNodeViewModel;
            TargetNodeViewModel = targetNodeViewModel;

            DiagramService.DiagramChanged += OnDiagramChanged;
        }

        public override string Stereotype => DiagramConnector.ModelRelationship.Stereotype.Name;

        public override void Dispose()
        {
            base.Dispose();

            DiagramService.DiagramChanged -= OnDiagramChanged;
        }

        public IDiagramConnector DiagramConnector => (IDiagramConnector) DiagramShape;

        public override object CloneForImageExport() 
            => new DiagramConnectorViewModel(ModelService, DiagramService, DiagramConnector, SourceNodeViewModel, TargetNodeViewModel);

        private ConnectorType ConnectorType => DiagramConnector.ConnectorType;
        private bool IsDashed => ConnectorType.ShaftLineType == LineType.Dashed;
        public ArrowHeadType ArrowHeadType => ConnectorType.ArrowHeadType;
        public DoubleCollection StrokeDashArray => IsDashed ? _dashPattern : null;

        public Point[] RoutePoints
        {
            get { return _routePoints; }
            private set
            {
                var nonNullValue = value ?? new Point[0];
                if (!_routePoints.SequenceEqual(nonNullValue))
                {
                    _routePoints = nonNullValue;
                    OnPropertyChanged();
                }
            }
        }

        private void OnDiagramChanged(DiagramEvent @event)
        {
            foreach (var change in @event.ShapeEvents)
                ProcessDiagramChange(change);
        }

        private void ProcessDiagramChange(DiagramShapeEventBase diagramShapeEvent)
        {
            if (diagramShapeEvent is DiagramConnectorRouteChangedEvent diagramConnectorRouteChangedEvent &&
                DiagramConnectorIdEqualityComparer.Instance.Equals(diagramConnectorRouteChangedEvent.NewConnector, DiagramConnector))
            {
                DiagramShape = diagramConnectorRouteChangedEvent.NewConnector;
                RoutePoints = diagramConnectorRouteChangedEvent.NewConnector.Route.ToWpf();
            }
        }
    }
}
