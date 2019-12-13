using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Finds those interfaces that derive from the given interface symbol.
    /// </summary>
    internal class DerivedInterfacesFinderVisitor : SymbolVisitor
    {
        [NotNull] private readonly INamedTypeSymbol _interfaceSymbol;
        [NotNull] private readonly IEqualityComparer<ISymbol> _symbolEqualityComparer;
        [NotNull] public List<INamedTypeSymbol> DerivedInterfaces { get; }

        internal DerivedInterfacesFinderVisitor(
            [NotNull] INamedTypeSymbol interfaceSymbol,
            [NotNull] IEqualityComparer<ISymbol> symbolEqualityComparer)
        {
            if (interfaceSymbol.TypeKind != TypeKind.Interface)
                throw new ArgumentException($"Interface expected but received {interfaceSymbol.TypeKind}.");

            _interfaceSymbol = interfaceSymbol;
            _symbolEqualityComparer = symbolEqualityComparer;
            DerivedInterfaces = new List<INamedTypeSymbol>();
        }

        public override void VisitAssembly([NotNull] IAssemblySymbol symbol)
        {
            Visit(symbol.GlobalNamespace);
        }

        public override void VisitNamespace([NotNull] INamespaceSymbol symbol)
        {
            foreach (var namespaceSymbol in symbol.GetNamespaceMembers())
                Visit(namespaceSymbol);

            foreach (var namedTypeSymbol in symbol.GetTypeMembers())
                Visit(namedTypeSymbol);
        }

        public override void VisitNamedType([NotNull] INamedTypeSymbol symbol)
        {
            if (symbol.TypeKind == TypeKind.Interface &&
                symbol.OriginalDefinition.Interfaces.Select(i => i.OriginalDefinition).Contains(_interfaceSymbol, _symbolEqualityComparer))
                DerivedInterfaces.Add(symbol);

            foreach (var namedTypeSymbol in symbol.GetTypeMembers())
                Visit(namedTypeSymbol);
        }
    }
}