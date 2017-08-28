using System.Collections.Generic;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.UI
{
    public delegate void ShowModelItemsEventHandler(IReadOnlyList<IModelNode> modelNodes, bool followNewDiagramNodes);
}
