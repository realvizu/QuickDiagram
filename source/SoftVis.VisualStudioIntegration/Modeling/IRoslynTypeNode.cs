using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// A model node that represents a Roslyn type.
    /// </summary>
    internal interface IRoslynTypeNode : IRoslynModelNode
    {
        INamedTypeSymbol NamedTypeSymbol { get; }
        bool IsAbstract { get; }
        string FullName { get; }
        string Description { get; }
    }
}