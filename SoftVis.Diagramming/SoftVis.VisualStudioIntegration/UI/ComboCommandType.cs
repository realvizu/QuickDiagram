namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Enumerates the different command types that the host environment can send for a combo box.
    /// </summary>
    // TODO: this should be moved to Hosting because it's VS-specific plumbing.
    public enum ComboCommandType
    {
        /// <summary>
        /// The host request a string that will be the current combo item.
        /// </summary>
        CurrentItemRequested,

        /// <summary>
        /// The selected item in the combo was changed.
        /// </summary>
        SelectedItemChanged,

        /// <summary>
        /// The command type could not be determined.
        /// </summary>
        Unknown
    }
}
