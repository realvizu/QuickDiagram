# How to...

## Build the source
1. Clone from https://github.com/realvizu/QuickDiagram.git
1. Open source\QuickDiagramTool.sln
1. Enable NuGet Package restore in Visual Studio
![Allowing NuGet package restore](images/doc/VSAllowNuGetPackageRestore.png)
1. Build the solution

## Debug the tool in Visual Studio
1. In the SoftVis.VisualStudioIntegration project set up debug start action.
   * StartProgram: C:\Program Files %28x86%29\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe
   * StartArguments: /rootsuffix Exp 
![Setting VSIX debug start action](images/doc/SetUpVsixDebugStartAction.png)
1. Set SoftVis.VisualStudioIntegration as StartUp project.
1. Run the solution.
