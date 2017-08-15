using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming.UnitTests.TestSubjects;
using Codartis.SoftVis.Util;
using FluentAssertions;
using Xunit;

namespace Codartis.SoftVis.Diagramming.UnitTests.Modeling.Implementation
{
    public class ModelTests
    {
        //[Fact]
        //public void GetOrAddRelationship_IsThreadSafe()
        //{
        //    const int repeat = 100;

        //    var model = new Model();

        //    var sourceEntity = model.GetOrAddEntity(i => true, () => new TestModelEntity());

        //    for (int i = 0; i < repeat; i++)
        //    {
        //        var testModelEntity = new TestModelEntity(i.ToString());
        //        model.GetOrAddEntity(testModelEntity);
        //    }

        //    var relationshipType = new ModelRelationshipType(ModelRelationshipClassifier.Generalization, ModelRelationshipStereotype.None);

        //    var tasks = new List<Task>
        //    {
        //        Task.Run(() => GetOrAddRelationship(model, sourceEntity, relationshipType, repeat)),
        //        Task.Run(() => GetOrAddRelationship(model, sourceEntity, relationshipType, repeat)),
        //        Task.Run(() => GetOrAddRelationship(model, sourceEntity, relationshipType, repeat)),
        //    };

        //    Task.WaitAll(tasks.ToArray());

        //    model.Relationships.Count.ShouldBeEquivalentTo(repeat);
        //}

        //private static void GetOrAddRelationship(Model model, IModelEntity sourceEntity, ModelRelationshipType relationshipType, int repeat)
        //{
        //    var targetEntities = model.Entities.Except(sourceEntity.ToEnumerable()).ToArray();

        //    for (int i = 0; i < repeat; i++)
        //    {
        //        var relationship = model.GetOrAddRelationship(sourceEntity, targetEntities[i], relationshipType);
        //        if (relationship == null)
        //            throw new Exception("Relationship is null.");
        //    }
        //}
    }
}
