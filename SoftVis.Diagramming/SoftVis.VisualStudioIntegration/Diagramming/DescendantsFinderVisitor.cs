using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Finds the descendants of the given type symbol.
    /// </summary>
    public class DescendantsFinderVisitor : SymbolVisitor
    {
        private INamedTypeSymbol BaseTypeSymbol { get; }
        public List<INamedTypeSymbol> Descendants { get; }

        public DescendantsFinderVisitor(INamedTypeSymbol baseTypeSymbol)
        {
            BaseTypeSymbol = baseTypeSymbol;
            Descendants = new List<INamedTypeSymbol>();
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
            if (symbol.BaseType == BaseTypeSymbol)
                Descendants.Add(symbol);

            foreach (var namedTypeSymbol in symbol.GetTypeMembers())
                Visit(namedTypeSymbol);
        }
    }
}
