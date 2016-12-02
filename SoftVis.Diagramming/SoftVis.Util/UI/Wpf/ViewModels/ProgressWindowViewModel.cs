namespace Codartis.SoftVis.Util.UI.Wpf.ViewModels
{
    /// <summary>
    /// View model for a window that shows the progress of a process.
    /// </summary>
    public class ProgressWindowViewModel : ViewModelBase
    {
        private string _text;
        private string _title;
        private int _progress;
        private int _maxProgress;
        private bool _showProgressNumber = true;
        private bool _isIndeterminate = true;

        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Progress
        {
            get { return _progress; }
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MaxProgress
        {
            get { return _maxProgress; }
            set
            {
                if (_maxProgress != value)
                {
                    _maxProgress = value;
                    OnPropertyChanged();

                    IsIndeterminate = _maxProgress == 0;
                }
            }
        }

        public bool ShowProgressNumber
        {
            get { return _showProgressNumber; }
            set
            {
                if (_showProgressNumber != value)
                {
                    _showProgressNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsIndeterminate
        {
            get { return _isIndeterminate; }
            set
            {
                if (_isIndeterminate != value)
                {
                    _isIndeterminate = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
