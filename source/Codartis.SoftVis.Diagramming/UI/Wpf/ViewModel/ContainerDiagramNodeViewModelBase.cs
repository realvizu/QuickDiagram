using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util.UI;
using Codartis.SoftVis.Util.UI.Wpf.Collections;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Abstract view model base class for diagram nodes that can contain other diagram nodes.
    /// </summary>
    public abstract class ContainerDiagramNodeViewModelBase : DiagramNodeViewModelBase, IContainerDiagramNodeUi
    {
        public ThreadSafeObservableCollection<DiagramNodeViewModelBase> ChildNodes { get; }

        protected ContainerDiagramNodeViewModelBase(IModelService modelService, IDiagramService diagramService,
            IFocusTracker<IDiagramShapeUi> focusTracker, IDiagramNode diagramNode) 
            : base(modelService, diagramService, focusTracker, diagramNode)
        {
            ChildNodes = new ThreadSafeObservableCollection<DiagramNodeViewModelBase>();
        }

        public override void Dispose()
        {
            foreach (var childNode in ChildNodes)
                childNode.Dispose();
            
            base.Dispose();
        }

        public void AddChildNode(IDiagramNodeUi childNode)
        {
            ChildNodes.Add(childNode as DiagramNodeViewModelBase);
        }
    }
}
