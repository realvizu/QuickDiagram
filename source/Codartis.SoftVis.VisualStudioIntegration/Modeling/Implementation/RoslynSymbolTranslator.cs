using System;
using System.Collections.Generic;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.Util;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    public sealed class RoslynSymbolTranslator : IRoslynSymbolTranslator
    {
        [NotNull]
        [ItemNotNull]
        private static readonly List<string> TrivialTypeNames =
            new List<string>
            {
                "bool", "System.Boolean",
                "byte", "System.Byte",
                "sbyte", "System.SByte",
                "char", "System.Char",
                "decimal", "System.Decimal",
                "double", "System.Double",
                "float", "System.Single",
                "int", "System.Int32",
                "uint", "System.UInt32",
                "long", "System.Int64",
                "ulong", "System.UInt64",
                "short", "System.Int16",
                "ushort", "System.UInt16",
                "object", "System.Object",
                "string", "System.String"
            };

        [NotNull]
        private static readonly TypeKind[] ModeledTypeKinds =
        {
            TypeKind.Class,
            TypeKind.Delegate,
            TypeKind.Enum,
            TypeKind.Interface,
            TypeKind.Struct
        };

        [NotNull]
        private static readonly SymbolKind[] MemberSymbolKinds =
        {
            SymbolKind.Event,
            SymbolKind.Field,
            SymbolKind.Method,
            SymbolKind.Property
        };

        [NotNull]
        private static readonly SymbolKind[] AssociationMemberSymbolKinds =
        {
            SymbolKind.Field,
            SymbolKind.Property,
            SymbolKind.Event
        };

        [NotNull]
        private static readonly MethodKind[] MethodMemberMethodKinds =
        {
            MethodKind.Ordinary
        };

        public bool ExcludeTrivialTypes { get; set; }

        public RoslynSymbolTranslator(bool excludeTrivialTypes = true)
        {
            ExcludeTrivialTypes = excludeTrivialTypes;
        }

        public bool IsModeledSymbol(ISymbol symbol)
        {
            return symbol.Kind.In(MemberSymbolKinds) ||
                   (symbol is INamedTypeSymbol).Implies(() => ((INamedTypeSymbol)symbol).TypeKind.In(ModeledTypeKinds)) &&
                   !IsHidden(symbol);
        }

        private bool IsHidden(ISymbol roslynSymbol)
        {
            return ExcludeTrivialTypes && TrivialTypeNames.Contains(roslynSymbol.GetFullyQualifiedName());
        }

        public ModelNodeStereotype GetStereotype(ISymbol symbol)
        {
            return symbol.Kind switch
            {
                SymbolKind.Field => ModelNodeStereotypes.Field,
                SymbolKind.Event => ModelNodeStereotypes.Event,
                SymbolKind.Method => ModelNodeStereotypes.Method,
                SymbolKind.Property => ModelNodeStereotypes.Property,
                SymbolKind.NamedType => GetStereotype(((INamedTypeSymbol)symbol).TypeKind),
                _ => ModelNodeStereotype.Default
            };
        }

        public static ModelNodeStereotype GetStereotype(TypeKind typeKind)
        {
            return typeKind switch
            {
                TypeKind.Class => ModelNodeStereotypes.Class,
                TypeKind.Struct => ModelNodeStereotypes.Struct,
                TypeKind.Enum => ModelNodeStereotypes.Enum,
                TypeKind.Delegate => ModelNodeStereotypes.Delegate,
                TypeKind.Interface => ModelNodeStereotypes.Interface,
                _ => ModelNodeStereotype.Default
            };
        }

        public static ISymbol GetTypeSymbolOfMemberSymbol([NotNull] ISymbol symbol)
        {
            return symbol switch
            {
                IPropertySymbol propertySymbol => propertySymbol.Type,
                IFieldSymbol fieldSymbol => fieldSymbol.Type,
                IEventSymbol eventSymbol => eventSymbol.Type,
                _ => throw new Exception($"Unexpected symbol type {symbol.GetType().Name}")
            };
        }

        public static bool IsModeledMember([NotNull] ISymbol i) => i.Kind.In(MemberSymbolKinds);

        public static bool IfMethodThenModeledMethod([NotNull] ISymbol i)
        {
            return !(i is IMethodSymbol methodSymbol) || methodSymbol.MethodKind.In(MethodMemberMethodKinds);
        }

        public static bool IsAssociationMember([NotNull] ISymbol i) => i.Kind.In(AssociationMemberSymbolKinds);
        public static bool IsExplicitlyDeclared([NotNull] ISymbol i) => !i.IsImplicitlyDeclared;
    }
}