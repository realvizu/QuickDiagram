using System.Threading.Tasks;
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
                "interface I1 {}" +
                "class C1 {}" +
                "class C2 : C1, I1 {}" +
                "class C3 : C2 {}");

            var baseSymbol = await modelProvider.GetSymbolAsync("C2");
            var relatedSymbolPairs = await CreateRelatedSymbolProvider(modelProvider).GetRelatedSymbolsAsync(baseSymbol);

            relatedSymbolPairs.Should().BeEquivalentTo(
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("C1"), DirectedModelRelationshipTypes.BaseType),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("C3"), DirectedModelRelationshipTypes.Subtype),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("I1"), DirectedModelRelationshipTypes.ImplementedInterface)
            );
        }

        [Fact]
        public async Task FindRelatedSymbols_ForAnInterface_Async()
        {
            var modelProvider = CreateTestModelProvider();
            modelProvider.AddSource(
                "interface I1 {}" +
                "interface I2 : I1 {}" +
                "interface I3 : I2 {}" +
                "class C1 : I2 {}");

            var baseSymbol = await modelProvider.GetSymbolAsync("I2");
            var relatedSymbolPairs = await CreateRelatedSymbolProvider(modelProvider).GetRelatedSymbolsAsync(baseSymbol);

            relatedSymbolPairs.Should().BeEquivalentTo(
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("I1"), DirectedModelRelationshipTypes.BaseType),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("I3"), DirectedModelRelationshipTypes.Subtype),
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("C1"), DirectedModelRelationshipTypes.ImplementerType)
            );
        }

        [Fact]
        public async Task FindRelatedSymbols_ForAStruct_Async()
        {
            var modelProvider = CreateTestModelProvider();
            modelProvider.AddSource(
                "interface I1 {}" +
                "struct S1 : I1 {}");

            var baseSymbol = await modelProvider.GetSymbolAsync("S1");
            var relatedSymbolPairs = await CreateRelatedSymbolProvider(modelProvider).GetRelatedSymbolsAsync(baseSymbol);

            relatedSymbolPairs.Should().BeEquivalentTo(
                new RelatedSymbolPair(baseSymbol, await modelProvider.GetSymbolAsync("I1"), DirectedModelRelationshipTypes.ImplementedInterface)
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