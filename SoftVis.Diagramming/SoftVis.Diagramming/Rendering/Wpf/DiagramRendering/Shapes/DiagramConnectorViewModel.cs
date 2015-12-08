using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Shapes
{
    /// <summary>
    /// A diagram connector that support data binding and can be used as a ViewModel from WPF controls.
    /// </summary>
    public class DiagramConnectorViewModel : DiagramConnector, INotifyPropertyChanged
    {
        private static readonly DoubleCollection DashPattern = new DoubleCollection(new[] { 5d, 5d });

        private readonly ConnectorType _connectorType;

        public event PropertyChangedEventHandler PropertyChanged;

        public DiagramConnectorViewModel(IModelRelationship relationship, DiagramNodeViewModel source, DiagramNodeViewModel target, 
            ConnectorType connectorType)
            : base(relationship, source, target)
        {
            _connectorType = connectorType;
        }

        public override Route RoutePoints
        {
            get { return base.RoutePoints; }
            set
            {
                if (RoutePoints != value)
                {
                    base.RoutePoints = value;
                    OnPropertyChanged();
                }
            }
        }

        public ArrowHeadType ArrowHeadType => _connectorType.ArrowHeadType;

        private bool IsDashed => _connectorType.ShaftLineType == LineType.Dashed;
        public DoubleCollection StrokeDashArray => IsDashed ? DashPattern : null;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
