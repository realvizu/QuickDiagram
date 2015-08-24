using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    public class DescendantsFinderVisitor : SymbolVisitor
    {
        private readonly INamedTypeSymbol _baseTypeSymbol;

        public List<INamedTypeSymbol> Descendants = new List<INamedTypeSymbol>();

        public DescendantsFinderVisitor(INamedTypeSymbol baseTypeSymbol)
        {
            _baseTypeSymbol = baseTypeSymbol;
        }

        public override void VisitAssembly(IAssemblySymbol symbol)
        {
            Visit(symbol.GlobalNamespace);
        }

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            foreach (var namespaceSymbol in symbol.GetNamespaceMembers())
                Visit(namespaceSymbol);

            foreach (var namedTypeSymbol in symbol.GetTypeMembers())
                Visit(namedTypeSymbol);
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            if (symbol.BaseType == _baseTypeSymbol)
                Descendants.Add(symbol);

            foreach (var namedTypeSymbol in symbol.GetTypeMembers())
                Visit(namedTypeSymbol);
        }
    }
}
