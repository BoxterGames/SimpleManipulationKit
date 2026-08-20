Scene setup:

1. Add MarqueeController and SelectionResetController to a scene object.
   Assign Interaction Space / Camera on MarqueeController if needed.
2. Put ManipulationObject, Collider and DragController3D on draggable 3D objects.
3. For UI use DragController with a raycast target (Graphic/Image).

Optional on MarqueeController:
- Interaction Space - root for marquee object search.
- Interaction Camera - defaults to Camera.main.
