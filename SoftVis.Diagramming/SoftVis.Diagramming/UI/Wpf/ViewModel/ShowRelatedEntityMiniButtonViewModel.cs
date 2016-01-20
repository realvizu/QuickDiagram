using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A minibutton to choose related entities.
    /// </summary>
    internal class ShowRelatedEntityMiniButtonViewModel : MiniButtonViewModelBase
    {
        public ShowRelatedEntityMiniButtonViewModel(double miniButtonRadius)
            : base(miniButtonRadius)
        {
        }

        public ConnectorType ConnectorType => new ConnectorType(ArrowHeadType.Hollow, LineType.Solid);
    }
}
