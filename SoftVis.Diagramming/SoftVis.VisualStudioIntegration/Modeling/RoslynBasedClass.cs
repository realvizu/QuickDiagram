using System.Collections.Generic;
using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// A model entity created from a Roslyn class symbol.
    /// </summary>
    public class RoslynBasedClass : IModelEntity
    {
        private INamedTypeSymbol RoslynSymbol { get; }
        public RoslynBasedClass BaseClass { get; set; }

        public RoslynBasedClass(INamedTypeSymbol namedTypeSymbol, RoslynBasedClass baseClass = null)
        {
            RoslynSymbol = namedTypeSymbol;
            BaseClass = baseClass;
        }

        public string Name => RoslynSymbol.Name;
        public ModelEntityType Type => ModelEntityType.Class;
        
        public IEnumerable<IModelRelationship> OutgoingRelationships
        {
            get
            {
                if (BaseClass != null)
                    yield return new ModelRelationship(this, BaseClass, ModelRelationshipType.Generalization);
            }
        }
    }
}
