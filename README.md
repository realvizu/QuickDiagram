# Quick Diagram Tool for C# #
[![Build status](https://ci.appveyor.com/api/projects/status/sw2picivqnv5buj8?svg=true)](https://ci.appveyor.com/project/realvizu/softvis)

Code visualization tool for C# to quickly **explore, navigate and document** source code structure and relationships. Integrates into Visual Studio.

<!-- Update the VS Gallery link after you upload the VSIX-->
<!-- Download this extension from the [VS Gallery](https://visualstudiogallery.msdn.microsoft.com/[GuidFromGallery])
or get the [CI build](http://vsixgallery.com/extension/7481ead5-87e4-4ac0-86d1-317e7adab60c/). -->


Similar to the Code Map feature of Visual Studio Enterprise Edition but much more lightweight.

The current version (v0.5) is an **experimental release** to showcase the approach of the tool. It supports only a few types of relationships: type inheritance and interface implementation. Later versions will show a lot more (namespaces, type members, method calls, property read/write, object creation, etc.)

See the [change log](CHANGELOG.md) for versions and road map.

**Explore code visually**
- Add types or entire hierarchies from source code to diagram to visualize their relationships.
- Use existing diagram nodes to discover their related entities.

**Navigate quickly**
- Add those types to a diagram that you are focusing on.
- Use the diagram to jump directly to relevant source code segments.

**Document with diagrams**
- Paste diagrams into documentation.

## License
[GPL-2.0](LICENSE)
