using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Geometry;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Extensibility;
using Codartis.SoftVis.UI.Geometry;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// Creates diagram button view model objects.
    /// </summary>
    internal class DiagramShapeButtonViewModelFactory : DiagramViewModelBase
    {
        private readonly IDiagramBehaviourProvider _diagramBehaviourProvider;
        private readonly double _buttonRadius;
        private readonly double _buttonOverlap;

        public DiagramShapeButtonViewModelFactory(IReadOnlyModel model, IDiagram diagram,
            IDiagramBehaviourProvider diagramBehaviourProvider, double buttonRadius, double buttonOverlap)
            : base(model, diagram)
        {
            _diagramBehaviourProvider = diagramBehaviourProvider;
            _buttonRadius = buttonRadius;
            _buttonOverlap = buttonOverlap;
        }

        public IEnumerable<DiagramShapeButtonViewModelBase> CreateButtons()
        {
            yield return CreateCloseButton();
            foreach (var descriptor in _diagramBehaviourProvider.GetRelatedEntityButtonDescriptors())
                yield return CreateShowRelatedEntityButton(descriptor);
        }

        private CloseShapeButtonViewModel CreateCloseButton()
        {
            var translate = new Point2D(-_buttonOverlap, _buttonOverlap);
            var buttonLocation = new RectRelativeLocation(RectAlignment.TopRight, translate);
            return new CloseShapeButtonViewModel(Model, Diagram, _buttonRadius, buttonLocation);
        }

        private ShowRelatedNodeButtonViewModel CreateShowRelatedEntityButton(RelatedEntityButtonDescriptor descriptor)
        {
            return new ShowRelatedNodeButtonViewModel(Model, Diagram, _buttonRadius, descriptor);
        }
    }
}
