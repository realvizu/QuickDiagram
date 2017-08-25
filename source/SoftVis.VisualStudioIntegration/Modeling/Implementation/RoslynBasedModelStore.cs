using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.VisualStudioIntegration.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Builds a model based on Roslyn-provided info.
    /// </summary>
    internal class RoslynBasedModelStore : ModelStore, IModelServices
    {
        private readonly IRoslynModelProvider _roslynModelProvider;
        //private readonly RoslynBasedModelUpdater _roslynBasedModelUpdater;

        private static readonly List<string> TrivialBaseSymbolNames =
            new List<string>
            {
                "System.Object",
                "object"
            };

        internal RoslynBasedModelStore(IRoslynModelProvider roslynModelProvider)
            : base(new RoslynBasedModel())
        {
            _roslynModelProvider = roslynModelProvider;

            //var workspace = _roslynModelProvider.GetWorkspace();
            //_roslynBasedModelUpdater = new RoslynBasedModelUpdater(_model, workspace);
        }

        private RoslynBasedModel CurrentRoslynModel => CurrentModel as RoslynBasedModel;

        public async Task<bool> IsCurrentSymbolAvailableAsync() => await GetCurrentSymbolAsync() != null;

        public async Task<IRoslynModelNode> AddCurrentSymbolAsync()
        {
            var namedTypeSymbol = await GetCurrentSymbolAsync();
            return namedTypeSymbol == null
                ? null
                : GetOrAddNode(GetOriginalDefinition(namedTypeSymbol));
        }

        public void ExtendModelWithRelatedEntities(IModelNode modelNode, DirectedModelRelationshipType? directedModelRelationshipType = null,
            CancellationToken cancellationToken = default(CancellationToken), IIncrementalProgress progress = null, bool recursive = false)
        {
            var roslynModelNode = modelNode as IRoslynModelNode;
            if (roslynModelNode == null)
                return;

            ExtendModelWithRelatedNodesRecursive(roslynModelNode, directedModelRelationshipType, cancellationToken, progress, recursive);
        }

        private void ExtendModelWithRelatedNodesRecursive(IRoslynModelNode roslynModelNode, DirectedModelRelationshipType? directedModelRelationshipType,
            CancellationToken cancellationToken, IIncrementalProgress progress, bool recursive)
        {
            var relatedSymbolPairs = roslynModelNode
                .FindRelatedSymbols(_roslynModelProvider, directedModelRelationshipType)
                .Select(GetOriginalDefinition)
                .Where(i => !IsHidden(i.RelatedSymbol)).ToList();

            foreach (var relatedSymbolPair in relatedSymbolPairs)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var relatedSymbol = relatedSymbolPair.RelatedSymbol;

                // Avoid infinite loop by stopping recursion when a node is already added.
                if (CurrentRoslynModel.GetNodeBySymbol(relatedSymbol) != null)
                    recursive = false;

                var relatedNode = GetOrAddNode(relatedSymbol, progress);
                GetOrAddRelationship(relatedSymbolPair);

                if (recursive)
                    ExtendModelWithRelatedNodesRecursive(relatedNode, directedModelRelationshipType, cancellationToken, progress, recursive: true);
            }
        }

        public bool HasSource(IModelNode modelNode)
        {
            var roslyBasedModelEntity = modelNode as IRoslynModelNode;
            if (roslyBasedModelEntity == null)
                return false;

            return _roslynModelProvider.HasSource(roslyBasedModelEntity.RoslynSymbol);
        }

        public void ShowSource(IModelNode modelNode)
        {
            var roslyBasedModelEntity = modelNode as IRoslynModelNode;
            if (roslyBasedModelEntity == null)
                return;

            _roslynModelProvider.ShowSource(roslyBasedModelEntity.RoslynSymbol);
        }

        public void UpdateFromSource(CancellationToken cancellationToken = default(CancellationToken), IIncrementalProgress progress = null)
        {
            UpdateEntitiesFromSource(cancellationToken, progress);
            UpdateRelationshipsFromSource(cancellationToken, progress);
        }

        public new void ClearModel() => base.ClearModel();

        private async Task<INamedTypeSymbol> GetCurrentSymbolAsync()
        {
            return await _roslynModelProvider.GetCurrentSymbolAsync() as INamedTypeSymbol;
        }

        private void UpdateEntitiesFromSource(CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var workspace = _roslynModelProvider.GetWorkspace();
            var compilations = workspace.CurrentSolution.Projects.Select(i => i.GetCompilationAsync(cancellationToken))
                .Select(i => i.Result).ToArray();

            foreach (var roslynTypeNode in CurrentRoslynModel.RoslynModelNodes)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var namedTypeSymbol = roslynTypeNode.RoslynSymbol;
                var newVersionOfSymbol = FindSymbolInCompilations(namedTypeSymbol, compilations, cancellationToken);

                if (newVersionOfSymbol == null)
                    RemoveNode(roslynTypeNode);
                else
                {
                    var updatedNode = RoslynModelItemFactory.CreateModelNode(newVersionOfSymbol, roslynTypeNode.Id);
                    UpdateNode(roslynTypeNode, updatedNode);
                }

                progress?.Report(1);
            }
        }

        private static ISymbol FindSymbolInCompilations(ISymbol namedTypeSymbol, IEnumerable<Compilation> compilations,
            CancellationToken cancellationToken)
        {
            var compilationArray = compilations as Compilation[] ?? compilations.ToArray();

            return FindSymbolInCompilationsByName(namedTypeSymbol, compilationArray, cancellationToken);

            // TODO: find better way to detect type rename. Location-based detection is too fragile.
            // ?? FindSymbolInCompilationsByLocation(symbol, compilationArray);
        }

        private static ISymbol FindSymbolInCompilationsByName(ISymbol namedTypeSymbol, Compilation[] compilationArray, CancellationToken cancellationToken)
        {
            var symbolMatchByName = compilationArray.SelectMany(i => SymbolFinder.FindSimilarSymbols(namedTypeSymbol, i, cancellationToken))
                .Where(i => i.GetType() == namedTypeSymbol.GetType())
                .OrderByDescending(i => i.Locations.Any(j => j.IsInSource))
                .FirstOrDefault();

            return symbolMatchByName;
        }

        private static ISymbol FindSymbolInCompilationsByLocation(ISymbol namedTypeSymbol, Compilation[] compilationArray)
        {
            var syntaxReference = namedTypeSymbol.DeclaringSyntaxReferences.FirstOrDefault();
            if (syntaxReference == null)
                return null;

            var compilation = compilationArray.FirstOrDefault(i => i.SyntaxTrees.Any(j => j.FilePath == syntaxReference.SyntaxTree.FilePath));
            var newSyntaxTree = compilation?.SyntaxTrees.FirstOrDefault(i => i.FilePath == syntaxReference.SyntaxTree.FilePath);
            var typeDeclarationSyntax = newSyntaxTree?.GetRoot().DescendantNodes(syntaxReference.Span).OfType<TypeDeclarationSyntax>().LastOrDefault();
            if (typeDeclarationSyntax == null)
                return null;

            var symbolMatchByLocation = compilation.GetSemanticModel(newSyntaxTree).GetDeclaredSymbol(typeDeclarationSyntax) as INamedTypeSymbol;
            return symbolMatchByLocation;
        }

        private void UpdateRelationshipsFromSource(CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var allSymbolRelations = CurrentRoslynModel.RoslynModelNodes
                .SelectMany(i =>
                {
                    progress?.Report(1);
                    return i.FindRelatedSymbols(_roslynModelProvider);
                })
                .Distinct().ToArray();

            foreach (var relationship in CurrentRoslynModel.RoslynRelationships)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (allSymbolRelations.All(i => !i.Matches(relationship)))
                    RemoveRelationship(relationship);

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

        private IModelRelationship GetOrAddRelationship(RelatedSymbolPair relatedSymbolPair)
        {
            var sourceNode = CurrentRoslynModel.GetNodeBySymbol(relatedSymbolPair.SourceSymbol);
            var targetNode = CurrentRoslynModel.GetNodeBySymbol(relatedSymbolPair.TargetSymbol);

            var relationship = CurrentRoslynModel.GetRelationship(sourceNode, targetNode, relatedSymbolPair.Stereotype);
            if (relationship != null)
                return relationship;

            var newRelationship = RoslynModelItemFactory.CreateRoslynRelationship(sourceNode, targetNode, relatedSymbolPair.Stereotype);
            AddRelationship(newRelationship);
            return newRelationship;
        }

        private static bool IsHidden(ISymbol roslynSymbol)
        {
            return GlobalOptions.HideTrivialBaseNodes
                && TrivialBaseSymbolNames.Contains(roslynSymbol.GetFullyQualifiedName());
        }
    }
}