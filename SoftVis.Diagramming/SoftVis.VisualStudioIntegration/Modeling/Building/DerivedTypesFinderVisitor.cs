using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Building
{
    /// <summary>
    /// Finds those types whose base is the given type symbol.
    /// </summary>
    internal class DerivedTypesFinderVisitor : SymbolVisitor
    {
        private INamedTypeSymbol BaseTypeSymbol { get; }
        public List<INamedTypeSymbol> DerivedTypeSymbols { get; }

        internal DerivedTypesFinderVisitor(INamedTypeSymbol baseTypeSymbol)
        {
            BaseTypeSymbol = baseTypeSymbol;
            DerivedTypeSymbols = new List<INamedTypeSymbol>();
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
                DerivedTypeSymbols.Add(symbol);

            foreach (var namedTypeSymbol in symbol.GetTypeMembers())
                Visit(namedTypeSymbol);
        }
    }
}
