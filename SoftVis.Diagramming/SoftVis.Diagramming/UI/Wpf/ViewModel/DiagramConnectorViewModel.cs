using System.Linq;
using System.Windows;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Util;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Defines the visible properties of diagram connectors.
    /// </summary>
    public sealed class DiagramConnectorViewModel : DiagramShapeViewModelBase
    {
        private static readonly DoubleCollection DashPattern = new DoubleCollection(new[] { 5d, 5d });

        private readonly IDiagramConnector _diagramConnector;
        private readonly DiagramNodeViewModel _sourceNode;
        private readonly DiagramNodeViewModel _targetNode;
        private readonly ConnectorType _connectorType;
        private Point[] _routePoints;

        public DiagramConnectorViewModel(IArrangedDiagram diagram, IDiagramConnector diagramConnector,
            DiagramNodeViewModel sourceNode, DiagramNodeViewModel targetNode)
            : base(diagram)
        {
            _diagramConnector = diagramConnector;
            _diagramConnector.RouteChanged += DiagramConnectorOnRouteChanged;

            _sourceNode = sourceNode;
            _targetNode = targetNode;
            _connectorType = Diagram.GetConnectorType(diagramConnector.Type);
        }

        public override IDiagramShape DiagramShape => _diagramConnector;
        public ArrowHeadType ArrowHeadType => _connectorType.ArrowHeadType;
        private bool IsDashed => _connectorType.ShaftLineType == LineType.Dashed;
        public DoubleCollection StrokeDashArray => IsDashed ? DashPattern : null;
        public DiagramNodeViewModel SourceNode => _sourceNode;
        public DiagramNodeViewModel TargetNode => _targetNode;

        public Point[] RoutePoints
        {
            get { return _routePoints; }
            set
            {
                if (!_routePoints.EmptyIfNullSequenceEqual(value))
                {
                    _routePoints = value;
                    OnPropertyChanged();
                }
            }
        }

        private void DiagramConnectorOnRouteChanged(IDiagramConnector diagramConnector, 
            Route oldRoute, Route newRoute)
        {
            RoutePoints = newRoute.Select(i => i.ToWpf()).ToArray();
        }
    }
}
