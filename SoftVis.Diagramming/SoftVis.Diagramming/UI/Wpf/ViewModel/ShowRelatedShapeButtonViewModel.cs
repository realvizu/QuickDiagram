using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Extensibility;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A diagram button to choose related entities.
    /// </summary>
    internal class ShowRelatedShapeButtonViewModel : DiagramButtonViewModelBase
    {
        private readonly RelatedEntityButtonDescriptor _descriptor;

        public ShowRelatedShapeButtonViewModel(double buttonRadius,
            RelatedEntityButtonDescriptor descriptor)
            : base(buttonRadius, descriptor.ButtonLocation, i => i.Remove())
        {
            _descriptor = descriptor;
        }

        public ConnectorType ConnectorType => _descriptor.ConnectorType;
        public RelationshipSpecification RelationshipSpecification => _descriptor.RelationshipSpecification;

        public override void AssociateWith(DiagramShapeViewModelBase diagramShapeViewModel)
        {
            base.AssociateWith(diagramShapeViewModel);
            IsEnabled = ((DiagramNode)diagramShapeViewModel.DiagramShape).Name.StartsWith("1");
        }
    }
}
