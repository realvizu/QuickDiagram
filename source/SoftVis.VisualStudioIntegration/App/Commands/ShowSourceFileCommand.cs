using System;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Activates the source code editor window for a given Symbol.
    /// </summary>
    internal class ShowSourceFileCommand : SyncCommandWithParameterBase<IDiagramNode>
    {
        private const string NoSourceMessage = "There's no source file for this item.";
        private static readonly TimeSpan NoSourceMessageDuration = TimeSpan.FromSeconds(5);

        public ShowSourceFileCommand(IAppServices appServices)
            : base(appServices)
        {
        }

#pragma warning disable VSTHRD100 // Avoid async void methods
        public override async void Execute(IDiagramNode diagramNode)
#pragma warning restore VSTHRD100 // Avoid async void methods
        {
            if (!(diagramNode?.ModelNode is IRoslynModelNode roslynModelNode))
                throw new Exception("DiagramNode or ModelNode is null or not an IRoslynModelNode.");

            if (await ModelService.HasSourceAsync(roslynModelNode))
                await ModelService.ShowSourceAsync(roslynModelNode);
            else
                UiService.ShowPopupMessage(NoSourceMessage, NoSourceMessageDuration);
        }
    }
}
