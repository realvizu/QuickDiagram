using System.Linq;
using System.Windows;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.UI.Wpf.Common.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Defines the visible properties of diagram connectors.
    /// </summary>
    public sealed class DiagramConnectorViewModel2 : DiagramShapeViewModelBase
    {
        private static readonly DoubleCollection DashPattern = new DoubleCollection(new[] { 5d, 5d });

        private readonly DiagramConnector _diagramConnector;
        private readonly ConnectorType _connectorType;
        private Point[] _routePoints;

        public DiagramConnectorViewModel2(DiagramConnector diagramConnector, ConnectorType connectorType)
        {
            _diagramConnector = diagramConnector;
            _connectorType = connectorType;
            UpdateState();
        }

        public override DiagramShape DiagramShape => _diagramConnector;

        public override void UpdateState()
        {
            var rect = _diagramConnector.Rect.ToWpf();
            Position = rect.Location;
            Size = rect.Size;
            RoutePoints = _diagramConnector.RoutePoints.Select(j => j.ToWpf()).ToArray();
        }

        public ArrowHeadType ArrowHeadType => _connectorType.ArrowHeadType;
        private bool IsDashed => _connectorType.ShaftLineType == LineType.Dashed;
        public DoubleCollection StrokeDashArray => IsDashed ? DashPattern : null;

        public Point[] RoutePoints
        {
            get { return _routePoints; }
            set
            {
                if (_routePoints != value)
                {
                    _routePoints = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
