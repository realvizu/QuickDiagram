using System.ComponentModel;
using System.Runtime.CompilerServices;
using Codartis.SoftVis.Common;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Shapes;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ViewModels
{
    /// <summary>
    /// A diagram connector that support data binding and can be used as a ViewModel from WPF controls.
    /// </summary>
    public class DiagramConnectorViewModel : DiagramConnector, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DiagramConnectorViewModel(IModelRelationship relationship, DiagramNodeViewModel source, DiagramNodeViewModel target)
            : base(relationship, source, target)
        {
        }

        public override DiagramPoint[] RoutePoints
        {
            get { return base.RoutePoints; }
            set
            {
                if (!RoutePoints.NullableSequenceEqual(value))
                {
                    base.RoutePoints = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
