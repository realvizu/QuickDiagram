rd /S /Q out
msbuild.exe SoftVis.Diagramming.sln /t:Clean;Build /p:Configuration=Release
md out
xcopy SoftVis.VisualStudioIntegration\bin\Release\QuickDiagramTool.vsix out



