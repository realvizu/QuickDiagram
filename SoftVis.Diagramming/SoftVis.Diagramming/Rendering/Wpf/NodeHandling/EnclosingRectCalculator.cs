using System.Collections.Generic;
using System.Windows;
using Codartis.SoftVis.Diagramming;

namespace Codartis.SoftVis.Rendering.Wpf.NodeHandling
{
    public static class EnclosingRectCalculator
    {
        public static Rect GetEnclosingRect(IEnumerable<DiagramNode> nodes)
        {
            var enclosingRect = Rect.Empty;

            foreach (var node in nodes)
            {
                var childPosition = node.Position;
                var childSize = node.Size;
                enclosingRect = GetAdjustedEnclosingRect(enclosingRect, childPosition, childSize);
            }

            return enclosingRect;
        }

        private static Rect GetAdjustedEnclosingRect(Rect enclosingRect, DiagramPoint childPosition, DiagramSize childSize)
        {
            if (enclosingRect.IsEmpty)
                return new Rect(new Point(childPosition.X, childPosition.Y), new Size(childSize.Width, childSize.Height));

            if (childPosition.X < enclosingRect.X)
                enclosingRect.X = childPosition.X;

            if (childPosition.Y < enclosingRect.Y)
                enclosingRect.Y = childPosition.Y;

            if (childPosition.X + childSize.Width > enclosingRect.X + enclosingRect.Width)
                enclosingRect.Width = childPosition.X + childSize.Width - enclosingRect.X;

            if (childPosition.Y + childSize.Height > enclosingRect.Y + enclosingRect.Height)
                enclosingRect.Height = childPosition.Y + childSize.Height - enclosingRect.Y;

            return enclosingRect;
        }
    }
}
