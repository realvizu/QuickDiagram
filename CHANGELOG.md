# Change log

## 0.5.63
(01/2020)

- [x] Visual Studio 2019 support.

## 0.5.61
(07/2017)

- [x] Bugfix: exception when adding related entities with the bubble list box.

## 0.5.60
(05/2017)

- [x] Visual Studio 2017 support.

## 0.5.56
(01/2017)

First experimental release to gather feedback.

- [x] Visual Studio 2015 extension.
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