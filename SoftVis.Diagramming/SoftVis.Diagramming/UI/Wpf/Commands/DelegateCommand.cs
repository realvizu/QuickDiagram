using System;
using System.Windows.Input;

namespace Codartis.SoftVis.UI.Wpf.Commands
{
    /// <summary>
    /// A command that executes a delegate with no parameter.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Action _execute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// A command that executes a delegate with one typed parameter.
    /// </summary>
    public class DelegateCommand<T> : ICommand
    {
        private readonly Predicate<T> _canExecute;
        private readonly Action<T> _execute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T> execute, Predicate<T> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// A command that executes a delegate with two typed parameters.
    /// </summary>
    public class DelegateCommand<T1,T2> : ICommand
    {
        private readonly Func<T1, T2, bool> _canExecute;
        private readonly Action<T1, T2> _execute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T1, T2> execute, Func<T1, T2, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;

            var tuple = (Tuple<T1, T2>)parameter;
            return _canExecute(tuple.Item1, tuple.Item2);
        }

        public void Execute(object parameter)
        {
            var tuple = (Tuple<T1, T2>) parameter;
            _execute(tuple.Item1, tuple.Item2);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// A command that executes a delegate with two typed parameters.
    /// </summary>
    public class DelegateCommand<T1, T2, T3> : ICommand
    {
        private readonly Func<T1, T2, T3, bool> _canExecute;
        private readonly Action<T1, T2, T3> _execute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<T1, T2, T3> execute, Func<T1, T2, T3, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;

            var tuple = (Tuple<T1, T2, T3>)parameter;
            return _canExecute(tuple.Item1, tuple.Item2, tuple.Item3);
        }

        public void Execute(object parameter)
        {
            var tuple = (Tuple<T1, T2, T3>)parameter;
            _execute(tuple.Item1, tuple.Item2, tuple.Item3);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
