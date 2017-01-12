# Quick Diagram Tool for C# #
[![Build status](https://ci.appveyor.com/api/projects/status/sw2picivqnv5buj8?svg=true)](https://ci.appveyor.com/project/realvizu/softvis)

Code visualization tool for C# to quickly **explore, navigate and document** source code structure and relationships. Integrates into Visual Studio.

<!-- Update the VS Gallery link after you upload the VSIX-->
<!-- Download this extension from the [VS Gallery](https://visualstudiogallery.msdn.microsoft.com/[GuidFromGallery])
or get the [CI build](http://vsixgallery.com/extension/7481ead5-87e4-4ac0-86d1-317e7adab60c/). -->


Similar to the Code Map feature of Visual Studio Enterprise Edition but much more lightweight.

The current version (v0.5) is an **experimental release** to showcase the approach of the tool. It supports only a few types of relationships: type inheritance and interface implementation. Later versions will show a lot more (namespaces, type members, method calls, property read/write, object creation, etc.)

See the [change log](CHANGELOG.md) for versions and road map.

## Getting started
* Add types or entire hierarchies from source code to diagram to visualize their relationships.

![Add To Quick Diagram](images/doc/help/AddToQuickDiagramContextMenuItem.png)

* Use existing diagram nodes to discover their related entities.

![Show Related Entities Large](images/doc/help/ShowRelatedEntitiesLarge.png)

* Use the diagram to jump directly to relevant source code segments.
* Save diagrams as image or copy/paste directly into documentation.

[See the Help for details.](Help.md)

## License
[GPL-2.0](LICENSE)
