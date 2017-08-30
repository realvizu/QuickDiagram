using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.VisualStudioIntegration.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Not used. 
    /// It was an experiment to keep the model continously updated by workspace changes.
    /// Should revisit later.
    /// </summary>
    internal class RoslynBasedModelUpdater
    {
        private readonly RoslynModel _model;
        private readonly Workspace _workspace;

        public RoslynBasedModelUpdater(RoslynModel model, Workspace workspace)
        {
            _model = model;
            _workspace = workspace;

            _workspace.WorkspaceChanged += UpdateModelAsync;
        }

        private async void UpdateModelAsync(object sender, WorkspaceChangeEventArgs workspaceChangeEventArgs)
        {
            Debug.WriteLine(workspaceChangeEventArgs.Kind);

            switch (workspaceChangeEventArgs.Kind)
            {
                case WorkspaceChangeKind.DocumentChanged:
                    await ProcessDocumentChangedEvent(workspaceChangeEventArgs);
                    break;
                case WorkspaceChangeKind.DocumentRemoved:
                    // TODO
                    break;
            }
        }

        private async Task ProcessDocumentChangedEvent(WorkspaceChangeEventArgs workspaceChangeEventArgs)
        {
            var declaredTypeSymbols = await GetDeclaredTypeSymbols(workspaceChangeEventArgs.NewSolution,
                workspaceChangeEventArgs.ProjectId, workspaceChangeEventArgs.DocumentId);

            foreach (var declaredTypeSymbol in declaredTypeSymbols)
            {
                // Match by name
                var matchingEntityByName = _model.RoslynNodes.FirstOrDefault(i => i.RoslynSymbol.SymbolEquals(declaredTypeSymbol));
                if (matchingEntityByName != null)
                {
                    Debug.WriteLine($"Found entity {declaredTypeSymbol.Name} by name.");
                    //_model.UpdateEntity(matchingEntityByName, declaredTypeSymbol);
                    continue;
                }

                // Match by location
                var mathingEntityByLocation = _model.GetNodeByLocation(declaredTypeSymbol.Locations.FirstOrDefault());
                if (mathingEntityByLocation != null)
                {
                    Debug.WriteLine($"Found entity {declaredTypeSymbol.Name} by location.");
                    //_model.UpdateEntity(mathingEntityByLocation, declaredTypeSymbol);
                    continue;
                }
            }
        }

        private static async Task<List<INamedTypeSymbol>> GetDeclaredTypeSymbols(Solution solution, ProjectId projectId, DocumentId documentId)
        {
            var document = solution.GetDocument(documentId);
            var syntaxTree = await document.GetSyntaxTreeAsync();
            var typeDeclarationSyntaxNodes = syntaxTree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>();

            var project = solution.GetProject(projectId);
            var compilation = await project.GetCompilationAsync();
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            return typeDeclarationSyntaxNodes.Select(i => semanticModel.GetDeclaredSymbol(i)).OfType<INamedTypeSymbol>().ToList();
        }
    }
}
