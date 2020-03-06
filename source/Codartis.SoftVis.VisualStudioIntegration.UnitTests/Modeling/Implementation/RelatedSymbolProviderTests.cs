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
                    C4 _f1;
                    C5 P1 { get; set; }
                    void m1(){}
                }
                class C3 : C2 {}
                class C4 {}
                class C5 {}
                ");

            var baseSymbol = await modelProvider.GetSymbolAsync("C2");
            var relatedSymbolPairs = await CreateRelatedSymbolProvider(modelProvider).GetRelatedSymbolsAsync(baseSymbol);

            relatedSymbolPairs.Should().BeEquivalentTo(
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("C1"), DirectedModelRelationshipTypes.BaseType),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("C3"), DirectedModelRelationshipTypes.Subtype),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("I1"), DirectedModelRelationshipTypes.ImplementedInterface),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("_f1"), CommonDirectedModelRelationshipTypes.Contained),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("P1"), CommonDirectedModelRelationshipTypes.Contained),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("m1"), CommonDirectedModelRelationshipTypes.Contained),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("C4"), DirectedModelRelationshipTypes.AssociatedType),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("C5"), DirectedModelRelationshipTypes.AssociatedType)
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
                    C2 P1 { get; }
                }
                interface I3 : I2 {}
                class C1 : I2 {}
                class C2 {}
                ");

            var baseSymbol = await modelProvider.GetSymbolAsync("I2");
            var relatedSymbolPairs = await CreateRelatedSymbolProvider(modelProvider).GetRelatedSymbolsAsync(baseSymbol);

            relatedSymbolPairs.Should().BeEquivalentTo(
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("I1"), DirectedModelRelationshipTypes.BaseType),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("I3"), DirectedModelRelationshipTypes.Subtype),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("C1"), DirectedModelRelationshipTypes.ImplementerType),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("m1"), CommonDirectedModelRelationshipTypes.Contained),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("P1"), CommonDirectedModelRelationshipTypes.Contained),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("C2"), DirectedModelRelationshipTypes.AssociatedType)
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
                    C2 P1 { get; set; }
                    void m1(){}
                }
                class C1 {}
                class C2 {}
                ");

            var baseSymbol = await modelProvider.GetSymbolAsync("S1");
            var relatedSymbolPairs = await CreateRelatedSymbolProvider(modelProvider).GetRelatedSymbolsAsync(baseSymbol);

            relatedSymbolPairs.Should().BeEquivalentTo(
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("I1"), DirectedModelRelationshipTypes.ImplementedInterface),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("_f1"), CommonDirectedModelRelationshipTypes.Contained),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("P1"), CommonDirectedModelRelationshipTypes.Contained),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("m1"), CommonDirectedModelRelationshipTypes.Contained),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("C1"), DirectedModelRelationshipTypes.AssociatedType),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("C2"), DirectedModelRelationshipTypes.AssociatedType)
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

        [Fact]
        public async Task FindRelatedSymbols_ForAField_Async()
        {
            var modelProvider = CreateTestModelProvider();
            modelProvider.AddSource(
                @"
                class C1
                {
                    C2 _f1;
                }
                class C2 {}
                ");

            var baseSymbol = await modelProvider.GetSymbolAsync("_f1");
            var relatedSymbolPairs = await CreateRelatedSymbolProvider(modelProvider).GetRelatedSymbolsAsync(baseSymbol);

            relatedSymbolPairs.Should().BeEquivalentTo(
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("C1"), CommonDirectedModelRelationshipTypes.Container)
            );
        }

        [Fact]
        public async Task FindRelatedSymbols_ForAProperty_Async()
        {
            var modelProvider = CreateTestModelProvider();
            modelProvider.AddSource(
                @"
                class C1
                {
                    C2 P1 {get; set; }
                }
                class C2 {}
                ");

            var baseSymbol = await modelProvider.GetSymbolAsync("P1");
            var relatedSymbolPairs = await CreateRelatedSymbolProvider(modelProvider).GetRelatedSymbolsAsync(baseSymbol);

            relatedSymbolPairs.Should().BeEquivalentTo(
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("C1"), CommonDirectedModelRelationshipTypes.Container)
            );
        }

        [Fact]
        public async Task FindRelatedSymbols_ForAMethod_Async()
        {
            var modelProvider = CreateTestModelProvider();
            modelProvider.AddSource(
                @"
                class C1
                {
                    C2 M1()
                }
                class C2 {}
                ");

            var baseSymbol = await modelProvider.GetSymbolAsync("M1");
            var relatedSymbolPairs = await CreateRelatedSymbolProvider(modelProvider).GetRelatedSymbolsAsync(baseSymbol);

            relatedSymbolPairs.Should().BeEquivalentTo(
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("C1"), CommonDirectedModelRelationshipTypes.Container)
            );
        }

        [Fact]
        public async Task FindRelatedSymbols_ForAnEvent_Async()
        {
            var modelProvider = CreateTestModelProvider();
            modelProvider.AddSource(
                @"
                class C1
                {
                    event D1 E1;
                }
                delegate D1();
                ");

            var baseSymbol = await modelProvider.GetSymbolAsync("E1");
            var relatedSymbolPairs = await CreateRelatedSymbolProvider(modelProvider).GetRelatedSymbolsAsync(baseSymbol);

            relatedSymbolPairs.Should().BeEquivalentTo(
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("C1"), CommonDirectedModelRelationshipTypes.Container)
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