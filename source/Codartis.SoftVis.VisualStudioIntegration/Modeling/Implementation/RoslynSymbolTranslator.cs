using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.VisualStudioIntegration.Util;
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
        private static readonly SymbolKind[] TypeSymbolKinds =
        {
            SymbolKind.NamedType
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
            return IsModeledMember(symbol) || IsModeledType(symbol);
        }

        public bool IsModeledType(ISymbol symbol)
        {
            return symbol.Kind.In(TypeSymbolKinds) &&
                   (symbol is ITypeSymbol).Implies(() => ((ITypeSymbol)symbol).TypeKind.In(ModeledTypeKinds)) &&
                   !IsExcluded(symbol);
        }

        public bool IsModeledMember(ISymbol symbol)
        {
            return symbol.Kind.In(MemberSymbolKinds) &&
                   (symbol is IMethodSymbol).Implies(() => ((IMethodSymbol)symbol).MethodKind.In(MethodMemberMethodKinds)) &&
                   !IsExcluded(symbol);
        }

        private bool IsExcluded([NotNull] ISymbol symbol) => !IsExplicitlyDeclared(symbol) || IsHidden(symbol);

        private static bool IsExplicitlyDeclared([NotNull] ISymbol symbol) => !symbol.IsImplicitlyDeclared;

        private bool IsHidden(ISymbol roslynSymbol)
        {
            return ExcludeTrivialTypes && TrivialTypeNames.Contains(GetFullyQualifiedName(roslynSymbol));
        }

        public bool IsAssociationMember([NotNull] ISymbol symbol) => symbol.Kind.In(AssociationMemberSymbolKinds);

        public ISymbol GetTypeSymbolOfMemberSymbol([NotNull] ISymbol symbol)
        {
            return symbol switch
            {
                IPropertySymbol propertySymbol => propertySymbol.Type,
                IFieldSymbol fieldSymbol => fieldSymbol.Type,
                IEventSymbol eventSymbol => eventSymbol.Type,
                _ => throw new Exception($"Unexpected symbol type {symbol.GetType().Name}")
            };
        }

        public ModelNodeStereotype GetStereotype(ISymbol symbol)
        {
            return symbol switch
            {
                IFieldSymbol fieldSymbol when fieldSymbol.IsConst => ModelNodeStereotypes.Constant,
                IFieldSymbol _ => ModelNodeStereotypes.Field,
                IEventSymbol _ => ModelNodeStereotypes.Event,
                IMethodSymbol _ => ModelNodeStereotypes.Method,
                IPropertySymbol _ => ModelNodeStereotypes.Property,
                INamedTypeSymbol namedTypeSymbol => GetStereotype(namedTypeSymbol.TypeKind),
                _ => ModelNodeStereotype.Default
            };
        }

        private static ModelNodeStereotype GetStereotype(TypeKind typeKind)
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

        public ModelOrigin GetOrigin(ISymbol symbol)
        {
            return symbol.Locations.Any(i => i.IsInSource)
                ? ModelOrigin.SourceCode
                : ModelOrigin.Metadata;
        }

        public string GetDescription(ISymbol symbol) => GetCommentSummary(symbol)?.Trim();

        private static string GetCommentSummary([NotNull] ISymbol symbol)
        {
            var xmlCommentAsString = symbol.GetDocumentationCommentXml();

            string summary = null;
            try
            {
                summary = new XmlDocumentation(xmlCommentAsString).Summary;
            }
            catch (XmlException)
            {
            }

            return summary;
        }

        public string GetName(ISymbol symbol)
        {
            switch (symbol)
            {
                case IMethodSymbol methodSymbol:
                    var parametersString = string.Join(", ", methodSymbol.Parameters.Select(ToDisplayString));
                    return $"{methodSymbol.Name}({parametersString})";
                default:
                    return GetMinimallyQualifiedName(symbol);
            }
        }

        public string GetFullName(ISymbol symbol) => GetNamespaceQualifiedName(symbol);

        [NotNull]
        private static string ToDisplayString([NotNull] IParameterSymbol parameterSymbol)
        {
            return $"{parameterSymbol.Type?.Name} {parameterSymbol.Name}";
        }

        [NotNull]
        private static string GetFullyQualifiedName([NotNull] ISymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        [NotNull]
        private static string GetNamespaceQualifiedName([NotNull] ISymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        }

        [NotNull]
        private static string GetMinimallyQualifiedName([NotNull] ISymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        }
    }
}