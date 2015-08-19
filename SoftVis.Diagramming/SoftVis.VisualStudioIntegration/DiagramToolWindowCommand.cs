using Codartis.SoftVis.VisualStudioIntegration.RoslynBasedModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Codartis.SoftVis.VisualStudioIntegration
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class DiagramToolWindowCommand
    {
        public const int CommandId = 0x0100;

        public static readonly Guid CommandSet = new Guid("3ec3e947-3047-4579-a09e-921b99ce5789");

        private readonly SoftVisPackage _package;
        private DiagramToolWindow _toolWindow;

        public static DiagramToolWindowCommand Instance { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagramToolWindowCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private DiagramToolWindowCommand(SoftVisPackage package)
        {
            if (package == null)
                throw new ArgumentNullException("package");

            _package = package;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(AddToDiagram, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        private IServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(SoftVisPackage package)
        {
            Instance = new DiagramToolWindowCommand(package);
        }

        private async void AddToDiagram(object sender, EventArgs e)
        {
            _toolWindow = GetToolWindow(_package);
            ShowToolWindow(_toolWindow);

            var symbol = await GetCurrentSymbol();
            var modelElement = GetModelElement(_package, symbol);
            AddModelElementToDiagram(modelElement, _toolWindow);
        }

        private static DiagramToolWindow GetToolWindow(SoftVisPackage package)
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            var toolWindow = (DiagramToolWindow)package.FindToolWindow(typeof(DiagramToolWindow), 0, true);
            if ((null == toolWindow) || (null == toolWindow.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }
            return toolWindow;
        }

        private static void ShowToolWindow(DiagramToolWindow toolWindow)
        {
            var windowFrame = (IVsWindowFrame)toolWindow.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        private static async Task<ISymbol> GetCurrentSymbol()
        {
            var document = SourceDocumentProvider.Instance.GetCurrentDocument();
            if (document == null)
                return null;

            var span = SourceDocumentProvider.Instance.GetSelection();
            Debug.Assert(span != null);

            var syntaxTree = await document.GetSyntaxTreeAsync();
            var semanticModel = await document.GetSemanticModelAsync();

            var syntaxRoot = syntaxTree.GetRoot();
            var actualNode = syntaxRoot.FindNode(new TextSpan(span.Start, span.Length));

            var symbolInfo = semanticModel.GetSymbolInfo(actualNode);
            var symbol = symbolInfo.Symbol ?? semanticModel.GetDeclaredSymbol(actualNode);

            return symbol;
        }

        private static RoslynBasedUmlModelElement GetModelElement(SoftVisPackage package, ISymbol symbol)
        {
            return package.Model.GetOrAdd(symbol);
        }

        private static void AddModelElementToDiagram(RoslynBasedUmlModelElement modelElement, DiagramToolWindow toolWindow)
        {
            toolWindow.Add(modelElement);
        }
    }
}
