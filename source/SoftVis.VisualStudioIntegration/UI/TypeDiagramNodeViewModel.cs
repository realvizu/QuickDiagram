using System.Collections.Generic;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling2;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util.UI.Wpf;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// View model for a diagram node that represents a type.
    /// </summary>
    internal class TypeDiagramNodeViewModel : DiagramNodeViewModelBase
    {
        private string _description;
        private bool _descriptionExists;
        private bool _isDescriptionVisible;

        public TypeDiagramNodeViewModel(IArrangedDiagram diagram, IDiagramNode diagramNode, bool isDescriptionVisible)
            : this(diagram, diagramNode, isDescriptionVisible, Size.Empty, PointExtensions.Undefined, PointExtensions.Undefined)
        {
        }

        public TypeDiagramNodeViewModel(IArrangedDiagram diagram, IDiagramNode diagramNode, bool isDescriptionVisible,
            Size size, Point center, Point topLeft)
            : base(diagram, diagramNode, isDescriptionVisible, size, center, topLeft)
        {
            _description = diagramNode.ModelNode.Description;
            _descriptionExists = !string.IsNullOrWhiteSpace(_description);
            _isDescriptionVisible = isDescriptionVisible;
        }

        public override object Clone()
        {
            return new TypeDiagramNodeViewModel(Diagram, DiagramNode, IsDescriptionVisible, Size, Center, TopLeft);
        }

        protected override IEnumerable<RelatedNodeType> GetRelatedNodeTypes()
        {
            // TODO 
            yield break;
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool DescriptionExists
        {
            get { return _descriptionExists; }
            set
            {
                if (_descriptionExists != value)
                {
                    _descriptionExists = value;
                    OnPropertyChanged();
                }
            }
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

        protected override void OnModelNodeUpdated(IDiagramNode diagramNode, IModelNode modelNode)
        {
            base.OnModelNodeUpdated(diagramNode, modelNode);

            Description = modelNode.Description;
            DescriptionExists = !string.IsNullOrWhiteSpace(Description);
        }

    }
}
