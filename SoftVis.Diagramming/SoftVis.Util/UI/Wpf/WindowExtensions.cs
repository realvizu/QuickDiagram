using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
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
        public static void ShowNonBlockingModal(this Window window)
        {
            window.Owner.Disable();

            window.Closing += OnClosing;
            window.Show();
        }

        private static void OnClosing(object sender, CancelEventArgs e)
        {
            var window = (Window)sender;
            window.Closing -= OnClosing;

            window.Owner.Enable();
            window.Owner.Activate();
        }

        public static void Enable(this Window window) => window.SetEnableState(true);
        public static void Disable(this Window window) => window.SetEnableState(false);

        public static void SetEnableState(this Window window, bool enable)
        {
            var handle = new WindowInteropHelper(window).Handle;
            EnableWindow(handle, enable);
        }
    }
}