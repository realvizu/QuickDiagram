using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.UI;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation;
using Codartis.Util.UI.Wpf.ViewModels;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    public abstract class RoslynSymbolViewModelBase : ViewModelBase, IPayloadUi
    {
        public ModelNodeStereotype Stereotype { get; }
        public ModelOrigin Origin { get; }
        public string Name { get; }
        public string FullName { get; }
        public string Description { get; }
        public bool DescriptionExists { get; }
        public bool IsAbstract { get; }

        protected RoslynSymbolViewModelBase([NotNull] ISymbol symbol)
        {
            Stereotype = symbol.GetStereotype();
            Origin = symbol.GetOrigin();
            Name = symbol.GetName();
            FullName = symbol.GetFullName();
            Description = symbol.GetDescription();
            DescriptionExists = !string.IsNullOrWhiteSpace(Description);
            IsAbstract = false;
        }
    }
}