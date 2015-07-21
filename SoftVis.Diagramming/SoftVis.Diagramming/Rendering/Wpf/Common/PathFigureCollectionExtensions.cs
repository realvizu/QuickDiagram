using System.Collections.Generic;
using System.Windows.Media;

namespace Codartis.SoftVis.Rendering.Wpf.Common
{
    public static class PathFigureCollectionExtensions
    {
        public static void Add(this PathFigureCollection pathFigureCollection, IEnumerable<PathFigure> otherPathFigureCollection)
        {
            foreach (var pathFigure in otherPathFigureCollection)
                pathFigureCollection.Add(pathFigure);
        }
    }
}
