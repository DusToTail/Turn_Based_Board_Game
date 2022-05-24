using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public partial class LevelEditorWindow
{
    public class DragAndDropManipulator : PointerManipulator
    {
        Label dropLabel;
        Object droppedObject = null;
        string assetPath = string.Empty;

        public DragAndDropManipulator(VisualElement root)
        {
            target = root.Q<VisualElement>(className: "drop_area");
            dropLabel = root.Q<Label>(className: "drop_label");
        }

        protected override void RegisterCallbacksOnTarget()
        {
            // Register a callback when the user presses the pointer down.
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            // Register callbacks for various stages in the drag process.
            target.RegisterCallback<DragEnterEvent>(OnDragEnter);
            target.RegisterCallback<DragLeaveEvent>(OnDragLeave);
            target.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
            target.RegisterCallback<DragPerformEvent>(OnDragPerform);
            target.RegisterCallback<DragExitedEvent>(OnDragExit);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            // Unregister all callbacks that you registered in RegisterCallbacksOnTarget().
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<DragEnterEvent>(OnDragEnter);
            target.UnregisterCallback<DragLeaveEvent>(OnDragLeave);
            target.UnregisterCallback<DragUpdatedEvent>(OnDragUpdate);
            target.UnregisterCallback<DragPerformEvent>(OnDragPerform);
            target.UnregisterCallback<DragExitedEvent>(OnDragExit);

        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            // Only do something if the window currently has a reference to an asset object.
            if (droppedObject != null)
            {
                if(droppedObject.GetType() == typeof(GameObject))
                // Clear existing data in DragAndDrop class.
                DragAndDrop.PrepareStartDrag();

                // Store reference to object and path to object in DragAndDrop static fields.
                DragAndDrop.objectReferences = new[] { droppedObject };
                if (assetPath != string.Empty)
                {
                    DragAndDrop.paths = new[] { assetPath };
                }
                else
                {
                    DragAndDrop.paths = new string[] { };
                }

                // Start a drag.
                DragAndDrop.StartDrag(string.Empty);
            }
        }

        private void OnPointerUp(PointerUpEvent evt)
        {

        }

        private void OnPointerMove(PointerMoveEvent evt)
        {

        }

        private void OnDragEnter(DragEnterEvent evt)
        {
            // Get the name of the object the user is dragging.
            var draggedName = string.Empty;
            if (DragAndDrop.paths.Length > 0)
            {
                assetPath = DragAndDrop.paths[0];
                var splitPath = assetPath.Split('/');
                draggedName = splitPath[splitPath.Length - 1];
            }
            else if (DragAndDrop.objectReferences.Length > 0)
            {
                draggedName = DragAndDrop.objectReferences[0].name;
            }

            // Change the appearance of the drop area if the user drags something over the drop area and holds it
            // there.
            dropLabel.text = $"Dropping '{draggedName}'...";
            target.AddToClassList("drop_area_dropping");

        }

        private void OnDragLeave(DragLeaveEvent evt)
        {
            assetPath = string.Empty;
            droppedObject = null;
            dropLabel.text = "Drag an asset here...";
            target.RemoveFromClassList("drop_area_dropping");

        }

        private void OnDragUpdate(DragUpdatedEvent evt)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        }

        private void OnDragPerform(DragPerformEvent evt)
        {
            // Set droppedObject and draggedName fields to refer to dragged object.
            droppedObject = DragAndDrop.objectReferences[0];
            dropLabel.text = "Drop here!";

            AddObjectToList((GameObject)droppedObject);

            target.RemoveFromClassList("drop_area_dropping");

        }

        private void OnDragExit(DragExitedEvent evt)
        {

        }




    }
}

