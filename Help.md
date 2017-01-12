# Quick Diagram Tool Help

### Add a type to the diagram
* In the source code editor window position the caret on a token that has a type ...
* Right-click for the context menu
* "Add to Quick Diagram"

![Add To Quick Diagram](images/doc/help/AddToQuickDiagramContextMenuItem.png)

### Add a type hierarchy to the diagram
* "Add to Quick Diagram With Hierarchy" also adds the base type and all subtypes (recursively).

![Add To Quick Diagram With Hierarchy](images/doc/help/AddToDiagramWithHierarchy.png)

### Show/Hide descriptions
* Use the "Show/Hide Descriptions" toolbar button to show/hide type descriptions (the summary part of the type's XML documentation).

![Show Hide Descriptions](images/doc/help/ShowHideDescriptions.png)

### Extend the diagram with related entities
* Click on the pop-up mini-buttons on the diagram nodes to show related entities.

![Show Related Entities](images/doc/help/ShowRelatedEntities.png)

### Type features on the diagram
* Black dots on the side of the type's rectangle means that it has some related entities.
* *Italic* type name means abstract type.
* **Bold** type name means that it was found in source code.
* Normal (non-bold) type name means that it was found in metadata (referenced assembly).

![Type Features](images/doc/help/TypeFeatures.png)

### Navigate from the diagram to the corresponding source file
* Double-click on a diagram shape.
* It works only for those types that were found in the source code (and not in metadata).

### Remove items from the diagram
* Click on the "remove" mini-button on a shape to remove it from the diagram.

![Remove Diagram Node](images/doc/help/RemoveDiagramNode.png)

### Export diagram to file or copy to clipboard
* Use the "Copy to Clipboard" / "Save to File..." toolbar buttons to save/copy the diagram.
* Set the desired resolution in DPI (dots per inch).

![Export Or Copy Diagram](images/doc/help/ExportOrCopyDiagram.png)

### Clear the diagram
* Use the "Clear Diagram" toolbar button to remove all shapes from the diagram.

![Clear Diagram Button](images/doc/help/ClearDiagramButton.png)

### Update the diagram to refkect code changes
* Use the "Update Diagram from Code" toolbar button to synchronize the diagram with the changed code.

![Update Diagram From Code Button](images/doc/help/UpdateDiagramFromCodeButton.png)

* Updates those entities that have the same fully qualified name.
* Removes those entities that no longer exist in code.
* Unfortunately this feature can't track type renames so renamed types will be removed from the diagram and must be manually added back if needed.

### Pan and zoom on the diagram
* Use the mouse: 
  * Pan by holding down the left mouse button.
  * Zoom with the mouse wheel.
* Or use the keyboard (only if the diagram window has the focus): 
  * Pan with the cursor keys.
  * Zoom with W and S keys (FPS shooter-style :)
* Or use the pan and zoom control on the diagram.

![Pan And Zoom Control](images/doc/help/PanAndZoomControl.png)

### Zoom to the diagram
* Use the "Fit Diagram to Window" toolbar button, or the rectangle in the Pan and Zoom Control.

![Fit Diagram To Window](images/doc/help/FitDiagramToWindow.png)

### Open the Quick Diagram tool window
* You can open the Quick Diagram tool window from the Visual Studio menu bar:
* View > Other Windows > Quick Diagram
* Or use the keyboard shortcut: Ctrl+Shift+D, Ctrl+Shift+D

![Show Quick Diagram Tool Window](images/doc/help/ShowQuickDiagramToolWindow.png)