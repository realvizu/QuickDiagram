using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// A simple implementation of the IModelEntity interface.
    /// </summary>
    [DebuggerDisplay("{Name} ({Type}/{Stereotype})")]
    public abstract class ModelEntity : IModelEntity
    {
        private readonly List<ModelRelationship> _outgoingRelationships = new List<ModelRelationship>();
        private readonly List<ModelRelationship> _incomingRelationships = new List<ModelRelationship>();

        public string Name { get; }
        public ModelEntityType Type { get; }
        public ModelEntityStereotype Stereotype { get; }
        public abstract int Priority { get; }
        public virtual bool IsAbstract => false;

        protected ModelEntity(string name, ModelEntityType type, ModelEntityStereotype stereotype = null)
        {
            Name = name;
            Type = type;
            Stereotype = stereotype;
        }

        public IEnumerable<IModelRelationship> OutgoingRelationships => _outgoingRelationships;
        public IEnumerable<IModelRelationship> IncomingRelationships => _incomingRelationships;
        public IEnumerable<IModelRelationship> AllRelationships => _incomingRelationships.Union(_outgoingRelationships);

        public void AddOutgoingRelationship(ModelRelationship relationship)
        {
            _outgoingRelationships.Add(relationship);
        }

        public void AddIncomingRelationship(ModelRelationship relationship)
        {
            _incomingRelationships.Add(relationship);
        }
    }
}
