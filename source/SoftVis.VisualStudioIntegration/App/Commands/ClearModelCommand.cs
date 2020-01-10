using System.Threading.Tasks;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Clears the model (and subsequently the diagram).
    /// </summary>
    internal sealed class ClearModelCommand : AsyncCommandBase
    {
        public ClearModelCommand(IAppServices appServices)
            : base(appServices)
        { }

        public override async Task ExecuteAsync()
        {
            await HostUiServices.ShowDiagramWindowAsync();
            ModelServices.Clear();
        }
    }
}
