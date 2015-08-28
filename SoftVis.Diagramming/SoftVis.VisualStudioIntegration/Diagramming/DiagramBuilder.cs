using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.VisualStudioIntegration.RoslynBasedModel;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codartis.SoftVis.VisualStudioIntegration.Diagramming
{
    public class DiagramBuilder
    {
        public static DiagramBuilder Instance { get; private set; }

        private readonly ISourceDocumentProvider _sourceDocumentProvider;
        private readonly RoslynBasedUmlModel _model;
        private readonly Diagram _diagram;

        private DiagramBuilder(ISourceDocumentProvider sourceDocumentProvider)
        {
            _sourceDocumentProvider = sourceDocumentProvider;
            _model = new RoslynBasedUmlModel();
            _diagram = new Diagram();
        }

        public static void Initialize(ISourceDocumentProvider sourceDocumentProvider)
        {
            Instance = new DiagramBuilder(sourceDocumentProvider);
        }

        public Diagram Diagram
        {
            get { return _diagram; }
        }

        public void Clear()
        {
            _diagram.Clear();
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
            var document = _sourceDocumentProvider.GetCurrentDocument();
            if (document == null)
                return null;

            var syntaxTree = await document.GetSyntaxTreeAsync();
            var syntaxRoot = syntaxTree.GetRoot();
            var span = _sourceDocumentProvider.GetSelection();
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
            AddModelElementToDiagram(modelElement);
        }

        private RoslynBasedUmlClass GetModelElement(INamedTypeSymbol namedTypeSymbol)
        {
            return _model.GetOrAdd(namedTypeSymbol);
        }

        private void AddModelElementToDiagram(RoslynBasedUmlClass modelElement)
        {
            _diagram.ShowModelElement(modelElement);
        }

        private IEnumerable<INamedTypeSymbol> GetChildTypeSymbols(ISymbol symbol)
        {
            var namedTypeSymbol = symbol as INamedTypeSymbol;
            if (namedTypeSymbol == null)
                return null;

            var workspace = _sourceDocumentProvider.GetWorkspace();
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
            _diagram.Layout();
        }
    }
}