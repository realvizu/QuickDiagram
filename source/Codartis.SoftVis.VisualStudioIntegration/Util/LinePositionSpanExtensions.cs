using Microsoft.CodeAnalysis.Text;

namespace Codartis.SoftVis.VisualStudioIntegration.Util
{
    public static class LinePositionSpanExtensions
    {
        public static bool Overlaps(this LinePositionSpan span1, LinePositionSpan span2)
        {
            return span2.Start <= span1.End && span1.Start <= span2.End;
        }
    }
}
