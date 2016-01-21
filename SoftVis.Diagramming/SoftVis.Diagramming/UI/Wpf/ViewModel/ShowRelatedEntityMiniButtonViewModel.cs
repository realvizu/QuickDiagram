using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Extensibility;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    /// <summary>
    /// A minibutton to choose related entities.
    /// </summary>
    internal class ShowRelatedEntityMiniButtonViewModel : MiniButtonViewModelBase
    {
        private readonly RelatedEntityMiniButtonDescriptor _descriptor;

        public ShowRelatedEntityMiniButtonViewModel(double miniButtonRadius,
            RelatedEntityMiniButtonDescriptor descriptor)
            : base(miniButtonRadius, descriptor.MiniButtonLocation)
        {
            _descriptor = descriptor;
        }

        public ConnectorType ConnectorType => _descriptor.ConnectorType;
        public RelationshipSpecification RelationshipSpecification => _descriptor.RelationshipSpecification;
    }
}
