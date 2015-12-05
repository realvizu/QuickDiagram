using System.Windows;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ViewModels;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.DiagramRendering
{
    /// <summary>
    /// A diagram node viewmodel extended with roslyn-provided info.
    /// </summary>
    internal class RoslynBasedDiagramNodeViewModel : DiagramNodeViewModel
    {
        public RoslynBasedDiagramNodeViewModel(IModelEntity modelEntity, Point2D position, Size2D size)
            : base(modelEntity, position, size)
        {
            FontStyle = modelEntity is RoslynBasedClass && ((RoslynBasedClass) modelEntity).IsAbstract
                ? FontStyles.Oblique
                : FontStyles.Normal;
        }

        private FontStyle _fontStyle;

        public FontStyle FontStyle
        {
            get { return _fontStyle; }
            set
            {
                if (_fontStyle != value)
                {
                    _fontStyle = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
