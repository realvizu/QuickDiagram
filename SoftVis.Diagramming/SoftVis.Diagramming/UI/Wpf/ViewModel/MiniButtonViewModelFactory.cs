using System.Collections.Generic;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.UI.Extensibility;
using Codartis.SoftVis.UI.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Creates MiniButton view model objects.
    /// </summary>
    internal class MiniButtonViewModelFactory
    {
        private readonly IDiagramBehaviourProvider _diagramBehaviourProvider;
        private readonly double _miniButtonRadius;
        private readonly double _miniButtonOverlap;

        public MiniButtonViewModelFactory(IDiagramBehaviourProvider diagramBehaviourProvider, 
            double miniButtonRadius, double miniButtonOverlap)
        {
            _diagramBehaviourProvider = diagramBehaviourProvider;
            _miniButtonRadius = miniButtonRadius;
            _miniButtonOverlap = miniButtonOverlap;
        }

        public IEnumerable<MiniButtonViewModelBase> CreateButtons()
        {
            yield return CreateCloseButton();
            foreach (var descriptor in _diagramBehaviourProvider.GetRelatedEntityMiniButtonDescriptors())
            {
                yield return CreateShowRelatedEntityButton(descriptor);
            }
        }

        private CloseMiniButtonViewModel CreateCloseButton()
        {
            var translate = new Point2D(-_miniButtonOverlap, _miniButtonOverlap);
            var miniButtonLocation = new RectRelativeLocation(RectAlignment.TopRight, translate);
            return new CloseMiniButtonViewModel(_miniButtonRadius, miniButtonLocation);
        }

        private ShowRelatedEntityMiniButtonViewModel CreateShowRelatedEntityButton(
            RelatedEntityMiniButtonDescriptor descriptor)
        {
            return new ShowRelatedEntityMiniButtonViewModel(_miniButtonRadius, descriptor);
        }
    }
}
