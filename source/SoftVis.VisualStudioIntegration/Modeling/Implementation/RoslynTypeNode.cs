using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.VisualStudioIntegration.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Abstract base class for model nodes that represent Roslyn types.
    /// Immutable.
    /// </summary>
    internal abstract class RoslynTypeNode : RoslynModelNode, IRoslynTypeNode
    {
        public INamedTypeSymbol NamedTypeSymbol { get; }

        protected RoslynTypeNode(ModelItemId id, INamedTypeSymbol roslynSymbol, NodeStereotype stereotype)
            : base(id, roslynSymbol, stereotype)
        {
            NamedTypeSymbol = roslynSymbol ?? throw new ArgumentNullException(nameof(roslynSymbol));
        }

        public bool IsAbstract => NamedTypeSymbol.IsAbstract;
        public string FullName => NamedTypeSymbol.GetFullName();
        public string Description => NamedTypeSymbol.GetDescription();

        protected static IEnumerable<RelatedSymbolPair> GetImplementedInterfaces(INamedTypeSymbol classOrStructSymbol)
        {
            foreach (var implementedInterfaceSymbol in classOrStructSymbol.Interfaces.Where(i => i.TypeKind == TypeKind.Interface))
                yield return new RelatedSymbolPair(classOrStructSymbol, implementedInterfaceSymbol,
                    DirectedRelationshipTypes.ImplementedInterface);
        }

        protected static IEnumerable<RelatedSymbolPair> GetDerivedTypes(IRoslynModelProvider roslynModelProvider, INamedTypeSymbol classSymbol)
        {
            var workspace = roslynModelProvider.GetWorkspace();

            return SymbolFinder.FindDerivedClassesAsync(classSymbol, workspace.CurrentSolution).Result
                .Where(i => classSymbol.SymbolEquals(i.BaseType.OriginalDefinition) && i.TypeKind == TypeKind.Class)
                .Select(i => new RelatedSymbolPair(classSymbol, i, DirectedRelationshipTypes.Subtype));
        }

        protected static IEnumerable<RelatedSymbolPair> GetImplementingTypes(IRoslynModelProvider roslynModelProvider, INamedTypeSymbol interfaceSymbol)
        {
            var workspace = roslynModelProvider.GetWorkspace();
            return FindImplementingTypes(workspace, interfaceSymbol)
                .Select(i => new RelatedSymbolPair(interfaceSymbol, i, DirectedRelationshipTypes.ImplementerType));
        }

        protected static IEnumerable<RelatedSymbolPair> GetDerivedInterfaces(IRoslynModelProvider roslynModelProvider, INamedTypeSymbol interfaceSymbol)
        {
            var workspace = roslynModelProvider.GetWorkspace();
            return FindDerivedInterfaces(workspace, interfaceSymbol)
                .Select(i => new RelatedSymbolPair(interfaceSymbol, i, DirectedRelationshipTypes.Subtype));
        }

        private static IEnumerable<INamedTypeSymbol> FindImplementingTypes(Workspace workspace, INamedTypeSymbol interfaceSymbol)
        {
            var implementerSymbols = SymbolFinder.FindImplementationsAsync(interfaceSymbol, workspace.CurrentSolution).Result;
            foreach (var namedTypeSymbol in implementerSymbols.OfType<INamedTypeSymbol>())
            {
                var interfaces = namedTypeSymbol.Interfaces.Select(i => i.OriginalDefinition);
                if (interfaces.Any(i => i.SymbolEquals(interfaceSymbol)))
                    yield return namedTypeSymbol;
            }

            // For some reason SymbolFinder does not find implementer structs. So we also make a search with a visitor.

            foreach (var compilation in GetCompilations(workspace))
            {
                var visitor = new ImplementingTypesFinderVisitor(interfaceSymbol);
                compilation.Assembly?.Accept(visitor);

                foreach (var descendant in visitor.ImplementingTypeSymbols.Where(i => i.TypeKind == TypeKind.Struct))
                    yield return descendant;
            }
        }

        private static IEnumerable<INamedTypeSymbol> FindDerivedInterfaces(Workspace workspace, INamedTypeSymbol interfaceSymbol)
        {
            foreach (var compilation in GetCompilations(workspace))
            {
                var visitor = new DerivedInterfacesFinderVisitor(interfaceSymbol);
                compilation.Assembly?.Accept(visitor);

                foreach (var descendant in visitor.DerivedInterfaces)
                    yield return descendant;
            }
        }
    }
}