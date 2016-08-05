using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming;
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
        public DelegateCommand DoubleClickCommand { get; }
        public List<RelatedEntityCueViewModel> RelatedEntityCueViewModels { get; }

        public DiagramNodeViewModel(IDiagram diagram, IDiagramNode diagramNode)
              : base(diagram)
        {
            DiagramNode = diagramNode;
            UpdatePropertiesFromDiagramShape();

            DoubleClickCommand = new DelegateCommand(OnDoubleClick);
            RelatedEntityCueViewModels = CreateRelatedEntityCueViewModels();
        }

        public override IDiagramShape DiagramShape => DiagramNode;

        public override void UpdatePropertiesFromDiagramShape()
        {
            Position = DiagramNode.Position.ToWpf();
            Size = DiagramNode.Size.ToWpf();
        }

        public string Name => DiagramNode.Name;
        public IModelEntity ModelEntity => DiagramNode.ModelEntity;
        public ModelEntityStereotype Stereotype => ModelEntity.Stereotype;
        public bool IsStereotypeVisible => Stereotype != ModelEntityStereotype.None;
        public string StereotypeText => IsStereotypeVisible ? $"<<{Stereotype.Name.ToLower()}>>" : string.Empty;
        public FontStyle FontStyle => ModelEntity.IsAbstract ? FontStyles.Oblique : FontStyles.Normal;

        private void OnDoubleClick() => Diagram.ActivateShape(DiagramNode);

        private List<RelatedEntityCueViewModel> CreateRelatedEntityCueViewModels()
        {
            return Diagram.GetEntityRelationTypes()
                .Select(i => new RelatedEntityCueViewModel(Diagram, DiagramNode, i))
                .ToList();
        }
    }
}
