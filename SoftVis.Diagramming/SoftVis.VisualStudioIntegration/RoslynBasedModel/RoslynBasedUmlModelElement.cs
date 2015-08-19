using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codartis.SoftVis.VisualStudioIntegration.RoslynBasedModel
{
    public class RoslynBasedUmlModelElement
    {
        public UmlModelElement UmlModelElement { get; private set; }

        public RoslynBasedUmlModelElement(ISymbol symbol)
        {
            UmlModelElement = new UmlClass
            {
                NativeItem = symbol,
                Name = symbol.Name,
            };
        }
    }
}
