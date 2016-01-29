using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Graph;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Wpf.Common.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Defines the visible properties of diagram nodes.
    /// </summary>
    public sealed class DiagramNodeViewModel2 : DiagramShapeViewModelBase
    {
        public DiagramNode DiagramNode { get; }

        public DiagramNodeViewModel2(IModel model, Diagram diagram, DiagramNode diagramNode)
              : base(model, diagram)
        {
            DiagramNode = diagramNode;
            UpdateState();
        }

        public override DiagramShape DiagramShape => DiagramNode;

        public override void UpdateState()
        {
            Position = DiagramNode.Position.ToWpf();
            Size = DiagramNode.Size.ToWpf();
        }

        public string Name => DiagramNode.Name;
        public ModelEntityStereotype Stereotype => DiagramNode.ModelEntity.Stereotype;
        public bool IsStereotypeVisible => Stereotype != null;
        public string StereotypeText => $"<<{Stereotype?.Name.ToLower()}>>";
        public FontStyle FontStyle => DiagramNode.ModelEntity.IsAbstract ? FontStyles.Oblique : FontStyles.Normal;
        private IModelEntity ModelEntity => DiagramNode.ModelEntity;
    }
}
