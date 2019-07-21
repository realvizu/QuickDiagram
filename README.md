# Quick Diagram Tool for C# #
[![Build status](https://ci.appveyor.com/api/projects/status/sw2picivqnv5buj8?svg=true)](https://ci.appveyor.com/project/realvizu/softvis)

Code visualization tool for C# to quickly **explore, navigate and document** source code structure and relationships. 
Integrates into Visual Studio 2015 and 2017.
Similar to the Code Map feature of Visual Studio Enterprise Edition but more lightweight ... and free.

Install from: [![Visual Studio extension](https://vsmarketplacebadge.apphb.com/version/FerencVizkeleti.QuickDiagramToolforC.svg)](https://marketplace.visualstudio.com/items?itemName=FerencVizkeleti.QuickDiagramToolforC)

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

## Versions
See the [**Change Log**](CHANGELOG.md) for versions and road map.

The current version (v0.5) is an **experimental release** to showcase the approach of the tool. It supports only a few types of relationships: type inheritance and interface implementation. Later versions will show a lot more (namespaces, type members, method calls, property read/write, object creation, etc.)

## Feedback
* Please use the [**Issue Tracker**](https://github.com/realvizu/QuickDiagram/issues) to record bugs and feature requests.
* Write a [**review**](https://marketplace.visualstudio.com/items?itemName=FerencVizkeleti.QuickDiagramToolforC#review-details).
* Contact me on Twitter [![Follow on Titter](https://img.shields.io/twitter/url/http/realvizu.svg?style=social&label=@realvizu)](https://twitter.com/realvizu)

## Donation
If you find this tool useful please consider donating. Your support is much appreciated!

[![PayPal Donate button](https://www.paypalobjects.com/en_US/i/btn/btn_donate_SM.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=D8HJMA86C2KJA&item_name=Donation+for+Quick+Diagram+Tool+for+C%23&currency_code=USD&source=url)

## Thanks to 
* [Roslyn](https://github.com/dotnet/roslyn) for the best parser API.
* [QuickGraph](https://quickgraph.codeplex.com/) for the great graph library.
* [Graph#](http://graphsharp.codeplex.com/) for inspiration about WPF diagram canvas implementation and graph layout algorithms.
* [LearnVSXNow](https://learnvsxnow.codeplex.com/) for VSIX resources.
* [Extensibility Tools](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.ExtensibilityTools) for helping a lot in VSIX authoring.

## License
[GPL-2.0](LICENSE)
