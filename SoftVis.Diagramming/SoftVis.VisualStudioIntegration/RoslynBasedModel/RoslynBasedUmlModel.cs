using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codartis.SoftVis.VisualStudioIntegration.RoslynBasedModel
{
    public class RoslynBasedUmlModel
    {
        private UmlModel _umlModel;
        private Dictionary<ISymbol, RoslynBasedUmlModelElement> _elements;

        public RoslynBasedUmlModel()
        {
            _umlModel = new UmlModel();
            _elements = new Dictionary<ISymbol, RoslynBasedUmlModelElement>();
        }

        private RoslynBasedUmlModelElement Add(ISymbol symbol)
        {
            var element = new RoslynBasedUmlModelElement(symbol);
            _umlModel.Add((UmlType)element.UmlModelElement);
            return element;
        }

        private RoslynBasedUmlModelElement Get(ISymbol symbol)
        {
            if (_elements.ContainsKey(symbol))
                return _elements[symbol];

            return null;
        }

        public RoslynBasedUmlModelElement GetOrAdd(ISymbol symbol)
        {
            var element = Get(symbol);
            if (element == null)
                element = Add(symbol);

            return element;
        }
    }
}
