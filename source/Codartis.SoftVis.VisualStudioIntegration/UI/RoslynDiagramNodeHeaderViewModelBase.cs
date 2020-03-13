using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI.Wpf.ViewModel;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// Abstract base class for Roslyn-based diagram node header view models.
    /// Responsible for translating Roslyn symbol info into presentable format
    /// using an <see cref="IRoslynSymbolTranslator"/>.
    /// </summary>
    public abstract class RoslynDiagramNodeHeaderViewModelBase : DiagramNodeHeaderViewModel
    {
        [NotNull] protected IRoslynSymbolTranslator RoslynSymbolTranslator { get; }

        private ModelOrigin _origin;
        private ModelNodeStereotype _stereotype;
        private string _name;
        private string _fullName;
        private string _description;
        private bool _descriptionExists;
        private bool _isDescriptionVisible;
        private bool _isAbstract;

        protected RoslynDiagramNodeHeaderViewModelBase(
            [NotNull] ISymbol symbol,
            [NotNull] IRoslynSymbolTranslator roslynSymbolTranslator,
            bool isDescriptionVisible)
        {
            RoslynSymbolTranslator = roslynSymbolTranslator;
            _isDescriptionVisible = isDescriptionVisible;
            SetProperties(symbol);
        }

        public ModelOrigin Origin
        {
            get { return _origin; }
            set
            {
                if (_origin != value)
                {
                    _origin = value;
                    OnPropertyChanged();
                }
            }
        }

        public ModelNodeStereotype Stereotype
        {
            get { return _stereotype; }
            set
            {
                if (_stereotype != value)
                {
                    _stereotype = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
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

        public void Update([NotNull] ISymbol symbol)
        {
            SetProperties(symbol);
        }

        private void SetProperties([NotNull] ISymbol symbol)
        {
            Origin = symbol.GetOrigin();
            Stereotype = RoslynSymbolTranslator.GetStereotype(symbol);
            Name = symbol.GetName();
            FullName = symbol.GetFullName();
            Description = symbol.GetDescription();
            DescriptionExists = !string.IsNullOrWhiteSpace(Description);
            IsAbstract = symbol.IsAbstract;
        }
    }
}