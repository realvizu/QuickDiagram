using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    public sealed class RelatedSymbolProvider : IRelatedSymbolProvider
    {
        private delegate Task<IEnumerable<RelatedSymbolPair>> SymbolFinderDelegate(ISymbol symbol);

        [NotNull] private readonly IRoslynWorkspaceProvider _roslynWorkspaceProvider;
        [NotNull] private readonly IEqualityComparer<ISymbol> _symbolEqualityComparer;
        [NotNull] private readonly IDictionary<ModelNodeStereotype, IDictionary<DirectedModelRelationshipType, SymbolFinderDelegate>> _symbolFinderMethods;

        public RelatedSymbolProvider(
            [NotNull] IRoslynWorkspaceProvider roslynWorkspaceProvider,
            [NotNull] IEqualityComparer<ISymbol> symbolEqualityComparer)
        {
            _roslynWorkspaceProvider = roslynWorkspaceProvider;
            _symbolEqualityComparer = symbolEqualityComparer;
            _symbolFinderMethods = CreateSymbolFinderMethodsMap();
        }

        [NotNull]
        private IDictionary<ModelNodeStereotype, IDictionary<DirectedModelRelationshipType, SymbolFinderDelegate>> CreateSymbolFinderMethodsMap()
        {
            return new Dictionary<ModelNodeStereotype, IDictionary<DirectedModelRelationshipType, SymbolFinderDelegate>>
            {
                [ModelNodeStereotypes.Class] = new Dictionary<DirectedModelRelationshipType, SymbolFinderDelegate>
                {
                    [DirectedModelRelationshipTypes.BaseType] = GetBaseTypesAsync,
                    [DirectedModelRelationshipTypes.Subtype] = GetDerivedTypesAsync,
                    [DirectedModelRelationshipTypes.ImplementedInterface] = GetImplementedInterfacesAsync,
                },
                [ModelNodeStereotypes.Interface] = new Dictionary<DirectedModelRelationshipType, SymbolFinderDelegate>
                {
                    [DirectedModelRelationshipTypes.BaseType] = GetBaseInterfacesAsync,
                    [DirectedModelRelationshipTypes.Subtype] = GetDerivedInterfacesAsync,
                    [DirectedModelRelationshipTypes.ImplementerType] = GetImplementingTypesAsync,
                },
                [ModelNodeStereotypes.Struct] = new Dictionary<DirectedModelRelationshipType, SymbolFinderDelegate>
                {
                    [DirectedModelRelationshipTypes.ImplementedInterface] = GetImplementedInterfacesAsync,
                },
            };
        }

        public async Task<IEnumerable<RelatedSymbolPair>> GetRelatedSymbolsAsync(
            ISymbol symbol,
            DirectedModelRelationshipType? directedModelRelationshipType = null)
        {
            var result = Enumerable.Empty<RelatedSymbolPair>();

            var stereotype = symbol.GetStereotype();

            if (!_symbolFinderMethods.TryGetValue(stereotype, out var methodsForStereotype))
                return result;

            var methodsToExecute = (directedModelRelationshipType == null
                    ? methodsForStereotype
                    : methodsForStereotype.Where(i => i.Key == directedModelRelationshipType))
                .Select(i => i.Value);

            foreach (var symbolFinderDelegate in methodsToExecute)
            {
                var foundSymbols = await symbolFinderDelegate.Invoke(symbol);
                result = result.Union(foundSymbols);
            }

            return result;
        }

        [NotNull]
        [ItemNotNull]
        private static Task<IEnumerable<RelatedSymbolPair>> GetBaseTypesAsync([NotNull] ISymbol symbol)
        {
            var result = new List<RelatedSymbolPair>();

            var typeSymbol = symbol.Ensure<ITypeSymbol>();
            var baseSymbol = typeSymbol.BaseType;
            if (baseSymbol?.TypeKind == TypeKind.Class)
                result.Add(new RelatedSymbolPair(typeSymbol, baseSymbol, DirectedModelRelationshipTypes.BaseType));

            return Task.FromResult((IEnumerable<RelatedSymbolPair>)result);
        }

        [NotNull]
        [ItemNotNull]
        private async Task<IEnumerable<RelatedSymbolPair>> GetDerivedTypesAsync([NotNull] ISymbol symbol)
        {
            var classSymbol = symbol.Ensure<INamedTypeSymbol>();

            var solution = await GetCurrentSolutionAsync();

            var derivedClasses = await SymbolFinder.FindDerivedClassesAsync(classSymbol, solution);

            return derivedClasses
                .Where(i => _symbolEqualityComparer.Equals(classSymbol, i.BaseType.OriginalDefinition) && i.TypeKind == TypeKind.Class)
                .Select(i => new RelatedSymbolPair(classSymbol, i, DirectedModelRelationshipTypes.Subtype));
        }

        [NotNull]
        private Task<IEnumerable<RelatedSymbolPair>> GetBaseInterfacesAsync([NotNull] ISymbol symbol)
        {
            var namedTypeSymbol = symbol.Ensure<INamedTypeSymbol>();

            var result = namedTypeSymbol.Interfaces
                .Where(i => i.TypeKind == TypeKind.Interface)
                .Select(i => new RelatedSymbolPair(namedTypeSymbol, i, DirectedModelRelationshipTypes.BaseType));

            return Task.FromResult(result);
        }

        [NotNull]
        [ItemNotNull]
        private Task<IEnumerable<RelatedSymbolPair>> GetImplementedInterfacesAsync([NotNull] ISymbol symbol)
        {
            var namedTypeSymbol = symbol.Ensure<INamedTypeSymbol>();

            var result = namedTypeSymbol.Interfaces.Where(i => i.TypeKind == TypeKind.Interface)
                .Select(i => new RelatedSymbolPair(namedTypeSymbol, i, DirectedModelRelationshipTypes.ImplementedInterface));

            return Task.FromResult(result);
        }

        [NotNull]
        [ItemNotNull]
        private async Task<IEnumerable<RelatedSymbolPair>> GetImplementingTypesAsync([NotNull] ISymbol symbol)
        {
            var namedTypeSymbol = symbol.Ensure<INamedTypeSymbol>();
            var implementingTypes = await FindImplementingTypesAsync(namedTypeSymbol);
            return implementingTypes.Select(i => new RelatedSymbolPair(namedTypeSymbol, i, DirectedModelRelationshipTypes.ImplementerType));
        }

        [NotNull]
        [ItemNotNull]
        private async Task<IEnumerable<RelatedSymbolPair>> GetDerivedInterfacesAsync([NotNull] ISymbol symbol)
        {
            var namedTypeSymbol = symbol.Ensure<INamedTypeSymbol>();
            var derivedInterfaces = await FindDerivedInterfacesAsync(namedTypeSymbol);
            return derivedInterfaces.Select(i => new RelatedSymbolPair(namedTypeSymbol, i, DirectedModelRelationshipTypes.Subtype));
        }

        [NotNull]
        [ItemNotNull]
        private async Task<IEnumerable<INamedTypeSymbol>> FindImplementingTypesAsync([NotNull] INamedTypeSymbol interfaceSymbol)
        {
            var result = new List<INamedTypeSymbol>();

            var solution = await GetCurrentSolutionAsync();

            var implementerSymbols = await SymbolFinder.FindImplementationsAsync(interfaceSymbol, solution);
            foreach (var namedTypeSymbol in implementerSymbols.OfType<INamedTypeSymbol>())
            {
                var interfaces = namedTypeSymbol.Interfaces.Select(i => i.OriginalDefinition);
                if (interfaces.Any(i => _symbolEqualityComparer.Equals(i, interfaceSymbol)))
                    result.Add(namedTypeSymbol);
            }

            // For some reason SymbolFinder does not find implementer structs. So we also make a search with a visitor.
            foreach (var compilation in await GetCompilationsAsync())
            {
                var visitor = new ImplementingTypesFinderVisitor(interfaceSymbol);
                compilation.Assembly?.Accept(visitor);

                result.AddRange(visitor.ImplementingTypeSymbols.Where(i => i.TypeKind == TypeKind.Struct));
            }

            return result;
        }

        [NotNull]
        [ItemNotNull]
        private async Task<IEnumerable<INamedTypeSymbol>> FindDerivedInterfacesAsync([NotNull] ISymbol symbol)
        {
            var namedTypeSymbol = symbol.Ensure<INamedTypeSymbol>();

            var result = new List<INamedTypeSymbol>();

            foreach (var compilation in await GetCompilationsAsync())
            {
                var visitor = new DerivedInterfacesFinderVisitor(namedTypeSymbol, _symbolEqualityComparer);
                compilation.Assembly?.Accept(visitor);

                result.AddRange(visitor.DerivedInterfaces);
            }

            return result;
        }

        [NotNull]
        [ItemNotNull]
        private async Task<IEnumerable<Compilation>> GetCompilationsAsync()
        {
            var currentSolution = await GetCurrentSolutionAsync();
            var compilations = await currentSolution?.Projects?.Where(i => i != null)
                .SelectAsync(async i => await i.GetCompilationAsync());
            return compilations?.EmptyIfNull().WhereNotNull();
        }

        [NotNull]
        [ItemCanBeNull]
        private async Task<Solution> GetCurrentSolutionAsync()
        {
            var workspace = await _roslynWorkspaceProvider.GetWorkspaceAsync();
            return workspace?.CurrentSolution;
        }
    }
}