using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Codartis.SoftVis.Util.UI.Wpf.ViewModels
{
    /// <summary>
    /// Abstract base class for all ViewModels. Implements INotifyPropertyChanged.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
