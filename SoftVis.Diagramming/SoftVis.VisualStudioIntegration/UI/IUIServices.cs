using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Codartis.SoftVis.Util.UI.Wpf.Dialogs;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Defines the UI operations of the diagram control.
    /// </summary>
    public interface IUiServices
    {
        Dpi ImageExportDpi { get; set; }

        void MessageBox(string message);
        ProgressDialog ShowProgressDialog(string text);
        string SelectSaveFilename(string title, string filter);

        void ShowDiagramWindow();
        void FitDiagramToView();
        Task<BitmapSource> CreateDiagramImageAsync(CancellationToken cancellationToken = default(CancellationToken), IProgress<double> progress = null);

        /// <summary>
        /// Provides combo items to fill a combo box in response to a command from the host environment.
        /// </summary>
        /// <param name="e">The event args of the command received from the host environment.</param>
        /// <param name="items">The combo items to be set.</param>
        void FillCombo(EventArgs e, IEnumerable<string> items);

        /// <summary>
        /// Returns a value indicating what type of combo box command is represented in the parameter event args.
        /// </summary>
        /// <param name="e">The event args of the command received from the host environment.</param>
        /// <returns>The type of command requested by the host.</returns>
        ComboCommandType GetComboCommandType(EventArgs e);

        /// <summary>
        /// Sets the current item of the combo represented by the parameter event args.
        /// </summary>
        /// <param name="e">The event args of the command received from the host environment.</param>
        /// <param name="item">The item that should be the current item in the combo.</param>
        void SetCurrentComboItem(EventArgs e, string item);

        /// <summary>
        /// Returns the new selected item of the combo represented by the parameter event args.
        /// </summary>
        /// <param name="e">The event args of the command received from the host environment.</param>
        /// <returns>The selected item in the combo.</returns>
        string GetSelectedComboItem(EventArgs e);
    }
}
