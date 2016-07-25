namespace Codartis.SoftVis.VisualStudioIntegration.App
{
    /// <summary>
    /// Operations of the host window provided by the host environment.
    /// </summary>
    public interface IHostWindow
    {
        void Initialize(string caption, object control);
        void Show();
        void Hide();
    }
}
