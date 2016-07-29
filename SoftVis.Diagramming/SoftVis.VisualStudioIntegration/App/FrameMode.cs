namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    /// <summary>
    /// The frame modes of the host window.
    /// </summary>
    /// <remarks>
    /// Mapped from Microsoft.VisualStudio.Shell.Interop.VSFRAMEMODE and VSFRAMEMODE2.
    /// </remarks>
    public enum FrameMode
    {
        Unknown,
        Docked,
        Floating,
        MdiChild,
        AutoHide
    }
}
