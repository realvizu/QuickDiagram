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

        private Point[] _routePoints;

        public IDiagramConnector DiagramConnector { get; }
        public DiagramNodeViewModelBase SourceNodeViewModel { get; }
        public DiagramNodeViewModelBase TargetNodeViewModel { get; }

        public DiagramConnectorViewModel(IArrangedDiagram diagram, IDiagramConnector diagramConnector,
            DiagramNodeViewModelBase sourceNodeViewModel, DiagramNodeViewModelBase targetNodeViewModel)
            : base(diagram, diagramConnector)
        {
            DiagramConnector = diagramConnector;
            SourceNodeViewModel = sourceNodeViewModel;
            TargetNodeViewModel = targetNodeViewModel;

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

        private ConnectorType ConnectorType => DiagramConnector.ConnectorType;
        private bool IsDashed => ConnectorType.ShaftLineType == LineType.Dashed;
        public ArrowHeadType ArrowHeadType => ConnectorType.ArrowHeadType;
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
