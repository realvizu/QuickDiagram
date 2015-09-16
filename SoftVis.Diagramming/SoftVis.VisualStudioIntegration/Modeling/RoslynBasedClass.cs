using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// A model entity created from a Roslyn class symbol.
    /// </summary>
    internal class RoslynBasedClass : IModelEntity
    {
        private readonly List<IModelRelationship> _outgoingRelationships = new List<IModelRelationship>();
        private readonly List<IModelRelationship> _incomingRelationships = new List<IModelRelationship>();

        public INamedTypeSymbol RoslynSymbol { get; }

        internal RoslynBasedClass(INamedTypeSymbol namedTypeSymbol)
        {
            //TODO: now treat all user defined types as class
            //if (namedTypeSymbol.TypeKind != TypeKind.Class)
            //    throw new ArgumentException($"{namedTypeSymbol.Name} is not a class.");

            RoslynSymbol = namedTypeSymbol;
        }

        public string Name => RoslynSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        public ModelEntityType Type => ModelEntityType.Class;
        public IEnumerable<IModelRelationship> OutgoingRelationships => _outgoingRelationships;
        public IEnumerable<IModelRelationship> IncomingRelationships => _incomingRelationships;

        public void AddOutgoingRelationship(IModelRelationship relationship)
        {
            _outgoingRelationships.Add(relationship);
        }

        public void AddIncomingRelationship(IModelRelationship relationship)
        {
            _incomingRelationships.Add(relationship);
        }

        public RoslynBasedClass BaseClass
        {
            get
            {
                return _outgoingRelationships
                    .FirstOrDefault(i => i.Type == ModelRelationshipType.Generalization)
                    ?.Target as RoslynBasedClass;
            }
        }

        public IEnumerable<RoslynBasedClass> ChildClasses
        {
            get
            {
                return _incomingRelationships
                    .Where(i => i.Type == ModelRelationshipType.Generalization)
                    .Select(i=>i.Source).OfType<RoslynBasedClass>();
            }
        }
    }
}
