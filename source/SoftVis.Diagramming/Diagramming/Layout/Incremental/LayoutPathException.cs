using System;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    [Serializable]
    public class LayoutPathException : Exception
    {
        public LayoutPathException(string message) : base(message)
        { }
    }
}