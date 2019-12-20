using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Abstract base class for those commands that add the current symbol to the diagram.
    /// </summary>
    internal abstract class AddCurrentSymbolToDiagramCommandBase : CommandBase
    {
        protected AddCurrentSymbolToDiagramCommandBase([NotNull] IAppServices appServices)
            : base(appServices)
        {
        }

        public override async Task<bool> IsEnabledAsync() => (await RoslynWorkspaceProvider.TryGetCurrentSymbolAsync()).HasValue;

        protected async Task<Maybe<IModelNode>> TryAddCurrentSymbolToDiagramAsync()
        {
            var maybeSymbol = await RoslynWorkspaceProvider.TryGetCurrentSymbolAsync();
            if (!maybeSymbol.HasValue)
                return Maybe<IModelNode>.Nothing;

            var modelNode = RoslynBasedModelService.GetOrAddNode(maybeSymbol.Value);
            DiagramService.AddNode(modelNode.Id);
            return Maybe.Create(modelNode);
        }
    }
}