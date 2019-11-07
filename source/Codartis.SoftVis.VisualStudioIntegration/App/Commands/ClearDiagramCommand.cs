using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Clears the diagram.
    /// </summary>
    [UsedImplicitly]
    internal sealed class ClearDiagramCommand : CommandBase
    {
        public ClearDiagramCommand(IAppServices appServices)
            : base(appServices)
        {
        }

        public override async Task ExecuteAsync()
        {
            await HostUiService.ShowDiagramWindowAsync();
            DiagramService.ClearDiagram();
        }
    }
}