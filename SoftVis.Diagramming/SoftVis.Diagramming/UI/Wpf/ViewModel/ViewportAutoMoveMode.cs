namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Enumerates the automatic viewport movement modes.
    /// </summary>
    public enum ViewportAutoMoveMode
    {
        /// <summary>
        /// The viewport center moves to the center of the target rect.
        /// </summary>
        Center,

        /// <summary>
        /// The viewport moves the minimal amount that is needed to contain the target rect.
        /// </summary>
        Contain
    }
}
