# Quick Diagram Tool for C# #

> This project is archived.

## Project Status Update

This was an experimental project to prototype a code visualization tool for C# that is integrated with Visual Studio and enables you to quickly add source symbols to a diagram to explore their structure and relationships, similar to the CodeMap feature in Visual Studio Enterprise Edition but with a more lightweight approach.

The enhanced version of this tool is now called **Codartis Diagram Tool** and is available as a commercial product at **https://codartis.com** .

## The following is the old Readme.md content

Code visualization tool for C# to quickly **explore, navigate and document** source code structure and relationships. 
Integrates into Visual Studio 2015, 2017 and 2019.
Similar to the Code Map feature of Visual Studio Enterprise Edition but more lightweight ... and free.

**Explore code visually**
* Add types or entire hierarchies from source code to diagram to visualize their relationships.
* Use the diagram to discover related types.

**Navigate quickly**
* Jump from the diagram to relevant source code segments.

**Document with diagrams**
* Copy/paste diagrams into documentation.
 
## Getting Started
* Use the context menu in the source code editor to add types or entire hierarchies to the diagram.

![Add To Quick Diagram](images/doc/help/AddToQuickDiagramContextMenuItem.png)

* Use existing diagram nodes to discover their related entities. Dots on the sides of the diagram rectangles indicate that related entities exist.

![Show Related Entities Large](images/doc/help/ShowRelatedEntitiesLarge.png)

* Double-click on diagram nodes to jump to their declaration in the source code.
* Save diagrams as image or copy/paste them directly into documentation.

See the [**Help**](Help.md) for details.

## Thanks to 
* [Roslyn](https://github.com/dotnet/roslyn) for the best parser API.
* [QuickGraph](https://github.com/YaccConstructor/QuickGraph) for the great graph library.
* Graph# (sadly, Codeplex link is dead) for inspiration about WPF diagram canvas implementation and graph layout algorithms.
* [LearnVSXNow](https://github.com/umutozel/LearnVSXNow) for VSIX resources.
* [Extensibility Tools](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.ExtensibilityTools) for helping a lot in VSIX authoring.

## License
[GPL-2.0](LICENSE)
