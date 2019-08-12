using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.Modeling.Definition;
using Codartis.SoftVis.VisualStudioIntegration.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Codartis.SoftVis.VisualStudioIntegration.Modeling.Implementation
{
    /// <summary>
    /// Not used. 
    /// It was an experiment to keep the model continuously updated by workspace changes.
    /// Should revisit later.
    /// </summary>
    internal class RoslynBasedModelUpdater
    {
        private readonly IModel _model;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Workspace _workspace;

        public RoslynBasedModelUpdater(IModel model, Workspace workspace)
        {
            _model = model;
            _workspace = workspace;

            _workspace.WorkspaceChanged += UpdateModel;
        }

        private async void UpdateModel(object sender, WorkspaceChangeEventArgs workspaceChangeEventArgs)
        {
            await UpdateModelAsync(workspaceChangeEventArgs);
        }

        private async Task UpdateModelAsync(WorkspaceChangeEventArgs workspaceChangeEventArgs)
        {
            Debug.WriteLine(workspaceChangeEventArgs.Kind);

            switch (workspaceChangeEventArgs.Kind)
            {
                case WorkspaceChangeKind.DocumentChanged:
                    await ProcessDocumentChangedEventAsync(workspaceChangeEventArgs);
                    break;
                case WorkspaceChangeKind.DocumentRemoved:
                    // TODO
                    break;
            }
        }

        private async Task ProcessDocumentChangedEventAsync(WorkspaceChangeEventArgs workspaceChangeEventArgs)
        {
            var declaredTypeSymbols = await GetDeclaredTypeSymbolsAsync(
                workspaceChangeEventArgs.NewSolution,
                workspaceChangeEventArgs.ProjectId,
                workspaceChangeEventArgs.DocumentId);

            foreach (var declaredTypeSymbol in declaredTypeSymbols)
            {
                // Match by name
                var matchingEntityByName = _model.GetRoslynNodes().FirstOrDefault(i => i.RoslynSymbol.SymbolEquals(declaredTypeSymbol));
                if (matchingEntityByName != null)
                {
                    Debug.WriteLine($"Found entity {declaredTypeSymbol.Name} by name.");
                    //_model.UpdateEntity(matchingEntityByName, declaredTypeSymbol);
                    continue;
                }

                // Match by location
                var matchingEntityByLocation = _model.GetNodeByLocation(declaredTypeSymbol.Locations.FirstOrDefault());
                if (matchingEntityByLocation != null)
                {
                    Debug.WriteLine($"Found entity {declaredTypeSymbol.Name} by location.");
                    //_model.UpdateEntity(matchingEntityByLocation, declaredTypeSymbol);
                    // ReSharper disable once RedundantJumpStatement
                    continue;
                }
            }
        }

        private static async Task<List<INamedTypeSymbol>> GetDeclaredTypeSymbolsAsync(Solution solution, ProjectId projectId, DocumentId documentId)
        {
            var document = solution.GetDocument(documentId);
            var syntaxTree = await document.GetSyntaxTreeAsync();
            var typeDeclarationSyntaxNodes = (await syntaxTree.GetRootAsync()).DescendantNodes().OfType<TypeDeclarationSyntax>();

            var project = solution.GetProject(projectId);
            var compilation = await project.GetCompilationAsync();
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            return typeDeclarationSyntaxNodes.Select(i => semanticModel.GetDeclaredSymbol(i)).OfType<INamedTypeSymbol>().ToList();
        }
    }
}