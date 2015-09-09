using System.Windows.Media.Imaging;

namespace Codartis.SoftVis.VisualStudioIntegration.Presentation
{
    /// <summary>
    /// Defines the operations of a diagram window.
    /// </summary>
    public interface IDiagramWindow
    {
        void Show();
        void AddCurrentSymbol();
        void Clear();
        void IncreaseFontSize();
        void DecreaseFontSize();
        BitmapSource GetDiagramAsBitmap();
    }
}
