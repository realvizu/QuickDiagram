using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;

namespace Codartis.SoftVis.Util.UI.Wpf.ViewModels
{
    /// <summary>
    /// Abstract base class for all ViewModels. Implements INotifyPropertyChanged.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private readonly Dispatcher _dispatcher;

        public event PropertyChangedEventHandler PropertyChanged;

        protected ViewModelBase()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void EnsureUiThread(Action action, DispatcherPriority dispatcherPriority = DispatcherPriority.Send)
        {
            if (Thread.CurrentThread == _dispatcher.Thread)
                action();
            else
                _dispatcher.Invoke(action, dispatcherPriority);
        }
    }
}
