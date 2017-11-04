using System.Collections.Generic;
using System.Collections.ObjectModel;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Abstract view model base class for diagram nodes that can contain other diagram nodes.
    /// </summary>
    public abstract class ContainerDiagramNodeViewModelBase : DiagramNodeViewModelBase, IContainerDiagramNodeUi
    {
        public ICollection<DiagramNodeViewModelBase> ChildNodes { get; }

        protected ContainerDiagramNodeViewModelBase(IModelService modelService, IDiagramService diagramService, IDiagramNode diagramNode) 
            : base(modelService, diagramService, diagramNode)
        {
            ChildNodes = new ObservableCollection<DiagramNodeViewModelBase>();
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
