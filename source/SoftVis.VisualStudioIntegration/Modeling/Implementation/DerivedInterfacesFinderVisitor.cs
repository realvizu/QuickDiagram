using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.VisualStudioIntegration.Util;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Finds those interfaces that derive from the given interface symbol.
    /// </summary>
    internal class DerivedInterfacesFinderVisitor : SymbolVisitor
    {
        private static readonly SymbolEqualityComparer SymbolEqualityComparer = new SymbolEqualityComparer();

        private INamedTypeSymbol InterfaceSymbol { get; }
        public List<INamedTypeSymbol> DerivedInterfaces { get; }

        internal DerivedInterfacesFinderVisitor(INamedTypeSymbol interfaceSymbol)
        {
            if (interfaceSymbol.TypeKind != TypeKind.Interface)
                throw new ArgumentException($"Interface expected but received {interfaceSymbol.TypeKind}.");

            InterfaceSymbol = interfaceSymbol;
            DerivedInterfaces = new List<INamedTypeSymbol>();
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
            if (symbol.TypeKind == TypeKind.Interface &&
                symbol.OriginalDefinition.Interfaces.Select(i => i.OriginalDefinition).Contains(InterfaceSymbol, SymbolEqualityComparer))
                DerivedInterfaces.Add(symbol);

            foreach (var namedTypeSymbol in symbol.GetTypeMembers())
                Visit(namedTypeSymbol);
        }
    }
}
