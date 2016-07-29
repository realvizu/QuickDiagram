using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Wpf.Commands;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Defines the visible properties of diagram nodes.
    /// </summary>
    public sealed class DiagramNodeViewModel : DiagramShapeViewModelBase
    {
        public IDiagramNode DiagramNode { get; }

        public DelegateCommand ActivateNodeCommand { get; private set; }

        public DiagramNodeViewModel(IReadOnlyModel readOnlyModel, IDiagram diagram, IDiagramNode diagramNode)
              : base(readOnlyModel, diagram)
        {
            DiagramNode = diagramNode;
            ActivateNodeCommand = new DelegateCommand(OnActivateNode);
            UpdateState();
        }

        public override IDiagramShape DiagramShape => DiagramNode;

        public override void UpdateState()
        {
            Position = DiagramNode.Position.ToWpf();
            Size = DiagramNode.Size.ToWpf();
        }

        public string Name => DiagramNode.Name;
        public ModelEntityStereotype Stereotype => DiagramNode.ModelEntity.Stereotype;
        public bool IsStereotypeVisible => Stereotype != ModelEntityStereotype.None;
        public string StereotypeText => IsStereotypeVisible ? $"<<{Stereotype.Name.ToLower()}>>" : string.Empty;
        public FontStyle FontStyle => DiagramNode.ModelEntity.IsAbstract ? FontStyles.Oblique : FontStyles.Normal;
        private IModelEntity ModelEntity => DiagramNode.ModelEntity;

        private void OnActivateNode() => Diagram.ActivateShape(DiagramNode);
    }
}
