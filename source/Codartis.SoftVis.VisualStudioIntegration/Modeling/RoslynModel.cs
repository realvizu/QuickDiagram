//using System.Collections.Generic;
//using System.Linq;
//using Codartis.SoftVis.Modeling.Definition;
//using Codartis.SoftVis.VisualStudioIntegration.Util;
//using Codartis.Util;
//using JetBrains.Annotations;
//using Microsoft.CodeAnalysis;

//namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
//{
//    public sealed class RoslynModel
//    {
//        [NotNull] public IModel Model { get; }

//        public RoslynModel([NotNull] IModel model)
//        {
//            Model = model;
//        }

//        [NotNull] public IEnumerable<RoslynModelNode> RoslynModelNodes => Model.Nodes.Select(i => new RoslynModelNode(i));

//        public Maybe<RoslynModelNode> TryGetNodeBySymbol(ISymbol symbol)
//        {
//            return RoslynModelNodes
//                .FirstOrDefault(i => i.RoslynNode.UnderlyingSymbol.SymbolEquals(symbol))
//                .ToMaybe();
//        }

//        //public bool RelationshipExists(
//        //    ISymbol sourceSymbol,
//        //    ModelNodeId targetNodeId,
//        //    ModelRelationshipStereotype stereotype)
//        //{
//        //    return Model.GetRelationship(sourceNode, targetNode, stereotype) != null;
//        //}

//        //public IModelRelationship GetRelationship(
//        //    IRoslynNode sourceNode,
//        //    IRoslynNode targetNode,
//        //    ModelRelationshipStereotype stereotype)
//        //{
//        //    return Model.Relationships.FirstOrDefault(i => i.Source == sourceNode && i.Target == targetNode && i.Stereotype == stereotype);
//        //}

//        public Maybe<RoslynModelNode> TryGetNodeByLocation(Location location)
//        {
//            if (location == null)
//                return Maybe<RoslynModelNode>.Nothing;

//            var fileLinePositionSpan = location.GetMappedLineSpan();

//            foreach (var roslynModelNode in RoslynModelNodes)
//            {
//                var nodeLocation = roslynModelNode.RoslynNode?.UnderlyingSymbol.Locations.FirstOrDefault()?.GetMappedLineSpan();
//                if (nodeLocation?.Overlaps(fileLinePositionSpan) == true)
//                    return Maybe.Create(roslynModelNode);
//            }

//            return Maybe<RoslynModelNode>.Nothing;
//        }

//    }
//}