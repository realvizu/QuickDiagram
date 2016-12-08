namespace Codartis.SoftVis.Util.UI.Wpf.ViewModels
{
    /// <summary>
    /// Designates a view model that can be added as decorator of another (host) view model.
    /// </summary>
    /// <typeparam name="THostViewModel">The type of the view model that hosts the decorators.</typeparam>
    public interface IDecoratorViewModel<in THostViewModel>
        where THostViewModel : ViewModelBase
    {
        void AssociateWith(THostViewModel hostViewModel);
        void Hide();
    }
}
