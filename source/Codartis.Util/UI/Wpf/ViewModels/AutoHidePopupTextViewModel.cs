using System;
using System.Threading.Tasks;

namespace Codartis.Util.UI.Wpf.ViewModels
{
    /// <summary>
    /// View model for a control that shows a text message and automatically hides it after a certain TimeSpan.
    /// </summary>
    public class AutoHidePopupTextViewModel : PopupTextViewModel
    {
        private TimeSpan _autoHideAfter;
        private int _autoHideInProgressCount;

        public TimeSpan AutoHideAfter
        {
            get { return _autoHideAfter; }
            set
            {
                _autoHideAfter = value;
                OnPropertyChanged();
            }
        }

        public override void Show()
        {
            base.Show();

            if (_autoHideAfter == default(TimeSpan) || _autoHideAfter <= TimeSpan.Zero)
                return;

            SetUpAutoHideAsync();
        }

        private async void SetUpAutoHideAsync()
        {
            // No need to synchronize access to this counter because all code that accesses it run on the UI thread.
            _autoHideInProgressCount++;

            await Task.Delay(_autoHideAfter);

            _autoHideInProgressCount--;
            if (_autoHideInProgressCount == 0)
                Hide();
        }
    }
}
