using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting.CommandRegistration
{
    /// <summary>
    /// Describes the properties of a combo specification.
    /// </summary>
    internal interface IComboSpecification
    {
        int GetItemsCommandId { get; }
        int ComboCommandId { get; }
        Type ComboAdapterType { get; }
    }
}
