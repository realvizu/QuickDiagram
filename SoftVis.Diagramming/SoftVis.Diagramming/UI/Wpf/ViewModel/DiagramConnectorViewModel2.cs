using System.Linq;
using System.Windows;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Wpf.Common.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Defines the visible properties of diagram connectors.
    /// </summary>
    /// <remarks>
    /// Calculates the enclosing rectangle of the route points 
    /// and translate the route points so they are relative to the bounding rectangle.
    /// </remarks>
    public sealed class DiagramConnectorViewModel2 : DiagramShapeViewModelBase
    {
        private static readonly DoubleCollection DashPattern = new DoubleCollection(new[] { 5d, 5d });

        private readonly DiagramConnector _diagramConnector;
        private readonly ConnectorType _connectorType;
        private Point[] _routePoints;

        public DiagramConnectorViewModel2(IModel model, Diagram diagram,
            DiagramConnector diagramConnector, ConnectorType connectorType)
            : base(model, diagram)
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
            var rectRelativeTranslate = -(Vector)rect.TopLeft;
            RoutePoints = _diagramConnector.RoutePoints.Select(i => i.ToWpf() + rectRelativeTranslate).ToArray();
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
