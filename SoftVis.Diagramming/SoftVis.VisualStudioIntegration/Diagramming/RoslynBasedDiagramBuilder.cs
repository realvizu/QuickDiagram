using System.Collections.Generic;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Builds a diagram from model elements.
    /// </summary>
    public class RoslynBasedDiagramBuilder
    {
        private readonly Diagram _diagram;

        internal RoslynBasedDiagramBuilder(Diagram diagram)
        {
            _diagram = diagram;
        }

        public void ShowModelEntity(IModelEntity modelEntity)
        {
            _diagram.ShowItems(new[] { modelEntity });
        }

        public void ShowModelEntityWithRelatedEntities(IModelEntity modelEntity)
        {
            IEnumerable<IModelItem> modelItems;

            if (modelEntity is RoslynBasedClass)
                modelItems = GetClassWithRelatedEntities((RoslynBasedClass)modelEntity);

            else if (modelEntity is RoslynBasedInterface)
                modelItems = GetInterfaceWithRelatedEntities((RoslynBasedInterface)modelEntity);

            else if (modelEntity is RoslynBasedStruct)
                modelItems = GetStructWithRelatedEntities((RoslynBasedStruct)modelEntity);

            else
                modelItems = new[] { modelEntity };

            _diagram.ShowItems(modelItems);
        }

        private static IEnumerable<IModelItem> GetClassWithRelatedEntities(RoslynBasedClass @class)
        {
             yield return @class;

            foreach (var baseClass in GetBaseClassesRecursive(@class))
                yield return baseClass;

            foreach (var implementedInterface in GetImplementedInterfacesRecursive(@class))
                yield return implementedInterface;

            foreach (var derivedClass in GetDerivedClassesRecursive(@class))
                yield return derivedClass;
        }

        private static IEnumerable<IModelItem> GetInterfaceWithRelatedEntities(RoslynBasedInterface @interface)
        {
            yield return @interface;

            foreach (var baseInterface in GetBaseInterfacesRecursive(@interface))
                yield return baseInterface;

            foreach (var derivedInterface in GetDerivedInterfacesRecursive(@interface))
                yield return derivedInterface;

            foreach (var implementer in GetImplementers(@interface))
                yield return implementer;
        }

        private static IEnumerable<IModelItem> GetStructWithRelatedEntities(RoslynBasedStruct @struct)
        {
            yield return @struct;

            foreach (var implementedInterface in GetImplementedInterfacesRecursive(@struct))
                yield return implementedInterface;
        }

        private static IEnumerable<IModelItem> GetImplementedInterfacesRecursive(dynamic @classOrStruct)
        {
            foreach (var @interface in @classOrStruct.ImplementedInterfaces)
            {
                yield return @interface;

                foreach (var baseInterfaceOfBaseInterface in GetBaseInterfacesRecursive(@interface))
                    yield return baseInterfaceOfBaseInterface;
            }
        }

        private static IEnumerable<IModelItem> GetDerivedClassesRecursive(RoslynBasedClass @class)
        {
            foreach (var childClass in @class.DerivedClasses)
            {
                yield return childClass;

                foreach (var childClassOfChildClass in GetDerivedClassesRecursive(childClass))
                    yield return childClassOfChildClass;
            }
        }

        private static IEnumerable<IModelItem> GetDerivedInterfacesRecursive(RoslynBasedInterface @interface)
        {
            foreach (var @derivedInterface in @interface.DerivedInterfaces)
            {
                yield return @derivedInterface;

                foreach (var derivedInterfaceOfDerivedInterfaces in GetDerivedInterfacesRecursive(@derivedInterface))
                    yield return derivedInterfaceOfDerivedInterfaces;
            }
        }

        private static IEnumerable<IModelItem> GetImplementers(RoslynBasedInterface @interface)
        {
            return @interface.ImplementerTypes;
        }

        private static IEnumerable<IModelItem> GetBaseClassesRecursive(RoslynBasedClass @class)
        {
            if (@class.BaseClass != null)
            {
                yield return @class.BaseClass;

                foreach (var baseClasses in GetBaseClassesRecursive(@class.BaseClass))
                    yield return baseClasses;
            }
        }

        private static IEnumerable<IModelItem> GetBaseInterfacesRecursive(RoslynBasedInterface @interface)
        {
            foreach (var baseInterface in @interface.BaseInterfaces)
            {
                yield return baseInterface;

                foreach (var baseInterfaceOfBase in GetBaseInterfacesRecursive(baseInterface))
                    yield return baseInterfaceOfBase;
            }
        }
    }
}