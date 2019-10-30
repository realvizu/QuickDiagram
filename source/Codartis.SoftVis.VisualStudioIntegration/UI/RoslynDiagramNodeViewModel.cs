using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Diagramming.Definition;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.Util.UI;
using JetBrains.Annotations;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// View model for a diagram node that represents a type.
    /// </summary>
    internal sealed class RoslynDiagramNodeViewModel : DiagramNodeViewModel
    {
        private bool _isDescriptionVisible;

        public RoslynDiagramNodeViewModel(
            [NotNull] IModelService modelService,
            [NotNull] IDiagramService diagramService,
            [NotNull] IRelatedNodeTypeProvider relatedNodeTypeProvider,
            [NotNull] IFocusTracker<IDiagramShapeUi> focusTracker,
            [NotNull] IDiagramNode diagramNode,
            bool isDescriptionVisible)
            : base(modelService, diagramService, relatedNodeTypeProvider, focusTracker, diagramNode)
        {
            IsDescriptionVisible = isDescriptionVisible;
        }

        public bool IsDescriptionVisible
        {
            get { return _isDescriptionVisible; }
            set
            {
                if (_isDescriptionVisible != value)
                {
                    _isDescriptionVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public override object CloneForImageExport()
        {
            var clone = new RoslynDiagramNodeViewModel(
                ModelService,
                DiagramService,
                RelatedNodeTypeProvider,
                FocusTracker,
                DiagramNode,
                IsDescriptionVisible);

            SetPropertiesForImageExport(clone);
            
            return clone;
        }
    }
}