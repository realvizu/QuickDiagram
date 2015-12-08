using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ViewModels
{
    /// <summary>
    /// A diagram connector that support data binding and can be used as a ViewModel from WPF controls.
    /// </summary>
    public class DiagramConnectorViewModel : DiagramConnector, INotifyPropertyChanged
    {
        private static readonly DoubleCollection DashPattern = new DoubleCollection(new[] { 5d, 5d });

        private readonly ConnectorStyle _connectorStyle;

        public event PropertyChangedEventHandler PropertyChanged;

        public DiagramConnectorViewModel(IModelRelationship relationship,
            DiagramNodeViewModel source, DiagramNodeViewModel target, 
            IDiagramExtensionProvider extensionProvider)
            : base(relationship, source, target)
        {
            _connectorStyle = extensionProvider.GetConnectorStyle(relationship);
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

        public ArrowHeadType ArrowHeadType => _connectorStyle.ArrowHeadType;

        private bool IsDashed => _connectorStyle.ShaftLineType == LineType.Dashed;
        public DoubleCollection StrokeDashArray => IsDashed ? DashPattern : null;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
