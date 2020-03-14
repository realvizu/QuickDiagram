using System.Threading.Tasks;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

namespace Codartis.SoftVis.VisualStudioIntegration.UnitTests.Modeling.Implementation
{
    public class RoslynSymbolTranslatorTests
    {
        [Fact]
        public async Task GetNameAndGetFullName_Works_Async()
        {
            var modelProvider = CreateTestModelProvider();
            modelProvider.AddSource(
                @"
                namespace N1
                {
                    class C1
                    {
                        const int Const1 = 1;
                        string _field1 = null;
                        int Property1 { get; set; }
                        E1? Method1(int a, I1<string> b) => null;
                        event D1 Event1;
                    }
                    interface I1<in T> where T : class {}
                    struct S1 {}
                    enum E1 {}
                    delegate int D1(int a, string b);
                }
                ");

            var roslynSymbolTranslator = CreateRoslynSymbolTranslator();

            var classSymbol = await modelProvider.GetSymbolAsync("C1");
            roslynSymbolTranslator.GetName(classSymbol).Should().Be("C1");
            roslynSymbolTranslator.GetFullName(classSymbol).Should().Be("class N1.C1");

            var interfaceSymbol = await modelProvider.GetSymbolAsync("I1");
            roslynSymbolTranslator.GetName(interfaceSymbol).Should().Be("I1<T>");
            roslynSymbolTranslator.GetFullName(interfaceSymbol).Should().Be("interface N1.I1<in T> where T : class");

            var structSymbol = await modelProvider.GetSymbolAsync("S1");
            roslynSymbolTranslator.GetName(structSymbol).Should().Be("S1");
            roslynSymbolTranslator.GetFullName(structSymbol).Should().Be("struct N1.S1");

            var enumSymbol = await modelProvider.GetSymbolAsync("E1");
            roslynSymbolTranslator.GetName(enumSymbol).Should().Be("E1");
            roslynSymbolTranslator.GetFullName(enumSymbol).Should().Be("enum N1.E1");

            var delegateSymbol = await modelProvider.GetSymbolAsync("D1");
            roslynSymbolTranslator.GetName(delegateSymbol).Should().Be("D1");
            roslynSymbolTranslator.GetFullName(delegateSymbol).Should().Be("delegate int N1.D1(int a, string b)");

            var constSymbol = await modelProvider.GetSymbolAsync("Const1");
            roslynSymbolTranslator.GetName(constSymbol).Should().Be("int Const1");
            roslynSymbolTranslator.GetFullName(constSymbol).Should().Be("private const int Const1");

            var fieldSymbol = await modelProvider.GetSymbolAsync("_field1");
            roslynSymbolTranslator.GetName(fieldSymbol).Should().Be("string _field1");
            roslynSymbolTranslator.GetFullName(fieldSymbol).Should().Be("private string _field1");

            var propertySymbol = await modelProvider.GetSymbolAsync("Property1");
            roslynSymbolTranslator.GetName(propertySymbol).Should().Be("int Property1");
            roslynSymbolTranslator.GetFullName(propertySymbol).Should().Be("private int Property1 { get; set; }");

            var methodSymbol = await modelProvider.GetSymbolAsync("Method1");
            roslynSymbolTranslator.GetName(methodSymbol).Should().Be("E1? Method1(int a, I1<string> b)");
            roslynSymbolTranslator.GetFullName(methodSymbol).Should().Be("private E1? Method1(int a, I1<string> b)");

            var eventSymbol = await modelProvider.GetSymbolAsync("Event1");
            roslynSymbolTranslator.GetName(eventSymbol).Should().Be("D1 Event1");
            roslynSymbolTranslator.GetFullName(eventSymbol).Should().Be("private event D1 Event1");
        }

        private static IRoslynSymbolTranslator CreateRoslynSymbolTranslator() => new RoslynSymbolTranslator();

        [NotNull]
        private static TestWorkspaceProvider CreateTestModelProvider() => new TestWorkspaceProvider();
    }
}
