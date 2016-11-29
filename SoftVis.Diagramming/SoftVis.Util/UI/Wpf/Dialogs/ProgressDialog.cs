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

        public ProgressDialog(Window parentWindow, string text, string title = null,
            ProgressMode progressMode = ProgressMode.Percentage)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _viewModel = new ProgressWindowViewModel
            {
                Title = title,
                Text = text,
                ProgressPercentage = 0,
                ProgressCount = 0,
                ProgressMode = progressMode
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
        public void SetText(string text) => _viewModel.Text = text;

        public void SetProgressPercentage(double progress)
        {
            AssertProgressMode(ProgressMode.Percentage);
            _viewModel.ProgressPercentage = progress;
        }

        public void ResetProgressCount(int i = 0)
        {
            AssertProgressMode(ProgressMode.Count);
            _viewModel.ProgressCount = i;
        }

        public void AddProgressCount(int i = 1)
        {
            AssertProgressMode(ProgressMode.Count);
            _viewModel.ProgressCount += i;
        }

        private void WindowOnClosed(object sender, EventArgs eventArgs)
        {
            _window.Closed -= WindowOnClosed;

            // The window might have been closed while in progress so a cancellation could be required.
            _cancellationTokenSource.Cancel();

            _cancellationTokenSource.Dispose();
        }

        private void AssertProgressMode(ProgressMode progressMode)
        {
            if (_viewModel.ProgressMode != progressMode)
                throw new InvalidOperationException($"The ProgressDialog must be in {progressMode} mode to use this operation.");
        }
    }
}