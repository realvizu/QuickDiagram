using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.VisualStudioIntegration.Util;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Abstract base class for model nodes that represent Roslyn types.
    /// Immutable.
    /// </summary>
    internal abstract class RoslynTypeBase : RoslynSymbolBase, IRoslynTypeNode
    {
        public INamedTypeSymbol NamedTypeSymbol { get; }

        protected RoslynTypeBase([NotNull] INamedTypeSymbol roslynSymbol)
            : base(roslynSymbol)
        {
            NamedTypeSymbol = roslynSymbol;
        }

        public virtual bool IsAbstract => false;
        public string FullName => NamedTypeSymbol.GetFullName();
        public string Description => NamedTypeSymbol.GetDescription();

        [NotNull]
        protected INamedTypeSymbol EnsureNamedTypeSymbol([NotNull] ISymbol newSymbol)
        {
            if (newSymbol is INamedTypeSymbol namedTypeSymbol)
                return namedTypeSymbol;

            throw new InvalidOperationException($"INamedTypeSymbol expected but received {newSymbol.GetType().Name}");
        }

        [NotNull]
        protected static IEnumerable<RelatedSymbolPair> GetImplementedInterfaces([NotNull] INamedTypeSymbol classOrStructSymbol)
        {
            foreach (var implementedInterfaceSymbol in classOrStructSymbol.Interfaces.Where(i => i.TypeKind == TypeKind.Interface))
                yield return new RelatedSymbolPair(
                    classOrStructSymbol,
                    implementedInterfaceSymbol,
                    DirectedModelRelationshipTypes.ImplementedInterface);
        }

        [NotNull]
        [ItemNotNull]
        protected static async Task<IEnumerable<RelatedSymbolPair>> GetDerivedTypesAsync(
            [NotNull] IRoslynModelProvider roslynModelProvider,
            [NotNull] INamedTypeSymbol classSymbol)
        {
            var workspace = await roslynModelProvider.GetWorkspaceAsync();
            
            var derivedClasses = await SymbolFinder.FindDerivedClassesAsync(classSymbol, workspace.CurrentSolution);

            return derivedClasses
                .Where(i => classSymbol.SymbolEquals(i.BaseType.OriginalDefinition) && i.TypeKind == TypeKind.Class)
                .Select(i => new RelatedSymbolPair(classSymbol, i, DirectedModelRelationshipTypes.Subtype));
        }

        [NotNull]
        [ItemNotNull]
        protected static async Task<IEnumerable<RelatedSymbolPair>> GetImplementingTypesAsync(
            [NotNull] IRoslynModelProvider roslynModelProvider,
            [NotNull] INamedTypeSymbol interfaceSymbol)
        {
            var workspace = await roslynModelProvider.GetWorkspaceAsync();
            var implementingTypes = await FindImplementingTypesAsync(workspace, interfaceSymbol);
            return implementingTypes.Select(i => new RelatedSymbolPair(interfaceSymbol, i, DirectedModelRelationshipTypes.ImplementerType));
        }

        [NotNull]
        [ItemNotNull]
        protected static async Task<IEnumerable<RelatedSymbolPair>> GetDerivedInterfacesAsync(
            [NotNull] IRoslynModelProvider roslynModelProvider,
            [NotNull] INamedTypeSymbol interfaceSymbol)
        {
            var workspace = await roslynModelProvider.GetWorkspaceAsync();
            var derivedInterfaces = await FindDerivedInterfacesAsync(workspace, interfaceSymbol);
            return derivedInterfaces.Select(i => new RelatedSymbolPair(interfaceSymbol, i, DirectedModelRelationshipTypes.Subtype));
        }

        [NotNull]
        [ItemNotNull]
        private static async Task<IEnumerable<INamedTypeSymbol>> FindImplementingTypesAsync(
            [NotNull] Workspace workspace,
            [NotNull] INamedTypeSymbol interfaceSymbol)
        {
            var result = new List<INamedTypeSymbol>();

            var implementerSymbols = await SymbolFinder.FindImplementationsAsync(interfaceSymbol, workspace.CurrentSolution);
            foreach (var namedTypeSymbol in implementerSymbols.OfType<INamedTypeSymbol>())
            {
                var interfaces = namedTypeSymbol.Interfaces.Select(i => i.OriginalDefinition);
                if (interfaces.Any(i => i.SymbolEquals(interfaceSymbol)))
                    result.Add(namedTypeSymbol);
            }

            // For some reason SymbolFinder does not find implementer structs. So we also make a search with a visitor.

            foreach (var compilation in await GetCompilationsAsync(workspace))
            {
                var visitor = new ImplementingTypesFinderVisitor(interfaceSymbol);
                compilation.Assembly?.Accept(visitor);

                result.AddRange(visitor.ImplementingTypeSymbols.Where(i => i.TypeKind == TypeKind.Struct));
            }

            return result;
        }

        [NotNull]
        [ItemNotNull]
        private static async Task<IEnumerable<INamedTypeSymbol>> FindDerivedInterfacesAsync(
            [NotNull] Workspace workspace,
            [NotNull] INamedTypeSymbol interfaceSymbol)
        {
            var result = new List<INamedTypeSymbol>();

            foreach (var compilation in await GetCompilationsAsync(workspace))
            {
                var visitor = new DerivedInterfacesFinderVisitor(interfaceSymbol);
                compilation.Assembly?.Accept(visitor);

                result.AddRange(visitor.DerivedInterfaces);
            }

            return result;
        }
    }
}