using System;
using System.Linq;
using System.Threading.Tasks;
using Codartis.SoftVis.VisualStudioIntegration.Modeling;
using Codartis.Util;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Codartis.SoftVis.VisualStudioIntegration.UnitTests.Modeling.Implementation
{
    public class TestWorkspaceProvider : IRoslynWorkspaceProvider
    {
        [NotNull] private readonly AdhocWorkspace _workspace;
        [NotNull] private readonly ProjectId _projectId;

        public TestWorkspaceProvider()
        {
            _workspace = new AdhocWorkspace();

            var solutionInfo = SolutionInfo.Create(SolutionId.CreateNewId(), VersionStamp.Default);
            _workspace.AddSolution(solutionInfo);

            var projectInfo = ProjectInfo.Create(ProjectId.CreateNewId(), VersionStamp.Default, "UnitTestProject", "UnitTestProject", "C#");
            _projectId = _workspace.AddProject(projectInfo).Id;
        }

        public void AddSource(string sourceText, string documentName = "Test.cs")
        {
            _workspace.AddDocument(_projectId, documentName, SourceText.From(sourceText));
        }

        public async Task<ISymbol> GetSymbolAsync(string symbolName)
        {
            var compilation = await _workspace.CurrentSolution.GetProject(_projectId).GetCompilationAsync();
            var symbols = compilation.GetSymbolsWithName(symbolName);
            return symbols.First();
        }

        public Task<Workspace> GetWorkspaceAsync() => Task.FromResult((Workspace)_workspace);

        public Task<Maybe<ISymbol>> TryGetCurrentSymbolAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasSourceAsync(ISymbol symbol)
        {
            throw new NotImplementedException();
        }

        public Task ShowSourceAsync(ISymbol symbol)
        {
            throw new NotImplementedException();
        }
    }
}
