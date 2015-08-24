using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Codartis.SoftVis.VisualStudioIntegration.RoslynBasedModel
{
    public class RoslynBasedUmlModel
    {
        private UmlModel _umlModel;
        private Dictionary<string, RoslynBasedUmlClass> _roslynSymbolNameToModelElementMap;

        public RoslynBasedUmlModel()
        {
            _umlModel = new UmlModel();
            _roslynSymbolNameToModelElementMap = new Dictionary<string, RoslynBasedUmlClass>();
        }

        public RoslynBasedUmlClass GetOrAdd(INamedTypeSymbol namedTypeSymbol)
        {
            var element = GetModelElement(namedTypeSymbol);
            if (element == null)
            {
                element = CreateModelElement(namedTypeSymbol);
                AddModelElement(namedTypeSymbol, element);
            }

            return element;
        }

        private RoslynBasedUmlClass GetModelElement(INamedTypeSymbol namedTypeSymbol)
        {
            var fullyQualifiedName = namedTypeSymbol.GetFullyQualifiedName();
            if (_roslynSymbolNameToModelElementMap.ContainsKey(fullyQualifiedName))
                return _roslynSymbolNameToModelElementMap[fullyQualifiedName];

            return null;
        }

        private RoslynBasedUmlClass CreateModelElement(INamedTypeSymbol namedTypeSymbol)
        {
            UmlClass baseClass = null;

            var baseTypeSymbol = namedTypeSymbol.BaseType;
            if (baseTypeSymbol != null)
                baseClass = GetOrAdd(baseTypeSymbol);

            return new RoslynBasedUmlClass(namedTypeSymbol, baseClass);
        }

        private void AddModelElement(INamedTypeSymbol namedTypeSymbol, RoslynBasedUmlClass modelElement)
        {
            _umlModel.Add(modelElement);
            _roslynSymbolNameToModelElementMap.Add(namedTypeSymbol.GetFullyQualifiedName(), modelElement);
        }
    }
}
