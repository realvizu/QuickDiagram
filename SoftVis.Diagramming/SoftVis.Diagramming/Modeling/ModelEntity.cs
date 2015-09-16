using System.Collections.Generic;
using System.Diagnostics;

namespace Codartis.SoftVis.Modeling
{
    /// <summary>
    /// A simple implementation of the IModelEntity interface.
    /// </summary>
    [DebuggerDisplay("{Name} ({Type}/{Stereotype})")]
    public class ModelEntity : IModelEntity
    {
        private readonly List<ModelRelationship> _outgoingRelationships = new List<ModelRelationship>();
        private readonly List<ModelRelationship> _incomingRelationships = new List<ModelRelationship>();

        public string Name { get; }
        public ModelEntityType Type { get; }
        public ModelEntityStereotype Stereotype { get; }

        public ModelEntity(string name, ModelEntityType type, ModelEntityStereotype stereotype = null)
        {
            Name = name;
            Type = type;
            Stereotype = stereotype;
        }

        public IEnumerable<IModelRelationship> OutgoingRelationships => _outgoingRelationships;
        public IEnumerable<IModelRelationship> IncomingRelationships => _incomingRelationships;

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
