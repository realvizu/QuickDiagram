using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;

namespace Codartis.SoftVis.Modeling.Definition
{
    /// <summary>
    /// Describes a change in a model by presenting the new model state and the item changes that led to it.
    /// </summary>
    public struct ModelEvent
    {
        [NotNull] public IModel NewModel { get; }
        [NotNull] [ItemNotNull] public IEnumerable<ModelItemEventBase> ItemEvents { get; }

        public ModelEvent(
            [NotNull] IModel newModel,
            [NotNull] [ItemNotNull] IEnumerable<ModelItemEventBase> itemEvents = null)
        {
            NewModel = newModel;
            ItemEvents = itemEvents ?? Enumerable.Empty<ModelItemEventBase>();
        }

        public static ModelEvent None([NotNull] IModel model) => new ModelEvent(model);

        public static ModelEvent Create(
            [NotNull] IModel model,
            [NotNull] [ItemNotNull] IEnumerable<ModelItemEventBase> itemEvents)
        {
            return new ModelEvent(model, itemEvents);
        }

        public static ModelEvent Create([NotNull] IModel model, ModelItemEventBase itemEvent = null)
        {
            var itemChanges = itemEvent == null
                ? Enumerable.Empty<ModelItemEventBase>()
                : new[] { itemEvent };

            return new ModelEvent(model, itemChanges);
        }
    }
}