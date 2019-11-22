using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.VisualStudioIntegration.Util;
using Codartis.Util;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Implements Roslyn model operations.
    /// Wraps a general-purpose IModelService.
    /// </summary>
    internal sealed class RoslynModelService : IRoslynModelService
    {
        [NotNull]
        [ItemNotNull]
        private static readonly List<string> TrivialTypeNames =
            new List<string>
            {
                "System.Object",
                "object"
            };

        [NotNull] private readonly IModelService _modelService;

        public bool ExcludeTrivialTypes { get; set; }

        public RoslynModelService([NotNull] IModelService modelService)
        {
            _modelService = modelService;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IModelNode AddSymbol(ISymbol symbol)
        {
            return _modelService.LatestModel.TryGetNodeByPayload(symbol).Match(
                some => some,
                () => _modelService.AddNode(symbol.GetName(), symbol.GetStereotype(), symbol)
            );
        }

        //public async Task<Maybe<IModelNode>> AddCurrentSymbolAsync()
        //{
        //    return (await TryGetCurrentSymbolAsync())
        //        .Select(i => GetOrAddNode(GetOriginalDefinition(i)));
        //}

        //public async Task ExtendModelWithRelatedNodesAsync(
        //    IModelNode node,
        //    DirectedModelRelationshipType? directedModelRelationshipType = null,
        //    CancellationToken cancellationToken = default,
        //    IIncrementalProgress progress = null,
        //    bool recursive = false)
        //{
        //    await ExtendModelWithRelatedNodesRecursiveAsync(
        //        node,
        //        directedModelRelationshipType,
        //        cancellationToken,
        //        progress,
        //        recursive,
        //        new HashSet<ModelNodeId> { node.Id }
        //    );
        //}

        //public async Task UpdateFromSourceAsync(
        //    IEnumerable<ModelNodeId> visibleModelNodeIds,
        //    CancellationToken cancellationToken = default,
        //    IIncrementalProgress progress = null)
        //{
        //    await UpdateEntitiesFromSourceAsync(cancellationToken, progress);
        //    await UpdateRelationshipsFromSourceAsync(cancellationToken, progress);

        //    foreach (var modelNodeId in visibleModelNodeIds)
        //    {
        //        await Model.TryGetNode(modelNodeId)
        //            .MatchAsync(async node => await ExtendModelWithRelatedNodesAsync(node, null, cancellationToken, progress, recursive: false));
        //    }
        //}

        //private async Task UpdateEntitiesFromSourceAsync(CancellationToken cancellationToken, IIncrementalProgress progress)
        //{
        //    cancellationToken.ThrowIfCancellationRequested();

        //    var workspace = await _hostModelProvider.GetWorkspaceAsync();
        //    var projects = workspace.CurrentSolution.Projects;
        //    var compilations = (await projects.SelectAsync(async i => await i.GetCompilationAsync(cancellationToken))).ToArray();

        //    foreach (var roslynModelNode in Model.GetRoslynNodes())
        //    {
        //        cancellationToken.ThrowIfCancellationRequested();

        //        var namedTypeSymbol = roslynModelNode.RoslynSymbol;
        //        var newVersionOfSymbol = FindSymbolInCompilations(namedTypeSymbol, compilations, cancellationToken);

        //        if (newVersionOfSymbol == null)
        //            RemoveNode(roslynModelNode.Id);
        //        else
        //        {
        //            var updatedNode = roslynModelNode.UpdateRoslynSymbol(newVersionOfSymbol);
        //            UpdateNode(updatedNode);
        //        }

        //        progress?.Report(1);
        //    }
        //}

        private static ISymbol FindSymbolInCompilations(
            ISymbol namedTypeSymbol,
            IEnumerable<Compilation> compilations,
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

        //private async Task UpdateRelationshipsFromSourceAsync(CancellationToken cancellationToken, IIncrementalProgress progress)
        //{
        //    cancellationToken.ThrowIfCancellationRequested();

        //    var allSymbolRelations = await Model.GetRoslynNodes().SelectManyAsync(
        //        async i =>
        //        {
        //            var relatedSymbolPairs = await i.FindRelatedSymbolsAsync(_hostModelProvider);
        //            progress?.Report(1);
        //            return relatedSymbolPairs;
        //        });
        //    var distinctSymbolRelations = allSymbolRelations.Distinct().ToArray();

        //    foreach (var relationship in Model.Relationships)
        //    {
        //        cancellationToken.ThrowIfCancellationRequested();

        //        if (distinctSymbolRelations.All(i => !i.Matches(relationship)))
        //            RemoveRelationship(relationship.Id);

        //        progress?.Report(1);
        //    }
        //}

        private static RelatedSymbolPair GetOriginalDefinition(RelatedSymbolPair symbolPair)
        {
            return symbolPair.WithRelatedSymbol(GetOriginalDefinition(symbolPair.RelatedSymbol));
        }

        private static ISymbol GetOriginalDefinition(ISymbol symbol)
        {
            return symbol.OriginalDefinition ?? symbol;
        }

        //[NotNull]
        //private IModelNode GetOrAddNode([NotNull] ISymbol symbol)
        //{
        //    var node = Model.GetNodeBySymbol(symbol);
        //    if (node != null)
        //        return node;

        //    var newNode = RoslynModelItemFactory.CreateModelNode(symbol);
        //    AddNode(newNode);
        //    return newNode;
        //}

        //private void AddRelationshipIfNotExists(RelatedSymbolPair relatedSymbolPair)
        //{
        //    var sourceNode = Model.GetNodeBySymbol(relatedSymbolPair.SourceSymbol);
        //    var targetNode = Model.GetNodeBySymbol(relatedSymbolPair.TargetSymbol);

        //    if (Model.RelationshipExists(sourceNode, targetNode, relatedSymbolPair.Stereotype))
        //        return;

        //    var newRelationship = RoslynModelItemFactory.CreateRoslynRelationship(sourceNode, targetNode, relatedSymbolPair.Stereotype);
        //    AddRelationship(newRelationship);
        //}

        private bool IsHidden(ISymbol roslynSymbol)
        {
            return ExcludeTrivialTypes && TrivialTypeNames.Contains(roslynSymbol.GetFullyQualifiedName());
        }

        //private async Task ExtendModelWithRelatedNodesRecursiveAsync(
        //    [NotNull] IModelNode node,
        //    DirectedModelRelationshipType? directedModelRelationshipType,
        //    CancellationToken cancellationToken,
        //    IIncrementalProgress progress,
        //    bool recursive,
        //    [NotNull] ISet<ModelNodeId> alreadyDiscoveredNodes)
        //{
        //    var roslynNode = GetRoslynItem(node);
        //    var relatedSymbolPairs = await roslynNode.FindRelatedSymbolsAsync(_hostModelProvider, directedModelRelationshipType);

        //    var presentableRelatedSymbolPairs = relatedSymbolPairs
        //        .Select(GetOriginalDefinition)
        //        .Where(i => !IsHidden(i.RelatedSymbol))
        //        .ToList();

        //    foreach (var relatedSymbolPair in presentableRelatedSymbolPairs)
        //    {
        //        cancellationToken.ThrowIfCancellationRequested();

        //        var relatedSymbol = relatedSymbolPair.RelatedSymbol;
        //        var relatedNode = GetOrAddNode(relatedSymbol);
        //        AddRelationshipIfNotExists(relatedSymbolPair);

        //        progress?.Report(1);

        //        if (!recursive)
        //            continue;

        //        // Avoid infinite loop by stopping recursion when a node is already added.
        //        if (alreadyDiscoveredNodes.Contains(relatedNode.Id))
        //            continue;

        //        alreadyDiscoveredNodes.Add(relatedNode.Id);

        //        await ExtendModelWithRelatedNodesRecursiveAsync(
        //            relatedNode,
        //            directedModelRelationshipType,
        //            cancellationToken,
        //            progress,
        //            true,
        //            alreadyDiscoveredNodes);
        //    }
        //}

        //private static RoslynSymbol GetRoslynItem(IModelNode node)
        //{
        //    return (RoslynSymbol)node.PayloadUi;
        //}
    }
}