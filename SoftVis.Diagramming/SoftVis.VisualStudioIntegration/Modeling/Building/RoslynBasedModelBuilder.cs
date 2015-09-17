using System;
using System.Collections.Generic;
using Codartis.SoftVis.Modeling;
using Codartis.SoftVis.VisualStudioIntegration.WorkspaceContext;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Building
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

            return GetOrAddWithRelatedSymbols(namedTypeSymbol);
        }

        private IModelEntity GetOrAddWithRelatedSymbols(INamedTypeSymbol namedTypeSymbol)
        {
            switch (namedTypeSymbol.TypeKind)
            {
                case TypeKind.Class:
                    return GetOrAddClassWithRelatedSymbols(namedTypeSymbol);
                case TypeKind.Interface:
                    return GetOrAddInterfaceWithRelatedSymbols(namedTypeSymbol);
                case TypeKind.Struct:
                    return GetOrAddStructWithRelatedSymbols(namedTypeSymbol);
                case TypeKind.Enum:
                    return Model.GetOrAddEntity(namedTypeSymbol);
                case TypeKind.Delegate:
                    return Model.GetOrAddEntity(namedTypeSymbol);
                default:
                    throw new Exception($"Unexpected TypeKind: {namedTypeSymbol.TypeKind}");
            }
        }

        private RoslynBasedModelEntity GetOrAddClassWithRelatedSymbols(INamedTypeSymbol classSymbol)
        {
            var newEntity = Model.GetOrAddEntity(classSymbol);
            AddBaseType(classSymbol);
            AddImplementedInterfaces(classSymbol);
            AddDerivedTypes(classSymbol);
            return newEntity;
        }

        private RoslynBasedModelEntity GetOrAddInterfaceWithRelatedSymbols(INamedTypeSymbol interfaceSymbol)
        {
            var newEntity = Model.GetOrAddEntity(interfaceSymbol);
            AddImplementedInterfaces(interfaceSymbol);
            AddDerivedInterface(interfaceSymbol);
            AddImplementingTypes(interfaceSymbol);
            return newEntity;
        }

        private RoslynBasedModelEntity GetOrAddStructWithRelatedSymbols(INamedTypeSymbol structSymbol)
        {
            var newEntity = Model.GetOrAddEntity(structSymbol);
            AddImplementedInterfaces(structSymbol);
            return newEntity;
        }

        private void AddBaseType(INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol.BaseType != null)
            {
                Model.GetOrAddEntity(namedTypeSymbol.BaseType);
                Model.GetOrAddRelationship(namedTypeSymbol, namedTypeSymbol.BaseType, ModelRelationshipType.Generalization);
                AddBaseType(namedTypeSymbol.BaseType);
            }
        }

        private void AddImplementedInterfaces(INamedTypeSymbol namedTypeSymbol)
        {
            foreach (var interfaceSymbol in namedTypeSymbol.Interfaces)
            {
                Model.GetOrAddEntity(interfaceSymbol);
                Model.GetOrAddRelationship(namedTypeSymbol, interfaceSymbol, 
                    ModelRelationshipType.Generalization, RoslynBasedModelRelationshipStereotype.Implementation);
                AddImplementedInterfaces(interfaceSymbol);
            }
        }

        private void AddDerivedTypes(INamedTypeSymbol namedTypeSymbol)
        {
            foreach (var childTypeSymbol in GetDerivedTypeSymbols(namedTypeSymbol))
            {
                Model.GetOrAddEntity(childTypeSymbol);
                Model.GetOrAddRelationship(childTypeSymbol, namedTypeSymbol, ModelRelationshipType.Generalization);
                AddDerivedTypes(childTypeSymbol);
            }
        }

        private IEnumerable<INamedTypeSymbol> GetDerivedTypeSymbols(INamedTypeSymbol namedTypeSymbol)
        {
            var workspace = WorkspaceServices.GetWorkspace();
            return FindDerivedTypesAsync(workspace, namedTypeSymbol);
        }

        private static IEnumerable<INamedTypeSymbol> FindDerivedTypesAsync(Workspace workspace, INamedTypeSymbol namedTypeSymbol)
        {
            foreach (var project in workspace.CurrentSolution.Projects)
            {
                var compilation = project.GetCompilationAsync().Result;
                var visitor = new DerivedTypesFinderVisitor(namedTypeSymbol);
                compilation.Assembly.Accept(visitor);
                foreach (var descendant in visitor.DerivedTypeSymbols)
                    yield return descendant;
            }
        }

        private void AddImplementingTypes(INamedTypeSymbol interfaceSymbol)
        {
            foreach (var implementingTypeSymbols in GetImplementingTypeSymbols(interfaceSymbol))
            {
                if (implementingTypeSymbols.TypeKind == TypeKind.Class ||
                    implementingTypeSymbols.TypeKind == TypeKind.Struct)
                {
                    Model.GetOrAddEntity(implementingTypeSymbols);
                    Model.GetOrAddRelationship(implementingTypeSymbols, interfaceSymbol,
                        ModelRelationshipType.Generalization, RoslynBasedModelRelationshipStereotype.Implementation);
                }
            }
        }

        private void AddDerivedInterface(INamedTypeSymbol interfaceSymbol)
        {
            foreach (var implementingTypeSymbols in GetImplementingTypeSymbols(interfaceSymbol))
            {
                if (implementingTypeSymbols.TypeKind == TypeKind.Interface)
                {
                    Model.GetOrAddEntity(implementingTypeSymbols);
                    Model.GetOrAddRelationship(implementingTypeSymbols, interfaceSymbol, ModelRelationshipType.Generalization);
                }
            }
        }

        private IEnumerable<INamedTypeSymbol> GetImplementingTypeSymbols(INamedTypeSymbol namedTypeSymbol)
        {
            var workspace = WorkspaceServices.GetWorkspace();
            return FindImplementingTypesAsync(workspace, namedTypeSymbol);
        }

        private static IEnumerable<INamedTypeSymbol> FindImplementingTypesAsync(Workspace workspace, INamedTypeSymbol namedTypeSymbol)
        {
            foreach (var project in workspace.CurrentSolution.Projects)
            {
                var compilation = project.GetCompilationAsync().Result;
                var visitor = new ImplementingTypesFinderVisitor(namedTypeSymbol);
                compilation.Assembly.Accept(visitor);
                foreach (var descendant in visitor.ImplementingTypeSymbols)
                    yield return descendant;
            }
        }
    }
}