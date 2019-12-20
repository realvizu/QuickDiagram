using System;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming.Definition;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Activates the source code editor window for a given diagram node.
    /// </summary>
    internal sealed class ShowSourceFileCommand : CommandBase
    {
        [NotNull] private const string NoSourceMessage = "There's no source file for this item.";
        private static readonly TimeSpan NoSourceMessageDuration = TimeSpan.FromSeconds(5);

        [NotNull] private readonly IDiagramNode _diagramNode;

        public ShowSourceFileCommand(
            [NotNull] IAppServices appServices,
            [NotNull] IDiagramNode diagramNode)
            : base(appServices)
        {
            _diagramNode = diagramNode;
        }

        public override async Task ExecuteAsync()
        {
            var symbol = RoslynBasedModelService.GetSymbol(_diagramNode.ModelNode);

            if (await RoslynWorkspaceProvider.HasSourceAsync(symbol))
                await RoslynWorkspaceProvider.ShowSourceAsync(symbol);
            else
                DiagramWindowService.ShowPopupMessage(NoSourceMessage, NoSourceMessageDuration);
        }
    }
}