using JetBrains.Annotations;

namespace Codartis.SoftVis.TestHostApp.Modeling
{
    /// <summary>
    /// A node in the TestHostApp's model concept.
    /// Immutable.
    /// </summary>
    internal interface ITestNode
    {
        [NotNull] string Name { get; }
    }
}