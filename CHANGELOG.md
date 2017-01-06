# Change log

## 0.5

First experimental release to gather feedback.

- [x] Integrates into Visual Studio 2015.
- [x] Add entities from source code to diagram.
  - [x] Types: class, interface, struct, enum, delegate.
  - [x] Type features: description (xml comment summary), is abstract, is from source or metadata.
  - [x] Relationships: inherits, implements.
- [x] Visually extend diagram nodes with related entities.
- [x] Jump from diagram node to corresponding source code.
- [x] Export diagram image to file or clipboard.
- [x] Refresh diagram to reflect code changes.
  
## Road map

- [ ] More diagram content:
  - [ ] Show namespaces and dependencies.
  - [ ] Show assemblies and dependencies.
  - [ ] Show type members (methods, properties, fields, etc.)
  - [ ] Show call, read, write, create relationships.
- [ ] Diagram manipulations:
  - [ ] Select diagram nodes/relationships.
  - [ ] Move/remove selected shapes.
  - [ ] Smarter diagram layout.
  - [ ] Undo/redo diagram manipulation.
  - [ ] Minimap for navigating in large diagrams.
- [ ] Multiple diagram windows.
- [ ] Live automatic update of the diagram with code changes.
- [ ] Follow in-code navigations on the diagram to provide a continuous visual context.
- [ ] Refactor code by modifying the diagram.
- [ ] ...