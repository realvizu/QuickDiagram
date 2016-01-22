using Codartis.SoftVis.UI.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A minibutton for removing a shape from the diagram.
    /// </summary>
    internal class CloseShapeButtonViewModel : DiagramButtonViewModelBase
    {
        public CloseShapeButtonViewModel(double miniButtonRadius, RectRelativeLocation miniButtonLocation)
            : base(miniButtonRadius, miniButtonLocation)
        {
            IsEnabled = true;
        }
    }
}
