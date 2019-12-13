using Codartis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// The default implementation of a diagram node header view model just contains the model node's payload.
    /// </summary>
    public class DiagramNodeHeaderViewModel : ViewModelBase, IDiagramNodeHeaderUi
    {
        private object _payload;

        public object Payload
        {
            get { return _payload; }
            set
            {
                if (_payload != value)
                {
                    _payload = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}