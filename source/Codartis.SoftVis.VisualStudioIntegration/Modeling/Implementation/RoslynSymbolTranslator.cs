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

        [NotNull]
        private static readonly SymbolDisplayFormat SymbolNameDisplayFormat = new SymbolDisplayFormat(
            memberOptions: SymbolDisplayMemberOptions.IncludeType |
                           SymbolDisplayMemberOptions.IncludeParameters,
            parameterOptions: SymbolDisplayParameterOptions.IncludeType |
                              SymbolDisplayParameterOptions.IncludeName,
            genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
            miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        private const SymbolDisplayMiscellaneousOptions MiscDisplayOptions = SymbolDisplayMiscellaneousOptions.UseSpecialTypes;

        private const SymbolDisplayParameterOptions ParameterDisplayOptions = SymbolDisplayParameterOptions.IncludeExtensionThis |
                                                                              SymbolDisplayParameterOptions.IncludeParamsRefOut |
                                                                              SymbolDisplayParameterOptions.IncludeType |
                                                                              SymbolDisplayParameterOptions.IncludeName |
                                                                              SymbolDisplayParameterOptions.IncludeDefaultValue;

        private const SymbolDisplayMemberOptions MemberDisplayOptions = SymbolDisplayMemberOptions.IncludeParameters |
                                                                        SymbolDisplayMemberOptions.IncludeExplicitInterface |
                                                                        SymbolDisplayMemberOptions.IncludeModifiers |
                                                                        SymbolDisplayMemberOptions.IncludeType |
                                                                        SymbolDisplayMemberOptions.IncludeAccessibility |
                                                                        SymbolDisplayMemberOptions.IncludeConstantValue;

        private const SymbolDisplayGenericsOptions GenericDisplayOptions = SymbolDisplayGenericsOptions.IncludeTypeParameters |
                                                                           SymbolDisplayGenericsOptions.IncludeTypeConstraints |
                                                                           SymbolDisplayGenericsOptions.IncludeVariance;

        [NotNull]
        private static readonly SymbolDisplayFormat TypeSymbolFullNameDisplayFormat = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
            delegateStyle: SymbolDisplayDelegateStyle.NameAndSignature,
            kindOptions: SymbolDisplayKindOptions.IncludeTypeKeyword,
            memberOptions: MemberDisplayOptions,
            parameterOptions: ParameterDisplayOptions,
            genericsOptions: GenericDisplayOptions,
            miscellaneousOptions: MiscDisplayOptions);

        [NotNull]
        private static readonly SymbolDisplayFormat MemberSymbolFullNameDisplayFormat = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
            propertyStyle: SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
            kindOptions: SymbolDisplayKindOptions.IncludeMemberKeyword,
            memberOptions: MemberDisplayOptions,
            parameterOptions: ParameterDisplayOptions,
            genericsOptions: GenericDisplayOptions,
            miscellaneousOptions: MiscDisplayOptions);

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
        
        public bool GetIsStatic(ISymbol symbol) => symbol.IsStatic;

        public bool GetIsAbstract(ISymbol symbol) => symbol.IsAbstract;

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

        public string GetName(ISymbol symbol) => symbol.ToDisplayString(SymbolNameDisplayFormat);

        public string GetFullName(ISymbol symbol)
        {
            switch (symbol)
            {
                case INamedTypeSymbol _:
                    return symbol.ToDisplayString(TypeSymbolFullNameDisplayFormat);
                default:
                    return symbol.ToDisplayString(MemberSymbolFullNameDisplayFormat);
            }
        }

        [NotNull]
        private static string GetFullyQualifiedName([NotNull] ISymbol symbol) => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }
}