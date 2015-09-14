using System.Collections.Generic;
using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// A model created from Roslyn symbols.
    /// </summary>
    internal class RoslynBasedModel : IModel
    {
        private readonly Dictionary<string, RoslynBasedClass> _roslynSymbolNameToModelEntityMap;
        private readonly List<IModelRelationship> _relationships;

        internal RoslynBasedModel()
        {
            _roslynSymbolNameToModelEntityMap = new Dictionary<string, RoslynBasedClass>();
            _relationships = new List<IModelRelationship>();
        }

        public IEnumerable<IModelEntity> Entities => _roslynSymbolNameToModelEntityMap.Values;
        public IEnumerable<IModelRelationship> Relationships => _relationships;

        public RoslynBasedClass GetOrAdd(INamedTypeSymbol namedTypeSymbol)
        {
            var element = GetModelElement(namedTypeSymbol);
            if (element == null)
            {
                element = CreateModelElement(namedTypeSymbol);
                AddModelElement(namedTypeSymbol, element);
            }

            return element;
        }

        private RoslynBasedClass GetModelElement(INamedTypeSymbol namedTypeSymbol)
        {
            var fullyQualifiedName = namedTypeSymbol.GetFullyQualifiedName();

            return _roslynSymbolNameToModelEntityMap.ContainsKey(fullyQualifiedName)
                ? _roslynSymbolNameToModelEntityMap[fullyQualifiedName]
                : null;
        }

        private RoslynBasedClass CreateModelElement(INamedTypeSymbol namedTypeSymbol)
        {
            RoslynBasedClass baseClass = null;

            var baseTypeSymbol = namedTypeSymbol.BaseType;
            if (baseTypeSymbol != null)
                baseClass = GetOrAdd(baseTypeSymbol);

            return CreateClassWithBaseRelationship(namedTypeSymbol, baseClass);
        }

        private RoslynBasedClass CreateClassWithBaseRelationship(INamedTypeSymbol namedTypeSymbol, RoslynBasedClass baseClass)
        {
            var newClass = new RoslynBasedClass(namedTypeSymbol);

            if (baseClass != null)
            {
                var newRelationship = new ModelRelationship(newClass, baseClass, ModelRelationshipType.Generalization);
                newClass.AddOutgoingRelationship(newRelationship);
                baseClass.AddIncomingRelationship(newRelationship);
                _relationships.Add(newRelationship);
            }

            return newClass;
        }

        private void AddModelElement(INamedTypeSymbol namedTypeSymbol, RoslynBasedClass modelElement)
        {
            _roslynSymbolNameToModelEntityMap.Add(namedTypeSymbol.GetFullyQualifiedName(), modelElement);
        }
    }
}
