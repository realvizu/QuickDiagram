using System;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
{
    /// <summary>
    /// Activates the source code editor window for a given Symbol.
    /// </summary>
    internal class ShowSourceFileCommand : AsyncCommandWithParameterBase<IDiagramNode>
    {
        private const string NoSourceMessage = "There's no source file for this item.";
        private static readonly TimeSpan NoSourceMessageDuration = TimeSpan.FromSeconds(5);

        public ShowSourceFileCommand(IAppServices appServices)
            : base(appServices)
        {
        }

        public override async Task ExecuteAsync(IDiagramNode diagramNode)
        {
            var modelEntity = diagramNode?.ModelEntity;
            if (modelEntity == null)
                throw new Exception("Entity missing in DiagramNode.");

            if (await ModelServices.HasSourceAsync(modelEntity))
                await ModelServices.ShowSourceAsync(modelEntity);
            else
                UiServices.ShowPopupMessage(NoSourceMessage, NoSourceMessageDuration);
        }
    }
}
