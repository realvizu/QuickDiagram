using System;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.Util.UI;
using Codartis.Util.UI.Wpf;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    public sealed class RoslynDiagramShapeViewModelFactory : DiagramShapeViewModelFactoryBase, IRoslynDiagramShapeUiFactory
    {
        public bool IsDescriptionVisible { get; set; }

        public RoslynDiagramShapeViewModelFactory(
            [NotNull] IModelEventSource modelEventSource,
            [NotNull] IDiagramEventSource diagramEventSource,
            [NotNull] IRelatedNodeTypeProvider relatedNodeTypeProvider,
            bool isDescriptionVisible)
            : base(modelEventSource, diagramEventSource, relatedNodeTypeProvider)
        {
            IsDescriptionVisible = isDescriptionVisible;
        }

        public override IDiagramNodeUi CreateDiagramNodeUi(
            IDiagramNode diagramNode,
            IFocusTracker<IDiagramShapeUi> focusTracker)
        {
            var payload = diagramNode.ModelNode.Payload;

            switch (payload)
            {
                case null:
                    return null;

                case ISymbol symbol:
                    return new RoslynDiagramNodeViewModel(
                        ModelEventSource,
                        DiagramEventSource,
                        diagramNode,
                        RelatedNodeTypeProvider,
                        (IWpfFocusTracker<IDiagramShapeUi>)focusTracker,
                        IsDescriptionVisible,
                        symbol,
                        new RoslynDiagramNodeHeaderViewModel(symbol, IsDescriptionVisible));

                default:
                    throw new Exception($"Unexpected payload type {payload.GetType().Name}");
            }
        }
    }
}