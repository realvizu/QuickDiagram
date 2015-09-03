using Codartis.SoftVis.VisualStudioIntegration.Hosting;

namespace Codartis.SoftVis.VisualStudioIntegration.Commands
{
    public interface IWindowManager
    {
        DiagramToolWindow GetDiagramWindow();
        void ShowDiagramWindow();
        void IncreaseFontSize();
        void DecreaseFontSize();
    }
}
