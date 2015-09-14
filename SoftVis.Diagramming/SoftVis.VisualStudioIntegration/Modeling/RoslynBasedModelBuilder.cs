using System.Collections.Generic;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.WorkspaceContext;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling
{
    /// <summary>
    /// Builds a simplified model based on Roslyn-provided info.
    /// </summary>
    public class RoslynBasedModelBuilder : IModelServices
    {
        private IWorkspaceServices WorkspaceServices { get; }
        private RoslynBasedModel Model { get; }

        internal RoslynBasedModelBuilder(IWorkspaceServices workspaceServices)
        {
            WorkspaceServices = workspaceServices;
            Model = new RoslynBasedModel();
        }

        public IModelEntity GetModelEntity(ISymbol symbol)
        {
            var namedTypeSymbol = symbol as INamedTypeSymbol;
            if (namedTypeSymbol == null)
                return null;

            return GetOrAddSymbolWithBaseAndChildren(namedTypeSymbol);
        }

        private IModelEntity GetOrAddSymbolWithBaseAndChildren(INamedTypeSymbol namedTypeSymbol)
        {
            GetOrAddBase(namedTypeSymbol);
            var modelEntity = GetOrAddSymbol(namedTypeSymbol);
            GetOrAddChildren(namedTypeSymbol);

            return modelEntity;
        }

        private void GetOrAddChildren(INamedTypeSymbol namedTypeSymbol)
        {
            var childTypeSymbols = GetChildTypeSymbols(namedTypeSymbol);
            foreach (var childTypeSymbol in childTypeSymbols)
            {
                GetOrAddSymbol(childTypeSymbol);
                GetOrAddChildren(childTypeSymbol);
            }
        }

        private void GetOrAddBase(INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol.BaseType != null)
            {
                GetOrAddBase(namedTypeSymbol.BaseType);
                GetOrAddSymbol(namedTypeSymbol.BaseType);
            }
        }

        private IModelEntity GetOrAddSymbol(INamedTypeSymbol namedTypeSymbol)
        {
           return Model.GetOrAdd(namedTypeSymbol);
        }

        private IEnumerable<INamedTypeSymbol> GetChildTypeSymbols(ISymbol symbol)
        {
            var namedTypeSymbol = symbol as INamedTypeSymbol;
            if (namedTypeSymbol == null)
                return null;

            var workspace = WorkspaceServices.GetWorkspace();
            var childTypeSymbols = FindChildTypesAsync(workspace, namedTypeSymbol);
            return childTypeSymbols;
        }

        private static IEnumerable<INamedTypeSymbol> FindChildTypesAsync(Workspace workspace, INamedTypeSymbol namedTypeSymbol)
        {
            foreach (var project in workspace.CurrentSolution.Projects)
            {
                var compilation = project.GetCompilationAsync().Result;
                var visitor = new DescendantsFinderVisitor(namedTypeSymbol);
                compilation.Assembly.Accept(visitor);
                foreach (var descendant in visitor.Descendants)
                    yield return descendant;
            }
        }
    }
}