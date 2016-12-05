using System;
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
    public sealed class DiagramConnectorViewModel : DiagramShapeViewModelBase, ICloneable, IDisposable
    {
        // This member cannot be static because it will be bound to UI elements created on different threads.
        private readonly DoubleCollection _dashPattern = new DoubleCollection(new[] { 5d, 5d });

        private readonly ConnectorType _connectorType;
        private Point[] _routePoints;

        public IDiagramConnector DiagramConnector { get; }
        public DiagramNodeViewModel SourceNodeViewModel { get; }
        public DiagramNodeViewModel TargetNodeViewModel { get; }

        public DiagramConnectorViewModel(IArrangedDiagram diagram, IDiagramConnector diagramConnector,
            DiagramNodeViewModel sourceNodeViewModel, DiagramNodeViewModel targetNodeViewModel)
            : base(diagram, diagramConnector)
        {
            DiagramConnector = diagramConnector;
            SourceNodeViewModel = sourceNodeViewModel;
            TargetNodeViewModel = targetNodeViewModel;

            _connectorType = Diagram.GetConnectorType(diagramConnector.Type);
            _routePoints = RouteToWpf(diagramConnector.RoutePoints);

            DiagramConnector.RouteChanged += OnRouteChanged;
        }

        public void Dispose()
        {
            DiagramConnector.RouteChanged -= OnRouteChanged;
        }

        public object Clone()
        {
            return new DiagramConnectorViewModel(Diagram, DiagramConnector, SourceNodeViewModel, TargetNodeViewModel)
            {
                _routePoints = _routePoints,
            };
        }

        public ArrowHeadType ArrowHeadType => _connectorType.ArrowHeadType;
        private bool IsDashed => _connectorType.ShaftLineType == LineType.Dashed;
        public DoubleCollection StrokeDashArray => IsDashed ? _dashPattern : null;

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
            RoutePoints = RouteToWpf(newRoute);
        }

        private static Point[] RouteToWpf(Route newRoute)
        {
            return newRoute.EmptyIfNull().Select(i => i.ToWpf()).ToArray();
        }
    }
}
