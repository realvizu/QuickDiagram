using System;
using System.Collections.Generic;
using System.Linq;
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
            EnsureSymbolTypeKind(classSymbol, TypeKind.Class);

            var newEntity = Model.GetOrAddEntity(classSymbol);
            AddBaseClass(classSymbol);
            AddImplementedInterfaces(classSymbol);
            AddDerivedTypes(classSymbol);
            return newEntity;
        }

        private RoslynBasedModelEntity GetOrAddInterfaceWithRelatedSymbols(INamedTypeSymbol interfaceSymbol)
        {
            EnsureSymbolTypeKind(interfaceSymbol, TypeKind.Interface);

            var newEntity = Model.GetOrAddEntity(interfaceSymbol);
            AddBaseInterfaces(interfaceSymbol);
            AddDerivedInterfaces(interfaceSymbol);
            AddImplementingTypes(interfaceSymbol);
            return newEntity;
        }

        private RoslynBasedModelEntity GetOrAddStructWithRelatedSymbols(INamedTypeSymbol structSymbol)
        {
            EnsureSymbolTypeKind(structSymbol, TypeKind.Struct);

            var newEntity = Model.GetOrAddEntity(structSymbol);
            AddImplementedInterfaces(structSymbol);
            return newEntity;
        }

        private void AddBaseClass(INamedTypeSymbol classSymbol)
        {
            EnsureSymbolTypeKind(classSymbol, TypeKind.Class);

            if (classSymbol.BaseType != null)
            {
                Model.GetOrAddEntity(classSymbol.BaseType);
                Model.GetOrAddRelationship(classSymbol, classSymbol.BaseType, ModelRelationshipType.Generalization);
                AddBaseClass(classSymbol.BaseType);
            }
        }

        private void AddImplementedInterfaces(INamedTypeSymbol classOrStructSymbol)
        {
            EnsureSymbolTypeKind(classOrStructSymbol, TypeKind.Class, TypeKind.Struct);

            foreach (var implementedInterfaceSymbol in classOrStructSymbol.Interfaces)
            {
                Model.GetOrAddEntity(implementedInterfaceSymbol);
                Model.GetOrAddRelationship(classOrStructSymbol, implementedInterfaceSymbol, 
                    ModelRelationshipType.Generalization, RoslynBasedModelRelationshipStereotype.Implementation);
                AddBaseInterfaces(implementedInterfaceSymbol);
            }
        }

        private void AddBaseInterfaces(INamedTypeSymbol interfaceSymbol)
        {
            EnsureSymbolTypeKind(interfaceSymbol, TypeKind.Interface);

            foreach (var baseInterfaceSymbol in interfaceSymbol.Interfaces)
            {
                Model.GetOrAddEntity(baseInterfaceSymbol);
                Model.GetOrAddRelationship(interfaceSymbol, baseInterfaceSymbol, ModelRelationshipType.Generalization);
                AddBaseInterfaces(baseInterfaceSymbol);
            }
        }

        private void AddDerivedTypes(INamedTypeSymbol classSymbol)
        {
            EnsureSymbolTypeKind(classSymbol, TypeKind.Class);

            foreach (var derivedTypeSymbol in GetDerivedTypeSymbols(classSymbol))
            {
                Model.GetOrAddEntity(derivedTypeSymbol);
                Model.GetOrAddRelationship(derivedTypeSymbol, classSymbol, ModelRelationshipType.Generalization);
                AddDerivedTypes(derivedTypeSymbol);
            }
        }

        private IEnumerable<INamedTypeSymbol> GetDerivedTypeSymbols(INamedTypeSymbol classSymbol)
        {
            EnsureSymbolTypeKind(classSymbol, TypeKind.Class);

            var workspace = WorkspaceServices.GetWorkspace();
            return FindDerivedTypesAsync(workspace, classSymbol);
        }

        private static IEnumerable<INamedTypeSymbol> FindDerivedTypesAsync(Workspace workspace, INamedTypeSymbol classSymbol)
        {
            EnsureSymbolTypeKind(classSymbol, TypeKind.Class);

            foreach (var project in workspace.CurrentSolution.Projects)
            {
                var compilation = project.GetCompilationAsync().Result;
                var visitor = new DerivedTypesFinderVisitor(classSymbol);
                compilation.Assembly.Accept(visitor);
                foreach (var descendant in visitor.DerivedTypeSymbols)
                    yield return descendant;
            }
        }

        private void AddImplementingTypes(INamedTypeSymbol interfaceSymbol)
        {
            EnsureSymbolTypeKind(interfaceSymbol, TypeKind.Interface);

            foreach (var implementingTypeSymbols in GetImplementingTypeSymbols(interfaceSymbol))
            {
                // The relationship between an interface and a derived interface 
                // is modeled as generalization, not implementation (no stereotype!).
                // So here we want to find non-interface children only.
                if (implementingTypeSymbols.TypeKind != TypeKind.Interface)
                {
                    Model.GetOrAddEntity(implementingTypeSymbols);
                    Model.GetOrAddRelationship(implementingTypeSymbols, interfaceSymbol,
                        ModelRelationshipType.Generalization, RoslynBasedModelRelationshipStereotype.Implementation);
                }
            }
        }

        private void AddDerivedInterfaces(INamedTypeSymbol interfaceSymbol)
        {
            EnsureSymbolTypeKind(interfaceSymbol, TypeKind.Interface);

            foreach (var implementingTypeSymbols in GetImplementingTypeSymbols(interfaceSymbol))
            {
                if (implementingTypeSymbols.TypeKind == TypeKind.Interface)
                {
                    Model.GetOrAddEntity(implementingTypeSymbols);
                    Model.GetOrAddRelationship(implementingTypeSymbols, interfaceSymbol, ModelRelationshipType.Generalization);
                }
            }
        }

        private IEnumerable<INamedTypeSymbol> GetImplementingTypeSymbols(INamedTypeSymbol interfaceSymbol)
        {
            EnsureSymbolTypeKind(interfaceSymbol, TypeKind.Interface);

            var workspace = WorkspaceServices.GetWorkspace();
            return FindImplementingTypesAsync(workspace, interfaceSymbol);
        }

        private static IEnumerable<INamedTypeSymbol> FindImplementingTypesAsync(Workspace workspace, INamedTypeSymbol interfaceSymbol)
        {
            EnsureSymbolTypeKind(interfaceSymbol, TypeKind.Interface);

            foreach (var project in workspace.CurrentSolution.Projects)
            {
                var compilation = project.GetCompilationAsync().Result;
                var visitor = new ImplementingTypesFinderVisitor(interfaceSymbol);
                compilation.Assembly.Accept(visitor);
                foreach (var descendant in visitor.ImplementingTypeSymbols)
                    yield return descendant;
            }
        }

        private static void EnsureSymbolTypeKind(INamedTypeSymbol symbol, params TypeKind[] expectedTypeKinds)
        {
            if (expectedTypeKinds.Any(i => symbol.TypeKind == i))
                return;

            throw new InvalidOperationException($"Unexpected symbol type: {symbol.TypeKind}");
        }
    }
}