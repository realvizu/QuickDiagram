using System;
using Codartis.SoftVis.UI;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    public sealed class RoslynSymbolPayloadUiFactory : IPayloadUiFactory
    {
        public IPayloadUi Create(object payload)
        {
            switch (payload)
            {
                case null:
                    return null;
                case INamedTypeSymbol namedTypeSymbol:
                    return new RoslynTypeViewModel(namedTypeSymbol);
                default:
                    throw new Exception($"Unexpected payload type {payload.GetType().Name}");
            }
        }
    }
}