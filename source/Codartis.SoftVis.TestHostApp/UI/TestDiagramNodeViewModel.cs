using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.TestHostApp.Modeling;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.Util.UI.Wpf;
using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp.UI
{
    public class TestDiagramNodeViewModel : DiagramNodeViewModel
    {
        public TestDiagramNodeViewModel(
            [NotNull] IModelEventSource modelEventSource,
            [NotNull] IDiagramEventSource diagramEventSource,
            [NotNull] IDiagramNode diagramNode,
            [NotNull] IRelatedNodeTypeProvider relatedNodeTypeProvider,
            [NotNull] IWpfFocusTracker<IDiagramShapeUi> focusTracker)
            : base(modelEventSource, diagramEventSource, diagramNode, relatedNodeTypeProvider, focusTracker)
        {
        }

        public override IDiagramNodeHeaderUi HeaderUi => (ITestNode)ModelNode.Payload;

        protected override DiagramNodeViewModel CreateInstance()
        {
            return new TestDiagramNodeViewModel(
                ModelEventSource,
                DiagramEventSource,
                DiagramNode,
                RelatedNodeTypeProvider,
                FocusTracker);
        }
    }
}