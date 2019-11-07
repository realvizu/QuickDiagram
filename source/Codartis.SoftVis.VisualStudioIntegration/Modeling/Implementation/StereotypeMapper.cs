using Codartis.SoftVis.Modeling.Definition;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    internal static class StereotypeMapper
    {
        public static ModelNodeStereotype GetStereotype([NotNull] this ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                //case SymbolKind.Field:
                //case SymbolKind.Event:
                //case SymbolKind.Method:
                case SymbolKind.Property:
                    return ModelNodeStereotypes.Property;
                case SymbolKind.NamedType:
                    return GetStereotype(((INamedTypeSymbol)symbol).TypeKind);
                default:
                    return ModelNodeStereotype.Default;
            }
        }

        public static ModelNodeStereotype GetStereotype(TypeKind typeKind)
        {
            switch (typeKind)
            {
                case TypeKind.Class:
                    return ModelNodeStereotypes.Class;
                case TypeKind.Struct:
                    return ModelNodeStereotypes.Struct;
                case TypeKind.Enum:
                    return ModelNodeStereotypes.Enum;
                case TypeKind.Delegate:
                    return ModelNodeStereotypes.Delegate;
                case TypeKind.Interface:
                    return ModelNodeStereotypes.Interface;
                default:
                    return ModelNodeStereotype.Default;
            }
        }
    }
}