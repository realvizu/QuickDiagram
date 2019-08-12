using System.Collections.Generic;
using Codartis.SoftVis.Modeling.Definition;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    public delegate void RelatedNodeMiniButtonEventHandler(
        RelatedNodeMiniButtonViewModel diagramNodeButtonViewModel,
        IReadOnlyList<IModelNode> modelNodes);
}
