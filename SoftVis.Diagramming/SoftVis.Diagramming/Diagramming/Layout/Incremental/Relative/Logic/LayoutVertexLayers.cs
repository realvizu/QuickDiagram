using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Common;

namespace Codartis.SoftVis.Diagramming.Layout.Incremental.Relative.Logic
{
    /// <summary>
    /// An ordered collection of LayoutVertexLayer items.
    /// </summary>
    internal class LayoutVertexLayers : IReadOnlyLayoutVertexLayers
    {
        private readonly List<LayoutVertexLayer> _layers;
        private readonly Map<LayoutVertexBase, int?> _vertexToLayerIndexMap;
        private readonly Predicate<LayoutVertexBase> _filterPredicate;
        private readonly List<IReadOnlyLayoutVertexLayer> _filteredLayers;

        public LayoutVertexLayers()
        {
            _layers = new List<LayoutVertexLayer>();
            _vertexToLayerIndexMap = new Map<LayoutVertexBase, int?>();
            _filteredLayers = new List<IReadOnlyLayoutVertexLayer>();
        }

        private LayoutVertexLayers(LayoutVertexLayers original, Predicate<LayoutVertexBase> filterPredicate)
        {
            _layers = original._layers;
            _vertexToLayerIndexMap = original._vertexToLayerIndexMap;
            _filterPredicate = filterPredicate;
            _filteredLayers = _layers.Select(i => i.GetFilteredView(_filterPredicate)).ToList();
        }

        public bool IsFiltered => _filterPredicate != null;

        private IEnumerable<IReadOnlyLayoutVertexLayer> Layers => IsFiltered
            ? _filteredLayers
            : (IEnumerable<IReadOnlyLayoutVertexLayer>)_layers;

        public IReadOnlyLayoutVertexLayers GetFilteredView(Predicate<LayoutVertexBase> predicate)
        {
            return new LayoutVertexLayers(this, predicate);
        }

        public IReadOnlyLayoutVertexLayers GetViewWithoutFloatingItems() => GetFilteredView(i => !i.IsFloating);

        public int Count => Layers.Count();
        public IEnumerator<IReadOnlyLayoutVertexLayer> GetEnumerator() => Layers.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Clear()
        {
            _layers.Clear();
            _vertexToLayerIndexMap.Clear();
            _filteredLayers.Clear();
        }

        public void AddVertex(LayoutVertexBase vertex, RelativeLocation targetLocation)
        {
            var layer = EnsureLayerExists(targetLocation.LayerIndex);
            _vertexToLayerIndexMap.Set(vertex, targetLocation.LayerIndex);

            layer.Add(vertex, targetLocation.IndexInLayer);
        }

        public void RemoveVertex(LayoutVertexBase vertex)
        {
            var layerIndex = GetLayerIndexOrThrow(vertex);
            _layers[layerIndex].Remove(vertex);
            _vertexToLayerIndexMap.Remove(vertex);
        }

        // TODO: move to VerticalPositionLogic class?
        public void UpdateLayerVerticalPositions(double verticalGap)
        {
            for (var i = 0; i < Layers.Count(); i++)
            {
                Layers.ElementAt(i).Top = (i == 0)
                    ? 0
                    : Layers.ElementAt(i - 1).Bottom + verticalGap;
            }
        }

        public int? GetLayerIndex(LayoutVertexBase vertex)
        {
            return IsFiltered && !_filterPredicate(vertex) 
                ? null 
                : _vertexToLayerIndexMap.Get(vertex);
        }

        public int GetLayerIndexOrThrow(LayoutVertexBase vertex)
        {
            var layerIndex = GetLayerIndex(vertex);
            if (layerIndex == null)
                throw new InvalidOperationException($"Vertex {vertex} has no layer index.");
            return layerIndex.Value;
        }

        public RelativeLocation? GetLocation(LayoutVertexBase vertex)
        {
            var layerIndex = GetLayerIndex(vertex);

            return layerIndex == null
                ? (RelativeLocation?)null
                : new RelativeLocation(layerIndex.Value, GetIndexInLayer(vertex));
        }

        public RelativeLocation GetLocationOrThrow(LayoutVertexBase vertex)
        {
            var location = GetLocation(vertex);
            if (location == null)
                throw new InvalidOperationException($"Vertex {vertex} has no relative location.");
            return location.Value;
        }

        public IReadOnlyLayoutVertexLayer GetLayer(int index)
        {
            EnsureLayerExists(index);
            return Layers.ElementAt(index);
        }

        public IReadOnlyLayoutVertexLayer GetLayer(LayoutVertexBase vertex)
        {
            var layerIndex = GetLayerIndexOrThrow(vertex);
            return Layers.ElementAt(layerIndex);
        }

        public int GetIndexInLayer(LayoutVertexBase vertex)
        {
            return GetLayer(vertex).IndexOf(vertex);
        }

        public LayoutVertexBase GetPreviousInLayer(LayoutVertexBase vertex)
        {
            return GetLayer(vertex).GetPrevious(vertex);
        }

        public LayoutVertexBase GetNextInLayer(LayoutVertexBase vertex)
        {
            return GetLayer(vertex).GetNext(vertex);
        }

        // TODO: eliminate
        public IEnumerable<LayoutVertexBase> GetOtherPlacedVerticesInLayer(LayoutVertexBase vertex)
        {
            return GetLayer(vertex).Where(i => i != vertex && !i.IsFloating);
        }

        private LayoutVertexLayer EnsureLayerExists(int layerIndex)
        {
            for (var i = _layers.Count; i <= layerIndex; i++)
            {
                var newLayer = new LayoutVertexLayer(i);
                _layers.Add(newLayer);

                if (IsFiltered)
                    _filteredLayers.Add(newLayer.GetFilteredView(_filterPredicate));
            }
            return _layers[layerIndex];
        }
    }
}
