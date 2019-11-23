using Codartis.SoftVis.TestHostApp.Modeling;
using Codartis.SoftVis.UI;

namespace Codartis.SoftVis.TestHostApp.UI
{
    /// <summary>
    /// A payload UI factory that returns the ITestNode payload as the payload UI.
    /// </summary>
    public sealed class TestNodePayloadUiFactory : IPayloadUiFactory
    {
        public IPayloadUi Create(object payload) => (ITestNode)payload;
    }
}
