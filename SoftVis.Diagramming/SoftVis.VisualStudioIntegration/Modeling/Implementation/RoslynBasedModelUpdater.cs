using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    internal class RoslynBasedModelUpdater
    {
        private readonly RoslynBasedModel _model;
        private readonly Workspace _workspace;

        public RoslynBasedModelUpdater(RoslynBasedModel model, Workspace workspace)
        {
            _model = model;
            _workspace = workspace;

            _workspace.WorkspaceChanged += UpdateModel;
        }

        private async void UpdateModel(object sender, WorkspaceChangeEventArgs workspaceChangeEventArgs)
        {
            Debug.WriteLine(workspaceChangeEventArgs.Kind);

            switch (workspaceChangeEventArgs.Kind)
            {
                case WorkspaceChangeKind.DocumentChanged:
                    await ProcessDocumentChangedEvent(workspaceChangeEventArgs);
                    break;
                case WorkspaceChangeKind.DocumentRemoved:
                    break;
            }
        }

        private async Task ProcessDocumentChangedEvent(WorkspaceChangeEventArgs workspaceChangeEventArgs)
        {
            var declaredTypeSymbols = await GetDeclaredTypeSymbols(workspaceChangeEventArgs.NewSolution,
                workspaceChangeEventArgs.ProjectId, workspaceChangeEventArgs.DocumentId);

            foreach (var declaredTypeSymbol in declaredTypeSymbols)
            {
                var roslynBasedModelEntity = _model.Entities.OfType<IRoslynBasedModelEntity>()
                    .FirstOrDefault(i => ReferenceEquals(i.RoslynSymbol, declaredTypeSymbol));

                if (roslynBasedModelEntity != null)
                    Debug.WriteLine($"Entity {roslynBasedModelEntity} found for symbol.");
                else
                    Debug.WriteLine($"Entity not found for symbol {declaredTypeSymbol.GetFullyQualifiedName()}");
            }
        }

        private static async Task<List<INamedTypeSymbol>> GetDeclaredTypeSymbols(Solution solution, ProjectId projectId, DocumentId documentId)
        {
            var project = solution.GetProject(projectId);
            var compilation = await project.GetCompilationAsync();

            var document = solution.GetDocument(documentId);
            var syntaxTree = await document.GetSyntaxTreeAsync();
            var typeDeclarationSyntaxNodes = syntaxTree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>();

            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var result = new List<INamedTypeSymbol>();
            foreach (var typeDeclarationSyntax in typeDeclarationSyntaxNodes)
            {
                var namedTypeSymbol = semanticModel.GetDeclaredSymbol(typeDeclarationSyntax) as INamedTypeSymbol;
                if (namedTypeSymbol != null)
                    result.Add(namedTypeSymbol);
            }

            return result;
        }
    }
}
