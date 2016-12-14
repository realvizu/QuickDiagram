using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;
using Codartis.SoftVis.Util;
using Codartis.SoftVis.Util.Roslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Builds a model based on Roslyn-provided info.
    /// </summary>
    public class RoslynBasedModelBuilder : IModelServices
    {
        private readonly RoslynBasedModel _model;
        private readonly IRoslynModelProvider _roslynModelProvider;
        //private readonly RoslynBasedModelUpdater _roslynBasedModelUpdater;

        private static readonly List<string> TrivialBaseSymbolNames =
            new List<string>
            {
                "System.Object",
                "object"
            };

        internal RoslynBasedModelBuilder(IRoslynModelProvider roslynModelProvider)
        {
            _roslynModelProvider = roslynModelProvider;
            _model = new RoslynBasedModel();

            //var workspace = _roslynModelProvider.GetWorkspace();
            //_roslynBasedModelUpdater = new RoslynBasedModelUpdater(_model, workspace);
        }

        public IReadOnlyModel Model => _model;

        public async Task<IRoslynBasedModelEntity> AddCurrentSymbolAsync()
        {
            var namedTypeSymbol = await _roslynModelProvider.GetCurrentSymbolAsync() as INamedTypeSymbol;
            if (namedTypeSymbol == null)
                return null;

            return GetOrAddEntity(GetOriginalDefinition(namedTypeSymbol));
        }

        public void ExtendModelWithRelatedEntities(IModelEntity modelEntity, EntityRelationType? entityRelationType = null,
            CancellationToken cancellationToken = default(CancellationToken), IIncrementalProgress progress = null, bool recursive = false)
        {
            var roslynBasedModelEntity = modelEntity as RoslynBasedModelEntity;
            if (roslynBasedModelEntity == null)
                return;

            var symbolRelations = roslynBasedModelEntity
                .FindRelatedSymbols(_roslynModelProvider, entityRelationType)
                .Select(GetOriginalDefinition)
                .Where(i => !IsHidden(i.RelatedSymbol)).ToList();

            foreach (var symbolRelation in symbolRelations)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var relatedEntity = GetOrAddEntity(symbolRelation.RelatedSymbol, progress);
                AddRelationshipIfNotExists(symbolRelation);

                // TODO: loop detection?
                if (recursive)
                    ExtendModelWithRelatedEntities(relatedEntity, entityRelationType, cancellationToken, progress, recursive: true);
            }
        }

        public bool HasSource(IModelEntity modelEntity)
        {
            var roslyBasedModelEntity = modelEntity as IRoslynBasedModelEntity;
            if (roslyBasedModelEntity == null)
                return false;

            return _roslynModelProvider.HasSource(roslyBasedModelEntity.RoslynSymbol);
        }

        public void ShowSource(IModelEntity modelEntity)
        {
            var roslyBasedModelEntity = modelEntity as IRoslynBasedModelEntity;
            if (roslyBasedModelEntity == null)
                return;

            _roslynModelProvider.ShowSource(roslyBasedModelEntity.RoslynSymbol);
        }

        public void UpdateFromSource(CancellationToken cancellationToken = default(CancellationToken), IIncrementalProgress progress = null)
        {
            UpdateEntitiesFromSource(cancellationToken, progress);
            UpdateRelationshipsFromSource(cancellationToken, progress);
        }

        public void Clear()
        {
            _model.Clear();
        }

        private void UpdateEntitiesFromSource(CancellationToken cancellationToken, IIncrementalProgress progress)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var workspace = _roslynModelProvider.GetWorkspace();
            var compilations = workspace.CurrentSolution.Projects.Select(i => i.GetCompilationAsync(cancellationToken))
                .Select(i => i.Result).ToArray();

            foreach (var roslynBasedModelEntity in _model.Entities.OfType<RoslynBasedModelEntity>().ToArray())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var namedTypeSymbol = roslynBasedModelEntity.RoslynSymbol;
                var newVersionOfSymbol = FindSymbolInCompilations(namedTypeSymbol, compilations, cancellationToken);

                if (newVersionOfSymbol == null)
                    _model.RemoveEntity(roslynBasedModelEntity);
                else
                    _model.UpdateEntity(roslynBasedModelEntity, newVersionOfSymbol);

                progress?.Report(1);
            }
        }

        private static INamedTypeSymbol FindSymbolInCompilations(INamedTypeSymbol namedTypeSymbol, IEnumerable<Compilation> compilations,
            CancellationToken cancellationToken)
        {
            var compilationArray = compilations as Compilation[] ?? compilations.ToArray();

            return FindSymbolInCompilationsByName(namedTypeSymbol, compilationArray, cancellationToken);

            // TODO: find better way to detect type rename. Location-based detection is too fragile.
            // ?? FindSymbolInCompilationsByLocation(namedTypeSymbol, compilationArray);
        }

        private static INamedTypeSymbol FindSymbolInCompilationsByName(INamedTypeSymbol namedTypeSymbol, Compilation[] compilationArray, CancellationToken cancellationToken)
        {
            var symbolMatchByName = compilationArray.SelectMany(i => SymbolFinder.FindSimilarSymbols(namedTypeSymbol, i, cancellationToken))
                .Where(i => i.TypeKind == namedTypeSymbol.TypeKind)
                .OrderByDescending(i => i.Locations.Any(j => j.IsInSource))
                .FirstOrDefault();

            return symbolMatchByName;
        }

        private static INamedTypeSymbol FindSymbolInCompilationsByLocation(INamedTypeSymbol namedTypeSymbol, Compilation[] compilationArray)
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

            var allSymbolRelations = _model.Entities.OfType<RoslynBasedModelEntity>()
                .SelectMany(i =>
                {
                    progress?.Report(1);
                    return i.FindRelatedSymbols(_roslynModelProvider);
                })
                .Distinct().ToArray();

            foreach (var relationship in _model.Relationships.OfType<ModelRelationship>().ToArray())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (allSymbolRelations.All(i => !i.Matches(relationship)))
                    _model.RemoveRelationship(relationship);

                progress?.Report(1);
            }
        }

        private static RoslynSymbolRelation GetOriginalDefinition(RoslynSymbolRelation symbolRelation)
        {
            return symbolRelation.WithRelatedSymbol(GetOriginalDefinition(symbolRelation.RelatedSymbol));
        }

        private static INamedTypeSymbol GetOriginalDefinition(INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol.OriginalDefinition ?? namedTypeSymbol;
        }

        private IRoslynBasedModelEntity GetOrAddEntity(INamedTypeSymbol namedTypeSymbol, IIncrementalProgress progress = null)
        {
            var modelEntity = _model.GetOrAddEntity(i => i.RoslynSymbol.SymbolEquals(namedTypeSymbol), () => CreateModelEntity(namedTypeSymbol));
            progress?.Report(1);
            return modelEntity;
        }

        private ModelRelationship AddRelationshipIfNotExists(RoslynSymbolRelation symbolRelation)
        {
            var sourceEntity = _model.GetEntityBySymbol(symbolRelation.SourceSymbol);
            var targetEntity = _model.GetEntityBySymbol(symbolRelation.TargetSymbol);
            var relationship = new ModelRelationship(sourceEntity, targetEntity, symbolRelation.Type);
            return _model.GetOrAddRelationship(relationship);
        }

        private static RoslynBasedModelEntity CreateModelEntity(INamedTypeSymbol namedTypeSymbol)
        {
            switch (namedTypeSymbol.TypeKind)
            {
                case TypeKind.Class:
                    return new RoslynBasedClass(namedTypeSymbol);
                case TypeKind.Interface:
                    return new RoslynBasedInterface(namedTypeSymbol);
                case TypeKind.Struct:
                    return new RoslynBasedStruct(namedTypeSymbol);
                case TypeKind.Enum:
                    return new RoslynBasedEnum(namedTypeSymbol);
                case TypeKind.Delegate:
                    return new RoslynBasedDelegate(namedTypeSymbol);
                default:
                    throw new Exception($"Unexpected TypeKind: {namedTypeSymbol.TypeKind}");
            }
        }

        private static bool IsHidden(INamedTypeSymbol roslynSymbol)
        {
            return GlobalOptions.HideTrivialBaseEntities
                && TrivialBaseSymbolNames.Contains(roslynSymbol.GetFullyQualifiedName());
        }
    }
}