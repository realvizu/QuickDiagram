using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    public class RoslynSymbolViewModel
    {
        private ISymbol _symbol;

        public RoslynSymbolViewModel(ISymbol symbol)
        {
            _symbol = symbol;
        }

    }
}
