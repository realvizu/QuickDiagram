using System;

namespace Codartis.SoftVis.Diagramming.Layout.Sugiyama
{
    [Serializable]
    internal class LayoutPathException : Exception
    {
        public LayoutPathException(string message) : base(message)
        { }
    }
}