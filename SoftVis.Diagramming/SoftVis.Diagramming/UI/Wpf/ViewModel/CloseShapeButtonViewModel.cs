using Codartis.SoftVis.UI.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A button for removing a shape from the diagram.
    /// </summary>
    internal class CloseShapeButtonViewModel : DiagramButtonViewModelBase
    {
        public CloseShapeButtonViewModel(double buttonRadius, RectRelativeLocation buttonLocation)
            : base(buttonRadius, buttonLocation, i => i.Remove())
        {
            IsEnabled = true;
        }
    }
}
