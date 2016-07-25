using System;
using System.Collections.Generic;

namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    /// <summary>
    /// Operations to access the UI services of the host environment.
    /// </summary>
    public interface IHostUiServices
    {
        /// <summary>
        /// Gets the window that hosts the diagram control.
        /// </summary>
        IHostWindow DiagramHostWindow { get; }

        /// <summary>
        /// Adds a menu command item to Visual Studio.
        /// </summary>
        /// <param name="commandSet">The Guid ID of the command.</param>
        /// <param name="commandId">The int ID of the command.</param>
        /// <param name="commandDelegate">The delegete to execute if the command is triggered.</param>
        void AddMenuCommand(Guid commandSet, int commandId, EventHandler commandDelegate);

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
        /// <returns>The type od command requested by the host.</returns>
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
