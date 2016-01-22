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
        private readonly RelatedEntityMiniButtonDescriptor _descriptor;

        public ShowRelatedShapeButtonViewModel(double miniButtonRadius,
            RelatedEntityMiniButtonDescriptor descriptor)
            : base(miniButtonRadius, descriptor.MiniButtonLocation)
        {
            _descriptor = descriptor;
        }

        public ConnectorType ConnectorType => _descriptor.ConnectorType;
        public RelationshipSpecification RelationshipSpecification => _descriptor.RelationshipSpecification;
    }
}
