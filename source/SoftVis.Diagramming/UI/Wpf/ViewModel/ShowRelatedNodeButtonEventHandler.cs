using System.Collections.Generic;
using Codartis.SoftVis.Modeling2;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    public delegate void ShowRelatedNodeButtonEventHandler(ShowRelatedNodeButtonViewModel diagramNodeButtonViewModel,
        IReadOnlyList<IModelNode> modelNodes);
}
