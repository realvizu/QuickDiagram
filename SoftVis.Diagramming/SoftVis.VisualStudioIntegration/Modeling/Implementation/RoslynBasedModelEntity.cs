using System;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Abstract base class for model entities created from Roslyn symbols.
    /// Capable of finding related symbols in the Roslyn model (API).
    /// </summary>
    public abstract class RoslynBasedModelEntity : ModelEntity, IRoslynBasedModelEntity
    {
        public INamedTypeSymbol RoslynSymbol { get; }

        protected RoslynBasedModelEntity(INamedTypeSymbol roslynSymbol, TypeKind typeKind)
            : base(roslynSymbol.GetMinimallyQualifiedName(), typeKind.ToModelEntityType(), typeKind.ToModelEntityStereotype())
        {
            if (roslynSymbol.TypeKind != typeKind)
                throw new ArgumentException($"{roslynSymbol.Name} must be a {typeKind}.");

            RoslynSymbol = roslynSymbol;
        }

        /// <summary>
        /// Finds and returns related Roslyn symbols.
        /// </summary>
        /// <param name="roslynModelProvider">Query API for the Roslyn model.</param>
        /// <param name="relatedEntitySpecification">Optionally specifies what kind of relations should be found. Null means all relations.</param>
        /// <returns>Related Roslyn symbols.</returns>
        public virtual IEnumerable<RoslynSymbolRelation> FindRelatedSymbols(IRoslynModelProvider roslynModelProvider,
            RelatedEntitySpecification? relatedEntitySpecification = null) => Enumerable.Empty<RoslynSymbolRelation>();

        protected static IEnumerable<RoslynSymbolRelation> GetImplementedInterfaces(INamedTypeSymbol classOrStructSymbol)
        {
            foreach (var implementedInterfaceSymbol in classOrStructSymbol.Interfaces)
                yield return new RoslynSymbolRelation(classOrStructSymbol, implementedInterfaceSymbol,
                    RoslynRelatedEntitySpecifications.ImplementedInterface);
        }

        protected static IEnumerable<RoslynSymbolRelation> GetDerivedTypes(
            IRoslynModelProvider roslynModelProvider, INamedTypeSymbol classSymbol)
        {
            var workspace = roslynModelProvider.GetWorkspace();
            return FindDerivedTypes(workspace, classSymbol).
                Select(i=> new RoslynSymbolRelation(classSymbol, i, RelatedEntitySpecifications.Subtype));
        }

        protected static IEnumerable<RoslynSymbolRelation> GetImplementingTypes(
            IRoslynModelProvider roslynModelProvider, INamedTypeSymbol interfaceSymbol)
        {
            var workspace = roslynModelProvider.GetWorkspace();
            return FindImplementingTypes(workspace, interfaceSymbol)
                .Where(i => i.TypeKind != TypeKind.Interface)
                .Select(i => new RoslynSymbolRelation(interfaceSymbol, i, RoslynRelatedEntitySpecifications.ImplementerType));
        }

        protected static IEnumerable<RoslynSymbolRelation> GetDerivedInterfaces(
            IRoslynModelProvider roslynModelProvider, INamedTypeSymbol interfaceSymbol)
        {
            var workspace = roslynModelProvider.GetWorkspace();
            return FindImplementingTypes(workspace, interfaceSymbol)
                .Where(i => i.TypeKind == TypeKind.Interface)
                .Select(i => new RoslynSymbolRelation(interfaceSymbol, i, RelatedEntitySpecifications.Subtype));
        }

        private static IEnumerable<INamedTypeSymbol> FindDerivedTypes(Workspace workspace, INamedTypeSymbol classSymbol)
        {
            //return SymbolFinder.FindDerivedClassesAsync(classSymbol, workspace.CurrentSolution).Result;

            foreach (var compilation in GetCompilations(workspace))
            {
                var visitor = new DerivedTypesFinderVisitor(classSymbol);
                compilation.Assembly.Accept(visitor);

                foreach (var descendant in visitor.DerivedTypeSymbols)
                    yield return descendant;
            }
        }

        private static IEnumerable<INamedTypeSymbol> FindImplementingTypes(Workspace workspace, INamedTypeSymbol interfaceSymbol)
        {
            foreach (var compilation in GetCompilations(workspace))
            {
                var visitor = new ImplementingTypesFinderVisitor(interfaceSymbol);
                compilation.Assembly.Accept(visitor);

                foreach (var descendant in visitor.ImplementingTypeSymbols)
                    yield return descendant;
            }
        }

        private static IEnumerable<Compilation> GetCompilations(Workspace workspace)
        {
            foreach (var project in workspace.CurrentSolution.Projects)
                yield return project.GetCompilationAsync().Result;
        }
    }
}