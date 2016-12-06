using System.Collections.Generic;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    public delegate void ShowRelatedNodeButtonEventHandler(ShowRelatedNodeButtonViewModel diagramNodeButtonViewModel,
        IEnumerable<IModelEntity> modelEntities);
}
