# How to contribute

## How to build the source
1. Prerequisites
   * Visual Studio 2017 (any edition) with workloads:
     * .NET desktop development
     * Visual Studio extensions development
   * [Extensibility Tools for Visual Studio](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.ExtensibilityTools)
     *  For generating C# classes from VSCT and vsixmanifest files.
1. Optional, recommended
   * [Markdown Editor for Visual Studio](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.MarkdownEditor)
1. [Download or clone the source](https://github.com/realvizu/QuickDiagram)
1. Open "source\QuickDiagramTool.sln"
1. Build the solution.

## How to debug the tool in Visual Studio 2017
1. Set SoftVis.VisualStudioIntegration.Vs2017 as StartUp project.
1. SoftVis.VisualStudioIntegration.Vs2017 -> Project properties -> Debug
   * Start external program: C:\Program Files (x86)\Microsoft Visual Studio\2017\YourEdition\Common7\IDE\devenv.exe
   * Command line arguments: /rootsuffix Exp
1. Run the solution.

## How to debug the tool in Visual Studio 2015
1. Set SoftVis.VisualStudioIntegration.Vs2015 as StartUp project.
1. SoftVis.VisualStudioIntegration.Vs2015 -> Project properties -> Debug
   * Start external program: C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe
   * Command line arguments: /rootsuffix Exp
1. Run the solution.

