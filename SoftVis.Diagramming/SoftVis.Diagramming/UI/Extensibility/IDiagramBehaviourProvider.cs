using System.Collections.Generic;

namespace Codartis.SoftVis.UI.Extensibility
{
    /// <summary>
    /// Extensibility point for the customization of a diagram's behaviour.
    /// </summary>
    public interface IDiagramBehaviourProvider
    {
        IEnumerable<RelatedEntityButtonDescriptor> GetRelatedEntityButtonDescriptors();
    }
}
