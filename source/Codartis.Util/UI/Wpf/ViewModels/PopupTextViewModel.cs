namespace Codartis.Util.UI.Wpf.ViewModels
{
    /// <summary>
    /// View model for a control that shows a text message.
    /// </summary>
    public class PopupTextViewModel : ShowHideViewModelBase
    {
        private string _text;

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }
    }
}
