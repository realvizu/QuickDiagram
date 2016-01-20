namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A minibutton for removing a shape from the diagram.
    /// </summary>
    internal class CloseMiniButtonViewModel : MiniButtonViewModelBase
    {
        public CloseMiniButtonViewModel(double miniButtonRadius)
            : base(miniButtonRadius)
        {
            IsEnabled = true;
        }
    }
}
