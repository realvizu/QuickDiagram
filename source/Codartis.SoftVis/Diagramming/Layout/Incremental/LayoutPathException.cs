using System;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental
{
    [Serializable]
    internal class LayoutPathException : Exception
    {
        public LayoutPathException(string message) : base(message)
        { }
    }
}