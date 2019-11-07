//using System;
//using System.Threading.Tasks;
//using Codartis.SoftVis.Diagramming;
//using Codartis.SoftVis.VisualStudioIntegration.Modeling;

//namespace Codartis.SoftVis.VisualStudioIntegration.App.Commands
//{
//    /// <summary>
//    /// Activates the source code editor window for a given Symbol.
//    /// </summary>
//    internal class ShowSourceFileCommand : CommandBase
//    {
//        private const string NoSourceMessage = "There's no source file for this item.";
//        private static readonly TimeSpan NoSourceMessageDuration = TimeSpan.FromSeconds(5);

//        private readonly IDiagramNode _diagramNode;

//        public ShowSourceFileCommand(IAppServices appServices, IDiagramNode diagramNode)
//            : base(appServices)
//        {
//            _diagramNode = diagramNode;
//        }

//        public override async Task ExecuteAsync()
//        {
//            if (!(_diagramNode?.ModelNode is IRoslynSymbol roslynModelNode))
//                throw new Exception("DiagramNode or ModelNode is null or not an IRoslynModelNode.");

//            if (await ModelService.HasSourceAsync(roslynModelNode))
//                await ModelService.ShowSourceAsync(roslynModelNode);
//            else
//                HostUiService.ShowPopupMessage(NoSourceMessage, NoSourceMessageDuration);
//        }
//    }
//}
