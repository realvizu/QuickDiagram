using System.Collections.Generic;
using System.Windows;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Util.UI;

namespace Codartis.SoftVis.UI.Wpf.ViewModel
{
    public delegate void EntitySelectorRequestedEventHandler(Point attachPoint, 
        HandleOrientation handleOrientation, IEnumerable<IModelEntity> modelEntities);
}
