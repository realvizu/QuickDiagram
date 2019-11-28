using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.Util.UI;
using Codartis.Util.UI.Wpf;
using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp.UI
{
    public sealed class TestDiagramShapeViewModelFactory : DiagramShapeViewModelFactory
    {
        public TestDiagramShapeViewModelFactory(
            [NotNull] IModelEventSource modelEventSource,
            [NotNull] IDiagramEventSource diagramEventSource,
            [NotNull] IRelatedNodeTypeProvider relatedNodeTypeProvider)
            : base(modelEventSource, diagramEventSource, relatedNodeTypeProvider)
        {
        }

        public override IDiagramNodeUi CreateDiagramNodeUi(
            IDiagramNode diagramNode,
            IFocusTracker<IDiagramShapeUi> focusTracker)
        {
            return new TestDiagramNodeViewModel(
                ModelEventSource,
                DiagramEventSource,
                diagramNode,
                RelatedNodeTypeProvider,
                (IWpfFocusTracker<IDiagramShapeUi>)focusTracker);
        }
    }
}