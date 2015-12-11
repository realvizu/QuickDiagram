using System;
using Codartis.SoftVis.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    public static class TypeKindExtensions
    {
        public static ModelEntityType ToModelEntityType(this TypeKind typeKind)
        {
            // All symbol types are modeled with class entities just different stereotypes.
            return ModelEntityType.Class;
        }

        public static ModelEntityStereotype ToModelEntityStereotype(this TypeKind typeKind)
        {
            switch (typeKind)
            {
                case TypeKind.Class:
                    return RoslynBasedModelEntityStereotype.Class;
                case TypeKind.Interface:
                    return RoslynBasedModelEntityStereotype.Interface;
                case TypeKind.Struct:
                    return RoslynBasedModelEntityStereotype.Struct;
                case TypeKind.Enum:
                    return RoslynBasedModelEntityStereotype.Enum;
                case TypeKind.Delegate:
                    return RoslynBasedModelEntityStereotype.Delegate;
                default:
                    throw new ArgumentException($"Unexpected TypeKind {typeKind}.");
            }
        }
    }
}
