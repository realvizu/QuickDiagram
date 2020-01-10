using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.Modeling.Implementation;
using Codartis.SoftVis.VisualStudioIntegration.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Abstract base class for model entities created from Roslyn symbols.
    /// </summary>
    public abstract class RoslynBasedModelEntity : ModelEntity, IRoslynBasedModelEntity
    {
        private readonly TypeKind _typeKind;
        private INamedTypeSymbol _roslynSymbol;

        protected RoslynBasedModelEntity(INamedTypeSymbol roslynSymbol, TypeKind typeKind)
            : base(
                roslynSymbol.GetName(),
                roslynSymbol.GetFullName(),
                roslynSymbol.GetDescription(),
                typeKind.ToModelEntityType(),
                typeKind.ToModelEntityStereotype(),
                roslynSymbol.GetOrigin())
        {
            _typeKind = typeKind;
            RoslynSymbol = roslynSymbol;
        }

        public INamedTypeSymbol RoslynSymbol
        {
            get { return _roslynSymbol; }
            set
            {
                if (value != null && value.TypeKind != _typeKind)
                    throw new ArgumentException($"{value.Name} must be a {_typeKind}.");

                _roslynSymbol = value;
            }
        }

        public bool SymbolEquals(INamedTypeSymbol roslynSymbol) => RoslynSymbol.SymbolEquals(roslynSymbol);

        /// <summary>
        /// Finds and returns related Roslyn symbols.
        /// </summary>
        /// <param name="roslynModelProvider">Query API for the Roslyn model.</param>
        /// <param name="entityRelationType">Optionally specifies what kind of relations should be found. Null means all relations.</param>
        /// <returns>Related Roslyn symbols.</returns>
        public virtual Task<IEnumerable<RoslynSymbolRelation>> FindRelatedSymbolsAsync(
            IRoslynModelProvider roslynModelProvider,
            EntityRelationType? entityRelationType = null)
        {
            return Task.FromResult(Enumerable.Empty<RoslynSymbolRelation>());
        }

        protected static IEnumerable<RoslynSymbolRelation> GetImplementedInterfaces(INamedTypeSymbol classOrStructSymbol)
        {
            foreach (var implementedInterfaceSymbol in classOrStructSymbol.Interfaces.Where(i => i.TypeKind == TypeKind.Interface))
                yield return new RoslynSymbolRelation(
                    classOrStructSymbol,
                    implementedInterfaceSymbol,
                    RoslynEntityRelationTypes.ImplementedInterface);
        }

        protected static async Task<IEnumerable<RoslynSymbolRelation>> GetDerivedTypesAsync(
            IRoslynModelProvider roslynModelProvider,
            INamedTypeSymbol classSymbol)
        {
            var workspace = await roslynModelProvider.GetWorkspaceAsync();

            return (await SymbolFinder.FindDerivedClassesAsync(classSymbol, workspace.CurrentSolution))
                .Where(i => classSymbol.SymbolEquals(i.BaseType.OriginalDefinition) && i.TypeKind == TypeKind.Class)
                .Select(i => new RoslynSymbolRelation(classSymbol, i, EntityRelationTypes.Subtype));
        }

        protected static async Task<IEnumerable<RoslynSymbolRelation>> GetImplementingTypesAsync(
            IRoslynModelProvider roslynModelProvider,
            INamedTypeSymbol interfaceSymbol)
        {
            var workspace = await roslynModelProvider.GetWorkspaceAsync();
            return (await FindImplementingTypesAsync(workspace, interfaceSymbol))
                .Select(i => new RoslynSymbolRelation(interfaceSymbol, i, RoslynEntityRelationTypes.ImplementerType));
        }

        protected static async Task<IEnumerable<RoslynSymbolRelation>> GetDerivedInterfacesAsync(
            IRoslynModelProvider roslynModelProvider,
            INamedTypeSymbol interfaceSymbol)
        {
            var workspace = await roslynModelProvider.GetWorkspaceAsync();
            return (await FindDerivedInterfacesAsync(workspace, interfaceSymbol))
                .Select(i => new RoslynSymbolRelation(interfaceSymbol, i, EntityRelationTypes.Subtype));
        }

        private static async Task<IEnumerable<INamedTypeSymbol>> FindImplementingTypesAsync(Workspace workspace, INamedTypeSymbol interfaceSymbol)
        {
            var result = new List<INamedTypeSymbol>();

            var implementerSymbols = await SymbolFinder.FindImplementationsAsync(interfaceSymbol, workspace.CurrentSolution);
            foreach (var namedTypeSymbol in implementerSymbols.OfType<INamedTypeSymbol>())
            {
                var interfaces = namedTypeSymbol.Interfaces.Select(i => i.OriginalDefinition);
                if (interfaces.Any(i => i.SymbolEquals(interfaceSymbol)))
                    result.Add(namedTypeSymbol);
            }

            // For some reason SymbolFinder does not find implementer structs. So we also make a search with a visitor.

            foreach (var compilation in await GetCompilationsAsync(workspace))
            {
                var visitor = new ImplementingTypesFinderVisitor(interfaceSymbol);
                compilation.Assembly.Accept(visitor);

                foreach (var descendant in visitor.ImplementingTypeSymbols.Where(i => i.TypeKind == TypeKind.Struct))
                    result.Add(descendant);
            }

            return result;
        }

        private static async Task<IEnumerable<INamedTypeSymbol>> FindDerivedInterfacesAsync(Workspace workspace, INamedTypeSymbol interfaceSymbol)
        {
            var result = new List<INamedTypeSymbol>();

            foreach (var compilation in await GetCompilationsAsync(workspace))
            {
                var visitor = new DerivedInterfacesFinderVisitor(interfaceSymbol);
                compilation.Assembly.Accept(visitor);

                foreach (var descendant in visitor.DerivedInterfaces)
                    result.Add(descendant);
            }

            return result;
        }

        private static async Task<IEnumerable<Compilation>> GetCompilationsAsync(Workspace workspace)
        {
            var result = new List<Compilation>();

            foreach (var project in workspace.CurrentSolution.Projects)
                result.Add(await project.GetCompilationAsync());

            return result;
        }
    }
}