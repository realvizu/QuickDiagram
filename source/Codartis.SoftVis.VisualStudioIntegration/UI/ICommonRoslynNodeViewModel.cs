using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.VisualStudioIntegration.UI
{
    /// <summary>
    /// A view model that exposes the most common Roslyn diagram node properties.
    /// </summary>
    public interface ICommonRoslynNodeViewModel
    {
        string Name { get; }
        string FullName { get; }
        ModelNodeStereotype Stereotype { get; }
        bool IsStatic { get; }
    }
}