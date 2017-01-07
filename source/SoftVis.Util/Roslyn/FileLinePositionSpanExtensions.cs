using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.Util.Roslyn
{
    public static class FileLinePositionSpanExtensions
    {
        public static bool Overlaps(this FileLinePositionSpan span1, FileLinePositionSpan span2)
        {
            return span1.IsValid && span2.IsValid
                   && span1.Path == span2.Path
                   && span1.Span.Overlaps(span2.Span);
        }
    }
}
