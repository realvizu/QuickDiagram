rd /S /Q out
msbuild.exe QuickDiagramTool.sln /t:Clean;Build /p:Configuration=Release
md out
xcopy SoftVis.VisualStudioIntegration\bin\Release\net46\QuickDiagramTool.vsix out



