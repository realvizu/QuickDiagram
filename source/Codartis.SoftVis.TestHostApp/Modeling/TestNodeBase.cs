using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    internal abstract class TestNodeBase : ITestNode
    {
        public string Name { get; }

        protected TestNodeBase([NotNull] string name)
        {
            Name = name;
        }
    }
}