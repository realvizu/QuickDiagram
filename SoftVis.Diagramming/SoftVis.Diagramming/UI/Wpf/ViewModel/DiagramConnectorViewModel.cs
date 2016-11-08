using System.Linq;
using System.Windows;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Defines the visible properties of diagram connectors.
    /// </summary>
    /// <remarks>
    /// Calculates the enclosing rectangle of the route points 
    /// and translate the route points so they are relative to the bounding rectangle.
    /// </remarks>
    public sealed class DiagramConnectorViewModel : DiagramShapeViewModelBase
    {
        private static readonly DoubleCollection DashPattern = new DoubleCollection(new[] { 5d, 5d });

        private readonly IDiagramConnector _diagramConnector;
        private readonly ConnectorType _connectorType;
        private Point[] _routePoints;

        public DiagramConnectorViewModel(IArrangedDiagram diagram, IDiagramConnector diagramConnector)
            : base(diagram)
        {
            _diagramConnector = diagramConnector;
            _connectorType = Diagram.GetConnectorType(diagramConnector.Type);

            //_diagramConnector.RouteChanged += (i,j,k) => UpdateLayoutProperties();
            //_diagramConnector.Source.TopLeftChanged += (i, j, k) => UpdateLayoutProperties();
            //_diagramConnector.Source.SizeChanged += (i, j, k) => UpdateLayoutProperties();
            //_diagramConnector.Target.TopLeftChanged += (i, j, k) => UpdateLayoutProperties();
            //_diagramConnector.Target.SizeChanged += (i, j, k) => UpdateLayoutProperties();

            UpdateLayoutProperties();
        }

        public override IDiagramShape DiagramShape => _diagramConnector;
        public ArrowHeadType ArrowHeadType => _connectorType.ArrowHeadType;
        private bool IsDashed => _connectorType.ShaftLineType == LineType.Dashed;
        public DoubleCollection StrokeDashArray => IsDashed ? DashPattern : null;

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

        private void UpdateLayoutProperties()
        {
            var rect = _diagramConnector.Rect.ToWpf();
            TopLeft = rect.Location;
            Size = rect.Size;
            var rectRelativeTranslate = -(Vector)rect.TopLeft;
            var routePoints = _diagramConnector.RoutePoints?.Select(i => i.ToWpf() + rectRelativeTranslate).ToArray();
            RoutePoints = routePoints == null || routePoints.Any(i => i.IsUndefined()) ? null : routePoints;
        }
    }
}
