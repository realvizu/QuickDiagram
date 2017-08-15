using Codartis.SoftVis.Diagramming.Implementation;
using Codartis.SoftVis.TestHostApp.Modeling;

namespace Codartis.SoftVis.TestHostApp.Diagramming
{
    internal class TypeDiagramNode : DiagramNode
    {
        public TestType TestType { get; }
        public string Stereotype { get; }

        public TypeDiagramNode(TestType testType) : base(testType)
        {
            TestType = testType;
            Stereotype = $"<<{testType.GetType().Name.ToLower()}>>";
        }
    }
}
