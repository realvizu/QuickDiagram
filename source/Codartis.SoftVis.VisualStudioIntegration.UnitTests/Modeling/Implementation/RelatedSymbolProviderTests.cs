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
        public async Task FindRelatedSymbols_Class_Subtype_Async()
        {
            var modelProvider = CreateTestModelProvider();
            modelProvider.AddSource(
                "class C1 {} " +
                "class C2 : C1 {}");

            var baseSymbol = await modelProvider.GetSymbolByNameAsync("C1");
            var relatedSymbol = await modelProvider.GetSymbolByNameAsync("C2");

            var relatedSymbolProvider = CreateRelatedSymbolProviderProvider(modelProvider);
            var relatedSymbols = await relatedSymbolProvider.GetRelatedSymbolsAsync(baseSymbol);

            relatedSymbols.Should().Contain(
                new RelatedSymbolPair(baseSymbol, relatedSymbol, DirectedModelRelationshipTypes.Subtype)
            );
        }

        [NotNull]
        private static TestWorkspaceProvider CreateTestModelProvider() => new TestWorkspaceProvider();

        [NotNull]
        private static IRelatedSymbolProvider CreateRelatedSymbolProviderProvider([NotNull] IRoslynWorkspaceProvider roslynWorkspaceProvider)
        {
            var symbolEqualityComparer = new SymbolEqualityComparer();
            return new RelatedSymbolProvider(roslynWorkspaceProvider, symbolEqualityComparer);
        }
    }
}