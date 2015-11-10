using System;
using Codartis.SoftVis.Diagramming.Layout;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative;
using Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic;
using Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Helpers;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.Diagramming.UnitTests.Diagramming.Layout.Incremental.Relative
{
    public class RelativeLayoutCalculatorTests
    {
        private readonly DiagramGraphBuilder _diagramGraphBuilder;
        private DiagramGraph DiagramGraph => _diagramGraphBuilder.Graph;

        public RelativeLayoutCalculatorTests()
        {
            _diagramGraphBuilder = new DiagramGraphBuilder();
        }

        [Fact]
        public void OnDiagramNodeAdded_FirstNodeAdded()
        {
            var calculator = new RelativeLayoutCalculator();
            _diagramGraphBuilder.SetUp("A");

            var diagramNode = _diagramGraphBuilder.GetVertex("A");
            AssertEvent<RelativeLayoutCalculator, RelativeLocationAssignedLayoutAction>(
                calculator, i => i.OnDiagramNodeAdded(diagramNode, null), i => i.To, 0, 0);
        }

        [Fact]
        public void OnDiagramNodeAdded_SecondNodeAdded()
        {
            var calculator = new RelativeLayoutCalculator();
            _diagramGraphBuilder.SetUp("A", "B");

            var diagramNode = _diagramGraphBuilder.GetVertex("A");
            AssertEvent<RelativeLayoutCalculator, RelativeLocationAssignedLayoutAction>(
                calculator, i => i.OnDiagramNodeAdded(diagramNode, null), i => i.To, 0, 0);

            var diagramNode2 = _diagramGraphBuilder.GetVertex("B");
            AssertEvent<RelativeLayoutCalculator, RelativeLocationAssignedLayoutAction>(
                calculator, i => i.OnDiagramNodeAdded(diagramNode2, null), i => i.To, 0, 1);
        }

        [Fact(Skip = "Fix later")]
        public void OnDiagramConnectorAdded()
        {
            var calculator = new RelativeLayoutCalculator();

            _diagramGraphBuilder.SetUp("A->B");
            var diagramNodeA = _diagramGraphBuilder.GetVertex("A");
            var diagramNodeB = _diagramGraphBuilder.GetVertex("B");
            var diagramConnectorAtoB = _diagramGraphBuilder.GetEdge("A", "B");

            calculator.OnDiagramNodeAdded(diagramNodeA, null);
            calculator.OnDiagramNodeAdded(diagramNodeB, null);
            AssertEvent<RelativeLayoutCalculator, RelativeLocationChangedLayoutAction>(
                calculator, i => i.OnDiagramConnectorAdded(diagramConnectorAtoB, null), i => i.From, 0, 1, i => i.To, 1, 0);
        }

        private static void AssertEvent<TSubject, TEventArgs>(TSubject subject, Action<TSubject> action,
            Func<TEventArgs, RelativeLocation> relativeLocationAccessor, int expectedLayerIndex, int expectedIndexInLayer,
            Func<TEventArgs, RelativeLocation> relativeLocationAccessor2 = null, int expectedLayerIndex2 = 0, int expectedIndexInLayer2 = 0)
            where TSubject : LayoutActionEventSource
            where TEventArgs : EventArgs
        {
            subject.MonitorEvents();
            action(subject);
            subject.ShouldRaise(nameof(subject.LayoutActionExecuted))
                .WithArgs<TEventArgs>(i => ValidateRelativeLocation(i, relativeLocationAccessor, expectedLayerIndex, expectedIndexInLayer))
                .WithArgs<TEventArgs>(i => ValidateRelativeLocation(i, relativeLocationAccessor2, expectedLayerIndex2, expectedIndexInLayer2));
        }

        private static bool ValidateRelativeLocation<TEventArgs>(TEventArgs i,
            Func<TEventArgs, RelativeLocation> relativeLocationAccessor, int expectedLayerIndex, int expectedIndexInLayer)
            where TEventArgs : EventArgs
        {
            if (relativeLocationAccessor == null)
                return true;

            return relativeLocationAccessor(i).LayerIndex == expectedLayerIndex &&
                   relativeLocationAccessor(i).IndexInLayer == expectedIndexInLayer;
        }
    }
}
