using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Codartis.SoftVis.VisualStudioIntegration.UnitTests.Modeling.Implementation
{
    public class RelatedSymbolProviderTests
    {
        [Fact]
        public async Task FindRelatedSymbols_ForAClass_Async()
        {
            var modelProvider = CreateTestModelProvider();
            modelProvider.AddSource(
                @"
                interface I1 {}
                class C1 {}
                class C2 : C1, I1 
                {
                    C1 _f1;
                    void m1(){}
                }
                class C3 : C2 {}
                ");

            var baseSymbol = await modelProvider.GetSymbolAsync("C2");
            var relatedSymbolPairs = await CreateRelatedSymbolProvider(modelProvider).GetRelatedSymbolsAsync(baseSymbol);

            relatedSymbolPairs.Should().BeEquivalentTo(
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("C1"), DirectedModelRelationshipTypes.BaseType),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("C3"), DirectedModelRelationshipTypes.Subtype),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("I1"), DirectedModelRelationshipTypes.ImplementedInterface),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("_f1"), CommonDirectedModelRelationshipTypes.Contained),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("m1"), CommonDirectedModelRelationshipTypes.Contained)
            );
        }

        [Fact]
        public async Task FindRelatedSymbols_ForAnInterface_Async()
        {
            var modelProvider = CreateTestModelProvider();
            modelProvider.AddSource(
                @"
                interface I1 {}
                interface I2 : I1 
                {
                    void m1();
                    I1 P1 { get; }
                }
                interface I3 : I2 {}
                class C1 : I2 {}
                ");

            var baseSymbol = await modelProvider.GetSymbolAsync("I2");
            var relatedSymbolPairs = await CreateRelatedSymbolProvider(modelProvider).GetRelatedSymbolsAsync(baseSymbol);

            relatedSymbolPairs.Should().BeEquivalentTo(
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("I1"), DirectedModelRelationshipTypes.BaseType),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("I3"), DirectedModelRelationshipTypes.Subtype),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("C1"), DirectedModelRelationshipTypes.ImplementerType),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("m1"), CommonDirectedModelRelationshipTypes.Contained),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("P1"), CommonDirectedModelRelationshipTypes.Contained)
            );
        }

        [Fact]
        public async Task FindRelatedSymbols_ForAStruct_Async()
        {
            var modelProvider = CreateTestModelProvider();
            modelProvider.AddSource(
                @"
                interface I1 {}
                struct S1 : I1 
                {
                    C1 _f1;
                    void m1(){}
                }
                ");

            var baseSymbol = await modelProvider.GetSymbolAsync("S1");
            var relatedSymbolPairs = await CreateRelatedSymbolProvider(modelProvider).GetRelatedSymbolsAsync(baseSymbol);

            relatedSymbolPairs.Should().BeEquivalentTo(
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("I1"), DirectedModelRelationshipTypes.ImplementedInterface),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("_f1"), CommonDirectedModelRelationshipTypes.Contained),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("m1"), CommonDirectedModelRelationshipTypes.Contained)
            );
        }

        [Fact]
        public async Task FindRelatedSymbols_ForAnEnum_Async()
        {
            var modelProvider = CreateTestModelProvider();
            modelProvider.AddSource(
                @"
                enum E1
                {
                    C1,
                    C2
                }
                ");

            var baseSymbol = await modelProvider.GetSymbolAsync("E1");
            var relatedSymbolPairs = await CreateRelatedSymbolProvider(modelProvider).GetRelatedSymbolsAsync(baseSymbol);

            relatedSymbolPairs.Should().BeEquivalentTo(
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("C1"), CommonDirectedModelRelationshipTypes.Contained),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("C2"), CommonDirectedModelRelationshipTypes.Contained)
            );
        }

        [NotNull]
        private static TestWorkspaceProvider CreateTestModelProvider() => new TestWorkspaceProvider();

        [NotNull]
        private static IRelatedSymbolProvider CreateRelatedSymbolProvider([NotNull] IRoslynWorkspaceProvider roslynWorkspaceProvider)
        {
            var symbolEqualityComparer = new SymbolEqualityComparer();
            return new RelatedSymbolProvider(roslynWorkspaceProvider, symbolEqualityComparer);
        }
    }
}