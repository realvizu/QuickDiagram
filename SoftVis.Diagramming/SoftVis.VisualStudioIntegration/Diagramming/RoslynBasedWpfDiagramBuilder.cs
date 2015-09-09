using System.Collections.Generic;
using System.Threading.Tasks;
using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Microsoft.CodeAnalysis;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    /// <summary>
    /// Builds a WPF-rendered diagram, and the underlying Roslyn-based model.
    /// </summary>
    public class RoslynBasedWpfDiagramBuilder
    {
        private ISourceDocumentProvider SourceDocumentProvider { get; }
        private RoslynBasedModel Model { get; }
        public Diagram Diagram { get; }

        public RoslynBasedWpfDiagramBuilder(ISourceDocumentProvider sourceDocumentProvider)
        {
            SourceDocumentProvider = sourceDocumentProvider;
            Model = new RoslynBasedModel();
            Diagram = new WpfDiagram();
        }

        public void Clear()
        {
            Diagram.Clear();
        }

        public async void AddCurrentSymbol()
        {
            var symbol = await GetCurrentSymbol();
            if (symbol is INamedTypeSymbol)
            {
                AddSymbolWithBaseAndChildren((INamedTypeSymbol)symbol);
                UpdateLayout();
            }
        }

        private async Task<ISymbol> GetCurrentSymbol()
        {
            var document = SourceDocumentProvider.GetCurrentDocument();
            if (document == null)
                return null;

            var syntaxTree = await document.GetSyntaxTreeAsync();
            var syntaxRoot = syntaxTree.GetRoot();
            var span = SourceDocumentProvider.GetSelection();
            var currentNode = syntaxRoot.FindNode(span);

            var semanticModel = await document.GetSemanticModelAsync();
            var symbolInfo = semanticModel.GetSymbolInfo(currentNode);
            var symbol = symbolInfo.Symbol ?? semanticModel.GetDeclaredSymbol(currentNode);
            return symbol;
        }

        private void AddSymbolWithBaseAndChildren(INamedTypeSymbol namedTypeSymbol)
        {
            AddBaseToDiagram(namedTypeSymbol);
            AddSymbolToDiagram(namedTypeSymbol);
            AddChildrenToDiagram(namedTypeSymbol);
        }

        private void AddChildrenToDiagram(INamedTypeSymbol namedTypeSymbol)
        { 
            var childTypeSymbols = GetChildTypeSymbols(namedTypeSymbol);
            foreach (var childTypeSymbol in childTypeSymbols)
            {
                AddSymbolToDiagram(childTypeSymbol);
                AddChildrenToDiagram(childTypeSymbol);
            }
        }

        private void AddBaseToDiagram(INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol.BaseType != null)
            {
                AddBaseToDiagram(namedTypeSymbol.BaseType);
                AddSymbolToDiagram(namedTypeSymbol.BaseType);
            }
        }

        private void AddSymbolToDiagram(INamedTypeSymbol namedTypeSymbol)
        {
            var modelElement = GetModelElement(namedTypeSymbol);
            AddClassToDiagram(modelElement);
        }

        private RoslynBasedClass GetModelElement(INamedTypeSymbol namedTypeSymbol)
        {
            return Model.GetOrAdd(namedTypeSymbol);
        }

        private void AddClassToDiagram(RoslynBasedClass @class)
        {
            Diagram.ShowNode(@class);
        }

        private IEnumerable<INamedTypeSymbol> GetChildTypeSymbols(ISymbol symbol)
        {
            var namedTypeSymbol = symbol as INamedTypeSymbol;
            if (namedTypeSymbol == null)
                return null;

            var workspace = SourceDocumentProvider.GetWorkspace();
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

        private void UpdateLayout()
        {
            Diagram.Layout();
        }
    }
}