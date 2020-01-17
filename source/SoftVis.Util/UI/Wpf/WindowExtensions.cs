using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

namespace Codartis.SoftVis.Util.UI.Wpf
{
    public static class WindowExtensions
    {
        [DllImport("user32.dll")]
        private static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

        /// <summary>
        /// Shows a window as modal (owner is disabled) but does not block the thread.
        /// </summary>
        /// <param name="window">The window to be shown as non-blocking modal.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public static void ShowNonBlockingModal(this Window window, CancellationToken cancellationToken = default)
        {
            lock (window)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                window.Owner.Disable();

                window.Closing += OnClosing;
                window.Show();
            }
        }

        private static void OnClosing(object sender, CancelEventArgs e)
        {
            var window = (Window)sender;
            lock (window)
            {
                window.Closing -= OnClosing;

                window.Owner.Enable();
                window.Owner.Activate();
            }
        }

        public static void Enable(this Window window) => window.SetEnableState(true);
        public static void Disable(this Window window) => window.SetEnableState(false);

        public static void SetEnableState(this Window window, bool enable)
        {
            EnableWindow(window.GetHandle(), enable);
        }

        public static IntPtr GetHandle(this Window window)
        {
            return new WindowInteropHelper(window).Handle;
        }
    }
}