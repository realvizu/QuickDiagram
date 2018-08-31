using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.VisualStudioIntegration.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Implements Roslyn model related operations.
    /// </summary>
    internal class RoslynModelService : ModelServiceBase, IRoslynModelService
    {
        private readonly IRoslynModelProvider _roslynModelProvider;

        private static readonly List<string> TrivialBaseSymbolNames =
            new List<string>
            {
                "System.Object",
                "object"
            };

        public bool HideTrivialBaseNodes { get; set; }

        internal RoslynModelService(IRoslynModelProvider roslynModelProvider)
            : base(new ModelStore(new RoslynModel()), null)
        {
            _roslynModelProvider = roslynModelProvider;
        }

        private RoslynModel CurrentRoslynModel => (RoslynModel) Model;

        public async Task<bool> IsCurrentSymbolAvailableAsync() => await GetCurrentSymbolAsync() != null;

        public async Task<IRoslynModelNode> AddCurrentSymbolAsync()
        {
            var namedTypeSymbol = await GetCurrentSymbolAsync();
            return namedTypeSymbol == null
                ? null
                : GetOrAddNode(GetOriginalDefinition(namedTypeSymbol));
        }

        public async Task ExtendModelWithRelatedNodesAsync(
            IModelNode modelNode, 
            DirectedModelRelationshipType? directedModelRelationshipType = null,
            CancellationToken cancellationToken = default(CancellationToken), 
            IIncrementalProgress progress = null, 
            bool recursive = false)
        {
            var roslynModelNode = modelNode as IRoslynModelNode;
            if (roslynModelNode == null)
                return;

            await ExtendModelWithRelatedNodesRecursiveAsync(
                roslynModelNode,
                directedModelRelationshipType,
                cancellationToken,
                progress,
                recursive,
                new HashSet<ModelNodeId> {roslynModelNode.Id}
            );
        }

        public bool HasSource(IRoslynModelNode modelNode) => _roslynModelProvider.HasSource(modelNode.RoslynSymbol);
        public void ShowSource(IRoslynModelNode modelNode) => _roslynModelProvider.ShowSource(modelNode.RoslynSymbol);

        public async Task UpdateFromSourceAsync(IEnumerable<ModelNodeId> visibleModelNodeIds,
            CancellationToken cancellationToken = default(CancellationToken), IIncrementalProgress progress = null)
        {
            await UpdateEntitiesFromSourceAsync(cancellationToken, progress);
            await UpdateRelationshipsFromSourceAsync(cancellationToken, progress);

            foreach (var modelNodeId in visibleModelNodeIds)
            {
                if (Model.TryGetNode(modelNodeId, out var modelNode))
                    await ExtendModelWithRelatedNodesAsync(modelNode, null, cancellationToken, progress, recursive: false);
            }
        }

        private async Task<INamedTypeSymbol> GetCurrentSymbolAsync()
            => await _roslynModelProvider.GetCurrentSymbolAsync() as INamedTypeSymbol;

        private async Task UpdateEntitiesFromSourceAsync(CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var workspace = _roslynModelProvider.GetWorkspace();
            var projects = workspace.CurrentSolution.Projects;
            var compilations = (await projects.SelectAsync(async i => await i.GetCompilationAsync(cancellationToken))).ToArray();

            foreach (var roslynTypeNode in CurrentRoslynModel.RoslynNodes)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var namedTypeSymbol = roslynTypeNode.RoslynSymbol;
                var newVersionOfSymbol = FindSymbolInCompilations(namedTypeSymbol, compilations, cancellationToken);

                if (newVersionOfSymbol == null)
                    RemoveNode(roslynTypeNode.Id);
                else
                {
                    var updatedNode = roslynTypeNode.UpdateRoslynSymbol(newVersionOfSymbol);
                    UpdateNode(updatedNode);
                }

                progress?.Report(1);
            }
        }

        private static ISymbol FindSymbolInCompilations(ISymbol namedTypeSymbol, IEnumerable<Compilation> compilations,
            CancellationToken cancellationToken)
        {
            var compilationArray = compilations as Compilation[] ?? compilations.ToArray();

            return FindSymbolInCompilationsByName(namedTypeSymbol, compilationArray, cancellationToken);
        }

        private static ISymbol FindSymbolInCompilationsByName(ISymbol namedTypeSymbol, Compilation[] compilationArray, CancellationToken cancellationToken)
        {
            var symbolMatchByName = compilationArray.SelectMany(i => SymbolFinder.FindSimilarSymbols(namedTypeSymbol, i, cancellationToken))
                .Where(i => i.GetType() == namedTypeSymbol.GetType())
                .OrderByDescending(i => i.Locations.Any(j => j.IsInSource))
                .FirstOrDefault();

            return symbolMatchByName;
        }

        private async Task UpdateRelationshipsFromSourceAsync(CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var allSymbolRelations = await CurrentRoslynModel.RoslynNodes.SelectManyAsync(async i => 
            {
                var relatedSymbolPairs = await i.FindRelatedSymbolsAsync(_roslynModelProvider);
                progress?.Report(1);
                return relatedSymbolPairs;
            });
            var distinctSymbolRelations = allSymbolRelations.Distinct().ToArray();

            foreach (var relationship in CurrentRoslynModel.Relationships)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (distinctSymbolRelations.All(i => !i.Matches(relationship)))
                    RemoveRelationship(relationship.Id);

                progress?.Report(1);
            }
        }

        private static RelatedSymbolPair GetOriginalDefinition(RelatedSymbolPair symbolPair)
        {
            return symbolPair.WithRelatedSymbol(GetOriginalDefinition(symbolPair.RelatedSymbol));
        }

        private static ISymbol GetOriginalDefinition(ISymbol symbol)
        {
            return symbol.OriginalDefinition ?? symbol;
        }

        private IRoslynModelNode GetOrAddNode(ISymbol symbol, IIncrementalProgress progress = null)
        {
            progress?.Report(1);

            var node = CurrentRoslynModel.GetNodeBySymbol(symbol);
            if (node != null)
                return node;

            var newNode = RoslynModelItemFactory.CreateModelNode(symbol);
            AddNode(newNode);
            return newNode;
        }

        private void AddRelationshipIfNotExists(RelatedSymbolPair relatedSymbolPair)
        {
            var sourceNode = CurrentRoslynModel.GetNodeBySymbol(relatedSymbolPair.SourceSymbol);
            var targetNode = CurrentRoslynModel.GetNodeBySymbol(relatedSymbolPair.TargetSymbol);

            if (CurrentRoslynModel.RelationshipExists(sourceNode, targetNode, relatedSymbolPair.Stereotype))
                return;

            var newRelationship = RoslynModelItemFactory.CreateRoslynRelationship(sourceNode, targetNode, relatedSymbolPair.Stereotype);
            AddRelationship(newRelationship);
        }

        private bool IsHidden(ISymbol roslynSymbol)
        {
            return HideTrivialBaseNodes && TrivialBaseSymbolNames.Contains(roslynSymbol.GetFullyQualifiedName());
        }

        private async Task ExtendModelWithRelatedNodesRecursiveAsync(
            IRoslynModelNode roslynModelNode, 
            DirectedModelRelationshipType? directedModelRelationshipType,
            CancellationToken cancellationToken, 
            IIncrementalProgress progress, 
            bool recursive, 
            HashSet<ModelNodeId> alreadyDiscoveredNodes)
        {
            var relatedSymbolPairs = await roslynModelNode.FindRelatedSymbolsAsync(_roslynModelProvider, directedModelRelationshipType);

            var presentableRelatedSymbolPairs = relatedSymbolPairs
                .Select(GetOriginalDefinition)
                .Where(i => !IsHidden(i.RelatedSymbol))
                .ToList();

            foreach (var relatedSymbolPair in presentableRelatedSymbolPairs)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var relatedSymbol = relatedSymbolPair.RelatedSymbol;
                var relatedNode = GetOrAddNode(relatedSymbol, progress);
                AddRelationshipIfNotExists(relatedSymbolPair);

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
                    true, 
                    alreadyDiscoveredNodes);
            }
        }
    }
}