using System.Collections.Generic;
using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// A model created from Roslyn symbols.
    /// </summary>
    public class RoslynBasedModel : IModel
    {
        private readonly Dictionary<string, RoslynBasedClass> _roslynSymbolNameToModelEntityMap;

        public RoslynBasedModel()
        {
            _roslynSymbolNameToModelEntityMap = new Dictionary<string, RoslynBasedClass>();
        }

        public IEnumerable<IModelEntity> Entities=> _roslynSymbolNameToModelEntityMap.Values;

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

            return new RoslynBasedClass(namedTypeSymbol, baseClass);
        }

        private void AddModelElement(INamedTypeSymbol namedTypeSymbol, RoslynBasedClass modelElement)
        {
            _roslynSymbolNameToModelEntityMap.Add(namedTypeSymbol.GetFullyQualifiedName(), modelElement);
        }
    }
}
