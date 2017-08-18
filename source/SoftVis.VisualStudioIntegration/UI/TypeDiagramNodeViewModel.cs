using System;
using System.Collections.Generic;
using System.Windows;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.Util.UI.Wpf;
using Codartis.SoftVis.VisualStudioIntegration.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// View model for a diagram node that represents a type.
    /// </summary>
    internal sealed class TypeDiagramNodeViewModel : DiagramNodeViewModelBase
    {
        private bool _isAbstract;
        private string _fullName;
        private string _description;
        private bool _descriptionExists;
        private bool _isDescriptionVisible;

        public TypeDiagramNodeViewModel(IArrangedDiagram diagram, TypeDiagramNode typeDiagramNode, bool isDescriptionVisible)
            : this(diagram, typeDiagramNode, isDescriptionVisible, Size.Empty, PointExtensions.Undefined, PointExtensions.Undefined)
        {
        }

        public TypeDiagramNodeViewModel(IArrangedDiagram diagram, TypeDiagramNode typeDiagramNode, bool isDescriptionVisible,
            Size size, Point center, Point topLeft)
            : base(diagram, typeDiagramNode, size, center, topLeft)
        {
            PopulateProperties(typeDiagramNode.RoslynTypeNode);
            IsDescriptionVisible = isDescriptionVisible;
        }

        public TypeDiagramNode TypeDiagramNode => (TypeDiagramNode) DiagramNode;

        public override object Clone()
        {
            return new TypeDiagramNodeViewModel(Diagram, TypeDiagramNode, IsDescriptionVisible, Size, Center, TopLeft);
        }

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

        protected override void OnModelNodeUpdated(IDiagramNode diagramNode, IModelNode modelNode)
        {
            base.OnModelNodeUpdated(diagramNode, modelNode);

            var typeDiagramNode = diagramNode as TypeDiagramNode;
            if (typeDiagramNode == null)
                throw new InvalidOperationException($"{typeof(TypeDiagramNode).Name} expected.");

            PopulateProperties(typeDiagramNode.RoslynTypeNode);
        }

        private void PopulateProperties(IRoslynTypeNode roslynTypeNode)
        {
            IsAbstract = roslynTypeNode.IsAbstract;
            FullName = roslynTypeNode.FullName;
            SetDescription(roslynTypeNode.Description);
        }

        private void SetDescription(string description)
        {
            Description = description;
            DescriptionExists = !string.IsNullOrWhiteSpace(_description);
        }
    }
}
