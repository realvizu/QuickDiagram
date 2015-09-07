using System.ComponentModel;
using System.Runtime.CompilerServices;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ViewModels
{
    internal class BindableClassNode : ClassNode, INotifyPropertyChanged
    {
        public BindableClassNode(UmlModelElement modelElement, string name, DiagramPoint position, DiagramSize size)
            : base(modelElement, name, position, size)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

        public DiagramRect Rect => new DiagramRect(Position, Size);
    }
}
