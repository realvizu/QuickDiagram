using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Finds those types that implement the given interface symbol.
    /// </summary>		      
    internal class ImplementingTypesFinderVisitor : SymbolVisitor
    {
        private INamedTypeSymbol InterfaceSymbol { get; }
        public List<INamedTypeSymbol> ImplementingTypeSymbols { get; }

        internal ImplementingTypesFinderVisitor(INamedTypeSymbol interfaceSymbol)
        {
            if (interfaceSymbol.TypeKind != TypeKind.Interface)
                throw new ArgumentException($"Interface expected but received {interfaceSymbol.TypeKind}.");

            InterfaceSymbol = interfaceSymbol;
            ImplementingTypeSymbols = new List<INamedTypeSymbol>();
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
            if (symbol.TypeKind != TypeKind.Interface && 
                symbol.OriginalDefinition.Interfaces.Select(i => i.OriginalDefinition).Contains(InterfaceSymbol))
                ImplementingTypeSymbols.Add(symbol);

            foreach (var namedTypeSymbol in symbol.GetTypeMembers())
                Visit(namedTypeSymbol);
        }
    }
}