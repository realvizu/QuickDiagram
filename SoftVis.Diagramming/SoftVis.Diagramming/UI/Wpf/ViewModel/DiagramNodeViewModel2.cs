using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Wpf.Common.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Defines the visible properties of diagram nodes.
    /// </summary>
    public sealed class DiagramNodeViewModel2 : DiagramShapeViewModelBase
    {
        private readonly DiagramNode _diagramNode;

        public DiagramNodeViewModel2(DiagramNode diagramNode)
        {
            _diagramNode = diagramNode;
            UpdateState();
        }

        public override void UpdateState()
        {
            Position = _diagramNode.Position.ToWpf();
            Size = _diagramNode.Size.ToWpf();
        }

        public string Name => _diagramNode.Name;
        public ModelEntityStereotype Stereotype => _diagramNode.ModelEntity.Stereotype;
        public bool IsStereotypeVisible => Stereotype != null;
        public string StereotypeText => $"<<{Stereotype?.Name.ToLower()}>>";
        public FontStyle FontStyle => _diagramNode.ModelEntity.IsAbstract ? FontStyles.Oblique : FontStyles.Normal;
    }
}
