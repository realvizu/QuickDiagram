using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Extensibility;
using Codartis.SoftVis.UI.Wpf.Commands;

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

        public DiagramNodeViewModel(IReadOnlyModel model, IDiagram diagram,
            IDiagramBehaviourProvider diagramBehaviourProvider, IDiagramNode diagramNode)
              : base(model, diagram)
        {
            DiagramNode = diagramNode;
            UpdatePropertiesFromDiagramShape();

            DoubleClickCommand = new DelegateCommand(OnDoubleClick);
            RelatedEntityCueViewModels = CreateRelatedEntityCueViewModels(diagramBehaviourProvider);
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

        private List<RelatedEntityCueViewModel> CreateRelatedEntityCueViewModels(
            IDiagramBehaviourProvider diagramBehaviourProvider)
        {
            return diagramBehaviourProvider
                .GetRelatedEntityButtonDescriptors()
                .Select(i => new RelatedEntityCueViewModel(Model, Diagram, i, DiagramNode, Size))
                .ToList();
        }
    }
}
