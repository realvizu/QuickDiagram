using System;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Creates Roslyn-based model items.
    /// </summary>
    internal static class RoslynModelItemFactory
    {
        public static RoslynModelNode CreateModelNode(ISymbol symbol)
        {
            var namedTypeSymbol = symbol as INamedTypeSymbol;
            if (namedTypeSymbol == null)
                throw new NotImplementedException($"CreateModelNode for {symbol.GetType().Name} is not implemented.");

            var id = ModelNodeId.Create();

            switch (namedTypeSymbol.TypeKind)
            {
                case TypeKind.Class:
                    return new RoslynClassNode(id, namedTypeSymbol);
                case TypeKind.Interface:
                    return new RoslynInterfaceNode(id, namedTypeSymbol);
                case TypeKind.Struct:
                    return new RoslynStructNode(id, namedTypeSymbol);
                case TypeKind.Enum:
                    return new RoslynEnumNode(id, namedTypeSymbol);
                case TypeKind.Delegate:
                    return new RoslynDelegateNode(id, namedTypeSymbol);
                default:
                    throw new Exception($"Unexpected TypeKind: {namedTypeSymbol.TypeKind}");
            }
        }

        public static ModelRelationship CreateRoslynRelationship(IRoslynModelNode sourceNode, IRoslynModelNode targetNode, ModelRelationshipStereotype stereotype)
        {
            var id = ModelRelationshipId.Create();

            if (stereotype == ModelRelationshipStereotypes.Inheritance)
                return new InheritanceRelationship(id, sourceNode, targetNode);

            if (stereotype == ModelRelationshipStereotypes.Implementation)
                return new ImplementationRelationship(id, sourceNode, targetNode);

            throw new InvalidOperationException($"Unexpected relationship type {stereotype.Name}");
        }
    }
}