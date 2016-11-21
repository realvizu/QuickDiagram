namespace Codartis.SoftVis.Util.UI.Wpf.ViewModels
{
    /// <summary>
    /// View model for a window that shows the progress of a process and can be cancelled.
    /// </summary>
    public class ProgressWindowViewModel : ViewModelBase
    {
        private double _progressValue;
        private string _text;
        private string _title;

        public double ProgressValue
        {
            get { return _progressValue; }
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_progressValue != value)
                {
                    _progressValue = value;
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
