using System.Collections.Generic;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.UI.Extensibility;
using Codartis.SoftVis.UI.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Creates MiniButton view model objects.
    /// </summary>
    internal class DiagramButtonViewModelFactory
    {
        private readonly IDiagramBehaviourProvider _diagramBehaviourProvider;
        private readonly double _miniButtonRadius;
        private readonly double _miniButtonOverlap;

        public DiagramButtonViewModelFactory(IDiagramBehaviourProvider diagramBehaviourProvider, 
            double miniButtonRadius, double miniButtonOverlap)
        {
            _diagramBehaviourProvider = diagramBehaviourProvider;
            _miniButtonRadius = miniButtonRadius;
            _miniButtonOverlap = miniButtonOverlap;
        }

        public IEnumerable<DiagramButtonViewModelBase> CreateButtons()
        {
            yield return CreateCloseButton();
            foreach (var descriptor in _diagramBehaviourProvider.GetRelatedEntityMiniButtonDescriptors())
            {
                yield return CreateShowRelatedEntityButton(descriptor);
            }
        }

        private CloseShapeButtonViewModel CreateCloseButton()
        {
            var translate = new Point2D(-_miniButtonOverlap, _miniButtonOverlap);
            var miniButtonLocation = new RectRelativeLocation(RectAlignment.TopRight, translate);
            return new CloseShapeButtonViewModel(_miniButtonRadius, miniButtonLocation);
        }

        private ShowRelatedShapeButtonViewModel CreateShowRelatedEntityButton(
            RelatedEntityMiniButtonDescriptor descriptor)
        {
            return new ShowRelatedShapeButtonViewModel(_miniButtonRadius, descriptor);
        }
    }
}
