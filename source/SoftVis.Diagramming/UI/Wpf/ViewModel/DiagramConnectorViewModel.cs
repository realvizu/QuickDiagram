using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Events;
using Codartis.SoftVis.Modeling;

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

        public DiagramNodeViewModelBase SourceNodeViewModel { get; }
        public DiagramNodeViewModelBase TargetNodeViewModel { get; }

        public DiagramConnectorViewModel(IReadOnlyModelStore modelStore, IReadOnlyDiagramStore diagramStore, 
            IDiagramConnector diagramConnector, DiagramNodeViewModelBase sourceNodeViewModel, DiagramNodeViewModelBase targetNodeViewModel)
            : base(modelStore, diagramStore, diagramConnector)
        {
            _routePoints = diagramConnector.Route.ToWpf();

            SourceNodeViewModel = sourceNodeViewModel;
            TargetNodeViewModel = targetNodeViewModel;

            DiagramStore.DiagramChanged += OnDiagramChanged;
        }


        public override void Dispose()
        {
            base.Dispose();

            DiagramStore.DiagramChanged -= OnDiagramChanged;
        }

        public IDiagramConnector DiagramConnector => (IDiagramConnector) DiagramShape;

        public object Clone()
        {
            return new DiagramConnectorViewModel(ModelStore, DiagramStore, DiagramConnector, SourceNodeViewModel, TargetNodeViewModel);
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

        private void OnDiagramChanged(DiagramEventBase diagramEvent)
        {
            if (diagramEvent is DiagramConnectorRouteChangedEvent diagramConnectorRouteChangedEvent
                && DiagramConnectorIdEqualityComparer.Instance.Equals(diagramConnectorRouteChangedEvent.NewConnector, DiagramConnector))
            {
                DiagramShape = diagramConnectorRouteChangedEvent.NewConnector;
                RoutePoints = diagramConnectorRouteChangedEvent.NewConnector.Route.ToWpf();
            }
        }
    }
}
