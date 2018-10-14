using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Clears the model (and subsequently the diagram).
    /// </summary>
    [UsedImplicitly]
    internal sealed class ClearModelCommand : CommandBase
    {
        public ClearModelCommand(IAppServices appServices)
            : base(appServices)
        { }

        public override async Task ExecuteAsync()
        {
            await UiService.ShowDiagramWindowAsync();
            ModelService.ClearModel();
        }
    }
}
