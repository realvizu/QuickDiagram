using System.ComponentModel;
using System.Runtime.CompilerServices;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Shapes;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ViewModels
{
    /// <summary>
    /// A diagram node that support data binding and can be used as a ViewModel from WPF controls.
    /// </summary>
    internal class DiagramNodeViewModel : DiagramNode, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DiagramNodeViewModel(IModelEntity modelEntity, DiagramPoint position, DiagramSize size)
            : base(modelEntity, position, SizeAdjustedWithStereotype(modelEntity, size))
        {
        }

        public bool IsStereotypeVisible => ModelEntity.Type != ModelEntityType.Class;
        public string StereotypeText => $"<<{ModelEntity.Type.ToString().ToLower()}>>";

        public override DiagramPoint Position
        {
            get { return base.Position; }
            set
            {
                if (Position != value)
                {
                    base.Position = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Rect));
                }
            }
        }

        public override DiagramSize Size
        {
            get { return base.Size; }
            set
            {
                if (Size != value)
                {
                    base.Size = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Rect));
                }
            }
        }

        private static DiagramSize SizeAdjustedWithStereotype(IModelEntity modelEntity, DiagramSize size)
        {
            return modelEntity.Type == ModelEntityType.Class
                ? size
                : new DiagramSize(size.Width, size.Height + 10);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
