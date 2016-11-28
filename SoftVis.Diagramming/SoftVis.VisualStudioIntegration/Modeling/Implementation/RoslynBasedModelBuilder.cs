using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Builds a model based on Roslyn-provided info.
    /// </summary>
    public class RoslynBasedModelBuilder : IModelServices
    {
        private readonly RoslynBasedModel _model;
        private readonly IRoslynModelProvider _roslynModelProvider;

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
        }

        public IReadOnlyModel Model => _model;

        public async Task<IRoslynBasedModelEntity> AddCurrentSymbolAsync()
        {
            var namedTypeSymbol = await _roslynModelProvider.GetCurrentSymbolAsync() as INamedTypeSymbol;
            if (namedTypeSymbol == null)
                return null;

            return AddEntityIfNotExists(GetOriginalDefinition(namedTypeSymbol));
        }

        public void ExtendModelWithRelatedEntities(IModelEntity modelEntity,
            EntityRelationType? entityRelationType = null, bool recursive = false)
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
                var relatedEntity = AddEntityIfNotExists(symbolRelation.RelatedSymbol);
                AddRelationshipIfNotExists(symbolRelation);

                // TODO: loop detection?
                if (recursive)
                    ExtendModelWithRelatedEntities(relatedEntity, entityRelationType, recursive: true);
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

        private static RoslynSymbolRelation GetOriginalDefinition(RoslynSymbolRelation symbolRelation)
        {
            return symbolRelation.WithRelatedSymbol(GetOriginalDefinition(symbolRelation.RelatedSymbol));
        }

        private static INamedTypeSymbol GetOriginalDefinition(INamedTypeSymbol namedTypeSymbol)
        {
            return namedTypeSymbol.OriginalDefinition ?? namedTypeSymbol;
        }

        private RoslynBasedModelEntity AddEntityIfNotExists(INamedTypeSymbol namedTypeSymbol)
        {
            var modelEntity = _model.GetModelEntity(namedTypeSymbol);
            if (modelEntity == null)
            {
                modelEntity = CreateModelEntity(namedTypeSymbol);
                _model.AddEntity(modelEntity);
            }
            return modelEntity;
        }

        private ModelRelationship AddRelationshipIfNotExists(RoslynSymbolRelation symbolRelation)
        {
            var sourceEntity = _model.GetModelEntity(symbolRelation.SourceSymbol);
            var targetEntity = _model.GetModelEntity(symbolRelation.TargetSymbol);
            return _model.AddRelationshipIfNotExists(sourceEntity, targetEntity, symbolRelation.Type);
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