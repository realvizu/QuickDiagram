using System;
using System.Threading;
using System.Windows;
using Codartis.SoftVis.Util.UI.Wpf.Controls;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.Util.UI.Wpf.Dialogs
{
    /// <summary>
    /// A modal popup window that shows the progress of a process while not blocking the execution thread.
    /// Has a cancellation token that signals if the dialog is closed before the progress reaches completion.
    /// </summary>
    public class ProgressDialog
    {
        private readonly ProgressWindow _window;
        private readonly ProgressWindowViewModel _viewModel;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public ProgressDialog(Window parentWindow, string text, string title = null)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _viewModel = new ProgressWindowViewModel
            {
                Title = title,
                Text = text,
                ProgressValue = 0
            };

            _window = new ProgressWindow
            {
                DataContext = _viewModel,
                Owner = parentWindow
            };
            _window.Closed += WindowOnClosed;
        }

        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        public void Show() => _window.ShowNonBlockingModal();
        public void Close() => _window.Close();
        public void SetProgress(double progress) => _viewModel.ProgressValue = progress;
        public void SetText(string text) => _viewModel.Text = text;

        private void WindowOnClosed(object sender, EventArgs eventArgs)
        {
            _window.Closed -= WindowOnClosed;

            if (_viewModel.ProgressValue < 1)
                _cancellationTokenSource.Cancel();

            _cancellationTokenSource.Dispose();
        }
    }
}