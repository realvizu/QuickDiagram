using System;

namespace Codartis.SoftVis.Diagramming.Layout
{
    internal interface IIncrementalLayoutEngine : IDisposable
    {
        void EnqueueDiagramAction(DiagramAction diagramAction);
    }
}