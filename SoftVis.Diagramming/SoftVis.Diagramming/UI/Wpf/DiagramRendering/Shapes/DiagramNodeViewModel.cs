using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.DiagramRendering.Shapes
{
    /// <summary>
    /// A diagram node that support data binding and can be used as a ViewModel from WPF controls.
    /// </summary>
    public class DiagramNodeViewModel : DiagramNode, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public DiagramNodeViewModel(IModelEntity modelEntity, Point2D position, Size2D size)
            : base(modelEntity, position, SizeAdjustedWithStereotype(modelEntity, size))
        {
        }

        public ModelEntityStereotype Stereotype => ModelEntity.Stereotype;
        public bool IsStereotypeVisible => Stereotype != null;
        public string StereotypeText => $"<<{Stereotype?.Name.ToLower()}>>";
        public FontStyle FontStyle => ModelEntity.IsAbstract ? FontStyles.Oblique : FontStyles.Normal;
        
        public override Point2D Position
        {
            get { return base.Position; }
            set
            {
                if (Position != value)
                {
                    base.Position = value;
                    OnPropertyChanged();
                }
            }
        }

        public override Size2D Size
        {
            get { return base.Size; }
            set
            {
                if (Size != value)
                {
                    base.Size = value;
                    OnPropertyChanged();
                }
            }
        }

        private static Size2D SizeAdjustedWithStereotype(IModelEntity modelEntity, Size2D size)
        {
            return modelEntity.Stereotype == null
                ? size
                : new Size2D(size.Width, size.Height + 10);
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
