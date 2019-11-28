using System.Linq;
using System.Windows;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;

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

        public DiagramConnectorViewModel(
            IModelEventSource modelEventSource,
            IDiagramEventSource diagramEventSource,
            [NotNull] IDiagramConnector diagramConnector)
            : base(modelEventSource, diagramEventSource, diagramConnector)
        {
            UpdateDiagramConnector(diagramConnector);
        }

        public override string StereotypeName => DiagramConnector.ModelRelationship.Stereotype.Name;

        public IDiagramConnector DiagramConnector => (IDiagramConnector)DiagramShape;

        public void Update([NotNull] IDiagramConnector connector)
        {
            UpdateDiagramShape(connector);
            UpdateDiagramConnector(connector);
        }

        private void UpdateDiagramConnector([NotNull] IDiagramConnector connector)
        {
            RoutePoints = connector.Route.ToWpf();
        }

        public override IDiagramShapeUi CloneForImageExport() => new DiagramConnectorViewModel(ModelEventSource, DiagramEventSource, DiagramConnector);

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
                if (_routePoints?.SequenceEqual(nonNullValue) != true)
                {
                    _routePoints = nonNullValue;
                    OnPropertyChanged();
                }
            }
        }
    }
}