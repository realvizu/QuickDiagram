using System.Threading.Tasks;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Abstract base class for those commands that add the current symbol to the diagram.
    /// </summary>
    internal abstract class AddCurrentSymbolToDiagramCommandBase : AsyncCommandWithoutParameterBase
    {
        protected AddCurrentSymbolToDiagramCommandBase(IAppServices appServices) 
            : base(appServices)
        {
        }

        public override async Task<bool> IsEnabledAsync()
        {
            return await ModelService.IsCurrentSymbolAvailableAsync();
        }
    }
}