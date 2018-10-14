using System;

namespace Codartis.SoftVis.VisualStudioIntegration.Hosting.CommandRegistration
{
    /// <summary>
    /// Describes the properties of a command specification.
    /// </summary>
    internal interface ICommandSpecification
    {
        int CommandId { get; }
        Type CommandType { get; }
    }
}
