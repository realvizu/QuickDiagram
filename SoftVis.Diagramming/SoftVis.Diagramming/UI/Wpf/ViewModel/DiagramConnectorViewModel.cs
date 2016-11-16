using System.Linq;
using System.Windows;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Defines the visible properties of diagram connectors.
    /// </summary>
    public sealed class DiagramConnectorViewModel : DiagramShapeViewModelBase
    {
        private static readonly DoubleCollection DashPattern = new DoubleCollection(new[] { 5d, 5d });

        private readonly ConnectorType _connectorType;
        private Point[] _routePoints = new Point[0];

        public IDiagramConnector DiagramConnector { get; }
        public DiagramNodeViewModel SourceNodeViewModel { get; }
        public DiagramNodeViewModel TargetNodeViewModel { get; }

        public DiagramConnectorViewModel(IArrangedDiagram diagram, IDiagramConnector diagramConnector,
            DiagramNodeViewModel sourceNodeViewModel, DiagramNodeViewModel targetNodeViewModel)
            : base(diagram, diagramConnector)
        {
            DiagramConnector = diagramConnector;
            DiagramConnector.RouteChanged += OnRouteChanged;

            SourceNodeViewModel = sourceNodeViewModel;
            TargetNodeViewModel = targetNodeViewModel;

            _connectorType = Diagram.GetConnectorType(diagramConnector.Type);
        }

        public ArrowHeadType ArrowHeadType => _connectorType.ArrowHeadType;
        private bool IsDashed => _connectorType.ShaftLineType == LineType.Dashed;
        public DoubleCollection StrokeDashArray => IsDashed ? DashPattern : null;

        public Point[] RoutePoints
        {
            get { return _routePoints; }
            set
            {
                var nonNullValue = value ?? new Point[0];
                if (!_routePoints.SequenceEqual(nonNullValue))
                {
                    _routePoints = nonNullValue;
                    OnPropertyChanged();
                }
            }
        }

        private void OnRouteChanged(IDiagramConnector diagramConnector, Route oldRoute, Route newRoute)
        {
            RoutePoints = newRoute.Select(i => i.ToWpf()).ToArray();
        }
    }
}
