namespace Codartis.SoftVis.Util.UI.Wpf.ViewModels
{
    /// <summary>
    /// View model for a window that shows the progress of a process and can be cancelled.
    /// </summary>
    public class ProgressWindowViewModel : ViewModelBase
    {
        private ProgressMode _progressMode;
        private double _progressPercentage;
        private int _progressCount;
        private string _text;
        private string _title;

        public ProgressMode ProgressMode
        {
            get { return _progressMode; }
            set
            {
                if (_progressMode != value)
                {
                    _progressMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public double ProgressPercentage
        {
            get { return _progressPercentage; }
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_progressPercentage != value)
                {
                    _progressPercentage = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ProgressCount
        {
            get { return _progressCount; }
            set
            {
                if (_progressCount != value)
                {
                    _progressCount = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text= value;
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
    }
}
