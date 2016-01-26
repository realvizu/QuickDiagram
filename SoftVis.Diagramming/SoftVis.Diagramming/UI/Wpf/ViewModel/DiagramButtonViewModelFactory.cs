using System.Collections.Generic;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.UI.Extensibility;
using Codartis.SoftVis.UI.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Creates diagram button view model objects.
    /// </summary>
    internal class DiagramButtonViewModelFactory
    {
        private readonly IDiagramBehaviourProvider _diagramBehaviourProvider;
        private readonly double _buttonRadius;
        private readonly double _buttonOverlap;

        public DiagramButtonViewModelFactory(IDiagramBehaviourProvider diagramBehaviourProvider, 
            double buttonRadius, double buttonOverlap)
        {
            _diagramBehaviourProvider = diagramBehaviourProvider;
            _buttonRadius = buttonRadius;
            _buttonOverlap = buttonOverlap;
        }

        public IEnumerable<DiagramButtonViewModelBase> CreateButtons()
        {
            yield return CreateCloseButton();
            foreach (var descriptor in _diagramBehaviourProvider.GetRelatedEntityButtonDescriptors())
                yield return CreateShowRelatedEntityButton(descriptor);
        }

        private CloseShapeButtonViewModel CreateCloseButton()
        {
            var translate = new Point2D(-_buttonOverlap, _buttonOverlap);
            var buttonLocation = new RectRelativeLocation(RectAlignment.TopRight, translate);
            return new CloseShapeButtonViewModel(_buttonRadius, buttonLocation);
        }

        private ShowRelatedShapeButtonViewModel CreateShowRelatedEntityButton(RelatedEntityButtonDescriptor descriptor)
        {
            return new ShowRelatedShapeButtonViewModel(_buttonRadius, descriptor);
        }
    }
}
