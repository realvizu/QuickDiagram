using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Codartis.SoftVis.Util.UI.Wpf.Controls;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;

namespace Codartis.SoftVis.Util.UI.Wpf.Dialogs
{
    /// <summary>
    /// A modal popup window that shows the progress of a process while not blocking the execution thread.
    /// </summary>
    public class ProgressDialog : IDisposable
    {
        private readonly ProgressWindow _window;
        private readonly ProgressWindowViewModel _viewModel;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ProgressAccumulator _progressAccumulator;
        private readonly Progress<int> _maxProgress;

        public ProgressDialog(Window parentWindow, string title, string text, int maxProgress = 0)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _progressAccumulator = new ProgressAccumulator(i => _viewModel.Progress = i);
            _maxProgress = new Progress<int>(i => _viewModel.MaxProgress = i);

            _viewModel = new ProgressWindowViewModel
            {
                Title = title,
                Text = text,
                Progress = 0,
                MaxProgress = maxProgress,
                ShowProgressNumber = true,
            };

            _window = new ProgressWindow
            {
                DataContext = _viewModel,
                Owner = parentWindow
            };
            _window.Closed += WindowOnClosed;
        }

        public void Dispose()
        {
            Close();
            _cancellationTokenSource.Dispose();
        }

        public CancellationToken CancellationToken => _cancellationTokenSource.Token;
        public IIncrementalProgress Progress => _progressAccumulator;
        public IProgress<int> MaxProgress => _maxProgress;

        public bool ShowProgressNumber
        {
            get { return _viewModel.ShowProgressNumber; }
            set { _viewModel.ShowProgressNumber = value; }
        }

        public async Task ShowWithDelayAsync(int delayMillisec = 500)
        {
            try
            {
                await Task.Delay(delayMillisec, CancellationToken);

                CancellationToken.ThrowIfCancellationRequested();

                _window.ShowNonBlockingModal(CancellationToken);
            }
            catch (OperationCanceledException)
            {
            }
            catch (ObjectDisposedException)
            {
                // Happens when the CancellationToken was already disposed.
                // TODO: how to make sure it does not happen?
            }
        }

        private void Close()
        {
            _window.Close();
        }

        private void WindowOnClosed(object sender, EventArgs eventArgs)
        {
            _window.Closed -= WindowOnClosed;

            // The window might have been closed while in progress so a cancellation could be required.
            _cancellationTokenSource.Cancel();
        }

        public void Reset(string text, int maxProgress = 0, bool showProgressNumber = true)
        {
            _viewModel.Text = text;
            _viewModel.Progress = 0;
            _viewModel.MaxProgress = maxProgress;
            _viewModel.ShowProgressNumber = showProgressNumber;
            _progressAccumulator.Reset();
        }
    }
}