using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Gets a model entity from a Roslyn symbol. Creates the entity if not yet exists in the model.
        /// </summary>
        /// <param name="namedTypeSymbol">A Roslyn symbol.</param>
        /// <returns>The model entity that corresponds to the given Roslyn symbol.</returns>
        public IRoslynBasedModelEntity FindOrCreateModelEntity(INamedTypeSymbol namedTypeSymbol)
        {
            return AddEntityIfNotExists(namedTypeSymbol);
        }

        /// <summary>
        /// Explores related symbols in the Roslyn model and adds them to the model.
        /// </summary>
        /// <param name="modelEntity">The starting model entity.</param>
        /// <param name="relatedEntitySpecification">Optionally specifies what kind of relations should be explored.</param>
        /// <param name="recursive">True means repeat exploring for related entities. Default is false.</param>
        public void ExtendModelWithRelatedEntities(IRoslynBasedModelEntity modelEntity,
            RelatedEntitySpecification? relatedEntitySpecification = null, bool recursive = false)
        {
            var symbolRelations = modelEntity
                .FindRelatedSymbols(_roslynModelProvider, relatedEntitySpecification)
                .Where(i => !IsHidden(i.RelatedSymbol));

            foreach (var symbolRelation in symbolRelations)
            {
                var relatedEntity = AddEntityIfNotExists(symbolRelation.RelatedSymbol);
                AddRelationshipIfNotExists(symbolRelation);

                // TODO: loop detection?
                if (recursive)
                    ExtendModelWithRelatedEntities(relatedEntity, relatedEntitySpecification, recursive: true);
            }
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
            return _model.AddRelationshipIfNotExists(sourceEntity, targetEntity, symbolRelation.TypeSpecification);
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