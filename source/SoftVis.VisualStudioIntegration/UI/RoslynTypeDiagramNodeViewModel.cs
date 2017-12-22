using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util.UI.Wpf.ViewModels;
using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// View model for a diagram node that represents a type.
    /// </summary>
    internal sealed class RoslynTypeDiagramNodeViewModel : DiagramNodeViewModelBase
    {
        private bool _isAbstract;
        private string _fullName;
        private string _description;
        private bool _descriptionExists;
        private bool _isDescriptionVisible;

        public RoslynTypeDiagramNodeViewModel(IModelService modelService, IDiagramService diagramService, IFocusTracker<IDiagramShapeUi> focusTracker,
            RoslynTypeDiagramNode roslynTypeDiagramNode, bool isDescriptionVisible)
            : base(modelService, diagramService, focusTracker, roslynTypeDiagramNode)
        {
            IsDescriptionVisible = isDescriptionVisible;
            PopulateFromDiagramNode(roslynTypeDiagramNode);
        }

        public RoslynTypeDiagramNode RoslynTypeDiagramNode => (RoslynTypeDiagramNode) DiagramNode;

        public override object Clone() 
            => new RoslynTypeDiagramNodeViewModel(ModelService, DiagramService, FocusTracker, RoslynTypeDiagramNode, IsDescriptionVisible) {Size = Size};

        protected override IEnumerable<RelatedNodeType> GetRelatedNodeTypes()
        {
            yield return new RelatedNodeType(DirectedRelationshipTypes.BaseType, "Base types");
            yield return new RelatedNodeType(DirectedRelationshipTypes.Subtype, "Subtypes");
            yield return new RelatedNodeType(DirectedRelationshipTypes.ImplementerType, "Implementers");
            yield return new RelatedNodeType(DirectedRelationshipTypes.ImplementedInterface, "Interfaces");
        }

        public bool IsAbstract
        {
            get { return _isAbstract; }
            set
            {
                if (_isAbstract != value)
                {
                    _isAbstract = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FullName
        {
            get { return _fullName; }
            set
            {
                if (_fullName != value)
                {
                    _fullName = value;
                    OnPropertyChanged();
                }
            }
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

        protected override void PopulateFromDiagramNode(IDiagramNode diagramNode)
        {
            base.PopulateFromDiagramNode(diagramNode);

            var roslynTypeDiagramNode = (RoslynTypeDiagramNode) diagramNode;

            IsAbstract = roslynTypeDiagramNode.IsAbstract;
            FullName = roslynTypeDiagramNode.RoslynTypeNode.FullName;
            SetDescription(roslynTypeDiagramNode.RoslynTypeNode.Description);
        }

        private void SetDescription(string description)
        {
            Description = description;
            DescriptionExists = !string.IsNullOrWhiteSpace(_description);
        }
    }
}
