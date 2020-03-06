using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CellWars.Threading;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Wraps a model service with Roslyn-specific operations.
    /// </summary>
    internal sealed class RoslynBasedModelService : IRoslynBasedModelService
    {
        [NotNull]
        [ItemNotNull]
        private static readonly List<string> TrivialTypeNames =
            new List<string>
            {
                "bool", "System.Boolean",
                "byte", "System.Byte",
                "sbyte", "System.SByte",
                "char", "System.Char",
                "decimal", "System.Decimal",
                "double", "System.Double",
                "float", "System.Single",
                "int", "System.Int32",
                "uint", "System.UInt32",
                "long", "System.Int64",
                "ulong", "System.UInt64",
                "short", "System.Int16",
                "ushort", "System.UInt16",
                "object", "System.Object",
                "string", "System.String"
            };

        [NotNull]
        private static readonly SymbolKind[] ModeledSymbolKinds =
        {
            SymbolKind.NamedType,
            SymbolKind.Field,
            SymbolKind.Method,
            SymbolKind.Property
        };

        [NotNull]
        private static readonly TypeKind[] ModeledTypeKinds =
        {
            TypeKind.Class,
            TypeKind.Delegate,
            TypeKind.Enum,
            TypeKind.Interface,
            TypeKind.Struct
        };

        [NotNull] private readonly IModelService _modelService;
        [NotNull] private readonly IRelatedSymbolProvider _relatedSymbolProvider;
        [NotNull] private readonly IEqualityComparer<ISymbol> _symbolEqualityComparer;
        [NotNull] private readonly IRoslynWorkspaceProvider _roslynWorkspaceProvider;
        [NotNull] private readonly AsyncLock _asyncLock;

        public bool ExcludeTrivialTypes { get; set; }

        public RoslynBasedModelService(
            [NotNull] IModelService modelService,
            [NotNull] IRelatedSymbolProvider relatedSymbolProvider,
            [NotNull] IEqualityComparer<ISymbol> symbolEqualityComparer,
            [NotNull] IRoslynWorkspaceProvider roslynWorkspaceProvider)
        {
            _modelService = modelService;
            _relatedSymbolProvider = relatedSymbolProvider;
            _symbolEqualityComparer = symbolEqualityComparer;
            _roslynWorkspaceProvider = roslynWorkspaceProvider;
            _asyncLock = new AsyncLock();
        }

        public IModel LatestModel => _modelService.LatestModel;

        public bool IsModeledSymbol(ISymbol symbol)
        {
            return symbol.Kind.In(ModeledSymbolKinds) &&
                   (symbol is INamedTypeSymbol).Implies(() => ((INamedTypeSymbol)symbol).TypeKind.In(ModeledTypeKinds)) &&
                   !IsHidden(symbol);
        }

        public ISymbol GetSymbol(IModelNode modelNode)
        {
            return (ISymbol)modelNode.Payload;
        }

        public Maybe<IModelNode> TryGetOrAddNode(ISymbol symbol)
        {
            return IsModeledSymbol(symbol)
                ? Maybe.Create(GetOrAddNode(symbol))
                : Maybe<IModelNode>.Nothing;
        }

        [NotNull]
        private IModelNode GetOrAddNode([NotNull] ISymbol symbol)
        {
            using (_asyncLock.Lock())
            {
                return _modelService.LatestModel.TryGetNodeByPayload(symbol).Match(
                    some => some,
                    () => _modelService.AddNode(symbol.GetName(), symbol.GetStereotype(), symbol));
            }
        }

        public IModelRelationship GetOrAddRelationship(RelatedSymbolPair relatedSymbolPair)
        {
            using (_asyncLock.Lock())
            {
                return _modelService.LatestModel.TryGetRelationshipByPayload(relatedSymbolPair).Match(
                    some => some,
                    () =>
                    {
                        var sourceNode = GetOrAddNode(relatedSymbolPair.SourceSymbol);
                        var targetNode = GetOrAddNode(relatedSymbolPair.TargetSymbol);
                        return _modelService.AddRelationship(sourceNode.Id, targetNode.Id, relatedSymbolPair.Stereotype, relatedSymbolPair);
                    });
            }
        }

        public async Task ExtendModelWithRelatedNodesAsync(
            IModelNode node,
            DirectedModelRelationshipType? directedModelRelationshipType = null,
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null,
            bool recursive = false)
        {
            using (await _asyncLock.LockAsync(cancellationToken))
            {
                await ExtendModelWithRelatedNodesRecursiveAsync(
                    node,
                    directedModelRelationshipType,
                    cancellationToken,
                    progress,
                    recursive,
                    new HashSet<ModelNodeId> { node.Id });
            }
        }

        public void ClearModel()
        {
            using (_asyncLock.Lock())
            {
                _modelService.ClearModel();
            }
        }

        public async Task UpdateFromSourceAsync(
            CancellationToken cancellationToken = default,
            IIncrementalProgress progress = null)
        {
            using (await _asyncLock.LockAsync(cancellationToken))
            {
                await UpdateEntitiesFromSourceAsync(cancellationToken, progress);
                await UpdateRelationshipsFromSourceAsync(cancellationToken, progress);
            }
        }

        private async Task UpdateEntitiesFromSourceAsync(CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var workspace = await _roslynWorkspaceProvider.GetWorkspaceAsync();
            var projects = workspace.CurrentSolution.Projects;
            var compilations = (await projects.SelectAsync(async i => await i.GetCompilationAsync(cancellationToken))).ToArray();

            foreach (var modelNode in LatestModel.Nodes)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var symbol = GetSymbol(modelNode);
                var newVersionOfSymbol = FindSymbolInCompilations(symbol, compilations, cancellationToken);

                if (newVersionOfSymbol == null)
                    _modelService.RemoveNode(modelNode.Id);
                else
                {
                    if (!ReferenceEquals(symbol, newVersionOfSymbol))
                    {
                        var updatedNode = modelNode.WithPayload(newVersionOfSymbol);
                        _modelService.UpdateNode(updatedNode);
                    }
                }

                progress?.Report(1);
            }
        }

        private ISymbol FindSymbolInCompilations(
            ISymbol namedTypeSymbol,
            IEnumerable<Compilation> compilations,
            CancellationToken cancellationToken)
        {
            var compilationArray = compilations as Compilation[] ?? compilations.ToArray();

            return FindSymbolInCompilationsByName(namedTypeSymbol, compilationArray, cancellationToken);
        }

        private ISymbol FindSymbolInCompilationsByName(ISymbol symbol, Compilation[] compilationArray, CancellationToken cancellationToken)
        {
            var symbolMatchByName = compilationArray.SelectMany(i => SymbolFinder.FindSimilarSymbols(symbol, i, cancellationToken))
                .Where(i => _symbolEqualityComparer.Equals(i, symbol))
                .OrderByDescending(i => i.Locations.Any(j => j.IsInSource))
                .FirstOrDefault();

            return symbolMatchByName;
        }

        private async Task UpdateRelationshipsFromSourceAsync(CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var allSymbolRelations = await LatestModel.Nodes.SelectManyAsync(
                async i =>
                {
                    var relatedSymbolPairs = await _relatedSymbolProvider.GetRelatedSymbolsAsync(GetSymbol(i));
                    progress?.Report(1);
                    return relatedSymbolPairs;
                });
            var distinctSymbolRelations = allSymbolRelations.Distinct().ToArray();

            foreach (var relationship in LatestModel.Relationships)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (distinctSymbolRelations.All(i => !IsSymbolPairMatchingRelationship(i, relationship)))
                    _modelService.RemoveRelationship(relationship.Id);

                progress?.Report(1);
            }
        }

        private bool IsSymbolPairMatchingRelationship(RelatedSymbolPair symbolPair, [NotNull] IModelRelationship relationship)
        {
            var sourceNode = LatestModel.TryGetNode(relationship.Source).Value;
            var targetNode = LatestModel.TryGetNode(relationship.Target).Value;

            return relationship.Stereotype == symbolPair.Stereotype &&
                   _symbolEqualityComparer.Equals((ISymbol)sourceNode.Payload, symbolPair.SourceSymbol) &&
                   _symbolEqualityComparer.Equals((ISymbol)targetNode.Payload, symbolPair.TargetSymbol);
        }

        private static RelatedSymbolPair GetOriginalDefinition(RelatedSymbolPair symbolPair)
        {
            return symbolPair.WithRelatedSymbol(GetOriginalDefinition(symbolPair.RelatedSymbol));
        }

        [NotNull]
        private static ISymbol GetOriginalDefinition([NotNull] ISymbol symbol)
        {
            return symbol.OriginalDefinition ?? symbol;
        }

        private bool IsHidden(ISymbol roslynSymbol)
        {
            return ExcludeTrivialTypes && TrivialTypeNames.Contains(roslynSymbol.GetFullyQualifiedName());
        }

        private async Task ExtendModelWithRelatedNodesRecursiveAsync(
            [NotNull] IModelNode node,
            DirectedModelRelationshipType? directedModelRelationshipType,
            CancellationToken cancellationToken,
            IIncrementalProgress progress,
            bool recursive,
            [NotNull] ISet<ModelNodeId> alreadyDiscoveredNodes)
        {
            var relatedSymbolPairs = await _relatedSymbolProvider.GetRelatedSymbolsAsync((ISymbol)node.Payload, directedModelRelationshipType);

            var presentableRelatedSymbolPairs = relatedSymbolPairs
                .Select(GetOriginalDefinition)
                .Where(i => IsModeledSymbol(i.RelatedSymbol))
                .ToList();

            foreach (var relatedSymbolPair in presentableRelatedSymbolPairs)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var relatedSymbol = relatedSymbolPair.RelatedSymbol;
                var relatedNode = GetOrAddNode(relatedSymbol);
                GetOrAddRelationship(relatedSymbolPair);

                progress?.Report(1);

                if (!recursive)
                    continue;

                // Avoid infinite loop by stopping recursion when a node is already added.
                if (alreadyDiscoveredNodes.Contains(relatedNode.Id))
                    continue;

                alreadyDiscoveredNodes.Add(relatedNode.Id);

                await ExtendModelWithRelatedNodesRecursiveAsync(
                    relatedNode,
                    directedModelRelationshipType,
                    cancellationToken,
                    progress,
                    recursive: true,
                    alreadyDiscoveredNodes);
            }
        }
    }
}