using System.Windows.Media.Imaging;

namespace Codartis.SoftVis.VisualStudioIntegration.Presentation
{
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
