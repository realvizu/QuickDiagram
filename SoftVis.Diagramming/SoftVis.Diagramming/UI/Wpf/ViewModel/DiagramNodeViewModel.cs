using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Defines the visible properties of a diagram node.
    /// </summary>
    public sealed class DiagramNodeViewModel : DiagramShapeViewModelBase
    {
        public IDiagramNode DiagramNode { get; }

        public DelegateCommand FocusCommand { get; }
        public DelegateCommand DoubleClickCommand { get; }

        public List<RelatedEntityCueViewModel> RelatedEntityCueViewModels { get; }

        public DiagramNodeViewModel(IArrangedDiagram diagram, IDiagramNode diagramNode)
              : base(diagram)
        {
            DiagramNode = diagramNode;
            DiagramNode.TopLeftChanged += DiagramNodeOnTopLeftChanged;

            FocusCommand = new DelegateCommand(Focus);
            DoubleClickCommand = new DelegateCommand(OnDoubleClick);

            RelatedEntityCueViewModels = CreateRelatedEntityCueViewModels();
        }

        private void DiagramNodeOnTopLeftChanged(IDiagramNode diagramNode, Point2D oldTopLeft, Point2D newTopLeft)
        {
            TopLeft = newTopLeft.ToWpf();
        }

        public override IDiagramShape DiagramShape => DiagramNode;
        public string Name => DiagramNode.Name;
        public string FullName => DiagramNode.FullName;
        public IModelEntity ModelEntity => DiagramNode.ModelEntity;
        public ModelEntityStereotype Stereotype => ModelEntity.Stereotype;
        public bool IsStereotypeVisible => Stereotype != ModelEntityStereotype.None;
        public string StereotypeText => IsStereotypeVisible ? $"<<{Stereotype.Name.ToLower()}>>" : string.Empty;

        protected override void OnSizeChanged(Size newSize)
        {
            DiagramNode.Size = newSize.FromWpf();
        }

        private void OnDoubleClick() => Diagram.ActivateShape(DiagramNode);

        private List<RelatedEntityCueViewModel> CreateRelatedEntityCueViewModels()
        {
            return Diagram.GetEntityRelationTypes()
                .Select(i => new RelatedEntityCueViewModel(Diagram, DiagramNode, i))
                .ToList();
        }
    }
}
